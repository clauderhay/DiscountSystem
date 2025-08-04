using DiscountSystem.API;
using Grpc.Core;
using Grpc.Net.Client;

namespace DiscountSystem.Client;

public class DiscountClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly Discount.DiscountClient _client;

    public DiscountClient(string address)
    {
        _channel = GrpcChannel.ForAddress(address);
        _client = new Discount.DiscountClient(_channel);
    }

    public async Task<bool> GenerateCodesAsync(int count)
    {
        try
        {
            var request = new GenerateRequest { Count = (uint)count };
            var response = await _client.GenerateCodesAsync(request);
            return response.Result;
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"gRPC error: {ex.Status.Detail}");
            return false;
        }
    }

    public async Task<bool> UseCodeAsync(string code)
    {
        try
        {
            var request = new UseCodeRequest { Code = code };
            var response = await _client.UseCodeAsync(request);
            return response.Result == 0;
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"gRPC error: {ex.Status.Detail}");
            return false;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        GC.SuppressFinalize(this);
    }

}