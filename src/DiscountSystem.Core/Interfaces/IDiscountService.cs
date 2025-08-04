using DiscountSystem.Core.Models;

namespace DiscountSystem.Core.Interfaces;

public interface IDiscountService
{
    Task<CodeGenerationResult> GenerateCodesAsync(int count, CancellationToken ct = default);
    Task<bool> UseCodeAsync(string code, CancellationToken ct = default);
}