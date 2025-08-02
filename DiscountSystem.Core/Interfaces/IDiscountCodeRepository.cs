using DiscountSystem.Core.Models;

namespace DiscountSystem.Core.Interfaces;

public interface IDiscountCodeRepository
{
    Task<DiscountCode?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<HashSet<string>> GetExistingCodesAsync(IEnumerable<string> codes,  CancellationToken ct = default);
    Task BulkInsertAsync(IEnumerable<DiscountCode> codes, CancellationToken ct = default);
    Task UpdateAsync(DiscountCode code, CancellationToken ct = default);
}