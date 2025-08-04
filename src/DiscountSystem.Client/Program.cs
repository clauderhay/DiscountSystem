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
    Console.WriteLine("3. Stress Test");
    Console.WriteLine("4. Exit");
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
            await StressTest();
            break;
        case "4":
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

// This function is not necssary but it is to showcase that no duplicates are inserted regardless of the
// number of concurrent requests made. This is SemaphoreSlim doing its job.
async Task StressTest()
{
    Console.WriteLine("=== Stress Test Configuration ===");
    Console.Write("Number of concurrent requests: ");
    if (!int.TryParse(Console.ReadLine(), out var concurrent) || concurrent < 1)
    {
        Console.WriteLine("Invalid number of concurrent requests.");
        return;
    }
    
    Console.Write("Codes per request (1-2000): ");
    if (!int.TryParse(Console.ReadLine(), out var codesPerRequest) || codesPerRequest > 2000)
    {
        Console.WriteLine("Invalid number of codes per request.");
    }
    
    Console.WriteLine($"\nStarting stress test: {concurrent} concurrent requests, {codesPerRequest} codes each");
    Console.WriteLine($"Total codes to generate: {concurrent * codesPerRequest}");
    Console.WriteLine("\nPress any key to start...");
    Console.ReadKey();
    
    var stopwatch = Stopwatch.StartNew();
    var tasks = new List<Task<bool>>();

    for (var i = 0; i < concurrent; i++)
    {
        tasks.Add(client.GenerateCodesAsync(codesPerRequest));
    }

    var results = await Task.WhenAll(tasks);
    stopwatch.Stop();

    var successful = results.Count(r => r);
    var failed = results.Count(r => !r);

    Console.WriteLine("\n=== Stress Test Results ===");
    Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds}ms");
    Console.WriteLine($"Successful requests: {successful}");
    Console.WriteLine($"Failed requests: {failed}");
    Console.WriteLine($"Average time per request: {stopwatch.ElapsedMilliseconds / concurrent}ms");
    Console.WriteLine($"Codes generated per second: {(successful * codesPerRequest * 1000) / stopwatch.ElapsedMilliseconds}");
    Console.WriteLine();
}