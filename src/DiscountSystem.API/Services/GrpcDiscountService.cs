using DiscountSystem.Core.Interfaces;
using Grpc.Core;

namespace DiscountSystem.API.Services;

public class GrpcDiscountService(
    IDiscountService discountService,
    ILogger<GrpcDiscountService> logger) : Discount.DiscountBase
{
    private readonly IDiscountService _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
    private readonly ILogger<GrpcDiscountService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    public override async Task<GenerateResponse> GenerateCodes(
        GenerateRequest request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Received request to generate {Count} codes", request.Count);

            var result = await _discountService.GenerateCodesAsync(
                (int)request.Count,
                context.CancellationToken);

            return new GenerateResponse { Result = result.Success };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters");
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Request cancelled by client");
            throw new RpcException(new Status(StatusCode.Cancelled, "Operation cancelled"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error generating codes");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while generating codes"));
        }
    }

    public override async Task<UseCodeResponse> UseCode(
        UseCodeRequest request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Received request to use code");

            var success = await _discountService.UseCodeAsync(
                request.Code,
                context.CancellationToken);

            return new UseCodeResponse { Result = success ? 0u : 1u };
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Request cancelled by client");
            throw new RpcException(new Status(StatusCode.Cancelled, "Operation cancelled"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error using code");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while using the code"));
        }
    }
}