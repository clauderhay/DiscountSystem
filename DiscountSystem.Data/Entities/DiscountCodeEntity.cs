namespace DiscountSystem.Data.Entities;

public class DiscountCodeEntity
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public bool IsUsed { get; set; }
}