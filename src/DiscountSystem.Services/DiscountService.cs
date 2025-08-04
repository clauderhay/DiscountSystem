using DiscountSystem.Core.Interfaces;
using DiscountSystem.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DiscountSystem.Services;

public class DiscountService(
    IDiscountCodeRepository repository,
    ICodeGenerator codeGenerator,
    IMemoryCache cache,
    ILogger<DiscountService> logger) : IDiscountService
{
    private readonly IDiscountCodeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ICodeGenerator _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ILogger<DiscountService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
    
    public async Task<CodeGenerationResult> GenerateCodesAsync(int count, CancellationToken ct)
    {
        if (count is < 1 or > 2000)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be between 1 and 2000");
        }
        
        await _semaphoreSlim.WaitAsync(ct);
        try
        {
            _logger.LogInformation("Starting generation of {Count} discount codes", count);

            var uniqueCodes = new HashSet<string>();
            var attempts = 0;
            const int maxAttemptsPerCode = 3;

            while (uniqueCodes.Count < count && attempts < count * maxAttemptsPerCode)
            {
                ct.ThrowIfCancellationRequested();

                var batch = GenerateBatch(Math.Min(100, count - uniqueCodes.Count));
                var existingCodes = await _repository.GetExistingCodesAsync(batch, ct);

                foreach (var code in batch.Except(existingCodes))
                {
                    uniqueCodes.Add(code);
                    if (uniqueCodes.Count >= count) break;
                }

                attempts += batch.Count;
            }

            // This is for bulk insertion
            var discountCodes = uniqueCodes.Select(code => new DiscountCode
            {
                Id = Guid.NewGuid(),
                Code = code,
                CreatedAt = DateTime.UtcNow,
            }).ToList();

            await _repository.BulkInsertAsync(discountCodes, ct);

            _logger.LogInformation("Finished generation of {Count} discount codes", count);

            return new CodeGenerationResult
            {
                Success = discountCodes.Count == count,
                RequestedCount = count,
                GeneratedCount = discountCodes.Count
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Code Generation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating discount codes");
            throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<bool> UseCodeAsync(string code, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 8)
        {
            return false;
        }
        
        // Cache checking
        var cacheKey = $"used_{code}";
        if (_cache.TryGetValue<bool>(cacheKey, out _))
        {
            _logger.LogInformation("Code {Code} was already used", code);
            return false;
        }

        try
        {
            var discountCode = await _repository.GetByCodeAsync(code, ct);

            if (discountCode == null)
            {
                _logger.LogWarning("Attempted to use non-existent code");
                return false;
            }

            if (discountCode.IsUsed)
            {
                // We'll cache this code as 'used'
                _cache.Set(cacheKey, true, TimeSpan.FromHours(24));
                return false;
            }
            
            // Mark code as 'used'
            discountCode.UsedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(discountCode, ct);
            
            // Cache result
            _cache.Set(cacheKey, true, TimeSpan.FromHours(24));
            
            _logger.LogInformation("Discount code successfully used at {TimeStamp}", discountCode.UsedAt);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating discount code");
            throw;
        }
    }

    private HashSet<string> GenerateBatch(int count)
    {
        var batch = new HashSet<string>(count);
        while (batch.Count < count)
        {
            batch.Add(_codeGenerator.GenerateCode());
        }
        return batch;
    }
}