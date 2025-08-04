namespace DiscountSystem.Core.Models;

public class DiscountCode
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UsedAt { get; set; }
    public bool IsUsed => UsedAt.HasValue;
}