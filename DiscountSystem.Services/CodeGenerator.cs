using System.Security.Cryptography;
using DiscountSystem.Core.Interfaces;

namespace DiscountSystem.Services;

public class CodeGenerator : ICodeGenerator
{
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string GenerateCode()
    {
        var buffer = new byte[8];
        RandomNumberGenerator.Fill(buffer);
        
        var result = new  char[8];
        for (var i = 0; i < 8; i++)
        {
            result[i] = Characters[(buffer[i] % Characters.Length)];
        }
        
        return new string(result);
    }
}