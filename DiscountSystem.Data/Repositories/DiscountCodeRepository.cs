using DiscountSystem.Core.Interfaces;
using DiscountSystem.Core.Models;
using DiscountSystem.Data.Context;
using DiscountSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountSystem.Data.Repositories;

public class DiscountCodeRepository : IDiscountCodeRepository
{
    private readonly DiscountDbContext _context;

    public DiscountCodeRepository(DiscountDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DiscountCode?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var entity = await _context.DiscountCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(dc => dc.Code == code, ct);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task<HashSet<string>> GetExistingCodesAsync(IEnumerable<string> codes, CancellationToken ct = default)
    {
        var codesList = codes.ToList();
        var existing = await _context.DiscountCodes
            .Where(dc => codesList.Contains(dc.Code))
            .Select(dc => dc.Code)
            .ToListAsync(ct);

        return existing.ToHashSet();
    }

    public async Task BulkInsertAsync(IEnumerable<DiscountCode> codes, CancellationToken ct = default)
    {
        var entities = codes.Select(MapToEntity);
        await _context.DiscountCodes.AddRangeAsync(entities, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DiscountCode code, CancellationToken ct = default)
    {
        var entity = await _context.DiscountCodes
            .FirstOrDefaultAsync(dc => dc.Id == code.Id, ct);

        if (entity != null)
        {
            entity.UsedAt = code.UsedAt;
            entity.IsUsed = code.IsUsed;
            await _context.SaveChangesAsync(ct);
        }
    }

    private static DiscountCode MapToModel(DiscountCodeEntity entity)
    {
        return new DiscountCode
        {
            Id = entity.Id,
            Code = entity.Code,
            CreatedAt = entity.CreatedAt,
            UsedAt = entity.UsedAt
        };
    }

    private static DiscountCodeEntity MapToEntity(DiscountCode model)
    {
        return new DiscountCodeEntity
        {
            Id = model.Id,
            Code = model.Code,
            CreatedAt = model.CreatedAt,
            UsedAt = model.UsedAt,
            IsUsed = model.IsUsed
        };
    }
}