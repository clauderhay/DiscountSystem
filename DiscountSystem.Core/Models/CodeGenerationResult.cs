namespace DiscountSystem.Core.Models;

public class CodeGenerationResult
{
    public bool Success { get; set; }
    public int RequestedCount { get; set; }
    public int GeneratedCount { get; set; }
}