namespace DiscountSystem.Data.Entities;

public class DiscountCodeEntity
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UsedAt { get; set; }
    public bool IsUsed { get; set; }
}