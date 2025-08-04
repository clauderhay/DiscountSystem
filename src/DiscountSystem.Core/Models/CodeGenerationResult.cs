namespace DiscountSystem.Core.Models;

public class CodeGenerationResult
{
    public bool Success { get; init; }
    public int RequestedCount { get; init; }
    public int GeneratedCount { get; init; }
}