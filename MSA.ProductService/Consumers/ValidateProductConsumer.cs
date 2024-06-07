using MassTransit;
using MSA.Common.Contracts.Commands.Product;

namespace MSA.ProductService.Consumers;

public class ValidateProductConsumer(ILogger<ValidateProductConsumer> logger) : IConsumer<ValidateProduct>
{
    public async Task Consume(ConsumeContext<ValidateProduct> context)
    {
        await Task.Yield();

        var message = context.Message;
        logger.Log(LogLevel.Information, $"Receiving message of order {message.OrderId} validating product {message.ProductId}");
    }
}