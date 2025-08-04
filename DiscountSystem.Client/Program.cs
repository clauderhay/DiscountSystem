using System.Diagnostics;
using DiscountSystem.Client;

Console.WriteLine("Hello, Lithuania!");

const string serverAddress = "http://localhost:5000";
using var client = new DiscountClient(serverAddress);

Console.WriteLine("===== Discount Code System Client =====");
Console.WriteLine($"Connected to: {serverAddress}");
Console.WriteLine();

while (true)
{
    Console.WriteLine("Choose an option:");
    Console.WriteLine("1. Generate discount codes");
    Console.WriteLine("2. Use a discount code");
    Console.WriteLine("3. Exit");
    Console.Write("\nChoose an option: ");
    
    var choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            await GenerateCodes();
            break;
        case "2":
            await UseCode();
            break;
        case "3":
            return;
        default:
            Console.WriteLine("Invalid choice. Try again. \n");
            break;
    }
}

async Task GenerateCodes()
{
    Console.Write("Input number of codes to generate (1-2000): ");
    if (int.TryParse(Console.ReadLine(), out var count) && count > 0 && count <= 2000)
    {
        Console.WriteLine($"\nGenerating {count} discount codes...");
        var stopwatch = Stopwatch.StartNew();

        var success = await client.GenerateCodesAsync(count);
        
        stopwatch.Stop();

        Console.WriteLine(success
            ? $"Successfully generated {count} discount codes in {stopwatch.ElapsedMilliseconds}ms."
            : "Invalid number. Please enter a value between 1 and 2000.");
        Console.WriteLine();
    }
}

async Task UseCode()
{
    Console.Write("Enter discount code: ");
    var code = Console.ReadLine()?.Trim().ToUpper();

    if (string.IsNullOrEmpty(code) || code.Length != 8)
    {
        Console.WriteLine("Invalid code format. Code must be 8 characters.");
        Console.WriteLine();
        return;
    }

    Console.WriteLine($"\nUsing discount code: {code}");
    var success = await client.UseCodeAsync(code);

    if (success)
    {
        Console.WriteLine("Code successfully used!");
    }
    else
    {
        Console.WriteLine("Failed to use code (invalid or already used)");
    }
    Console.WriteLine();
}