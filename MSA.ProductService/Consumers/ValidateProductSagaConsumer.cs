using MassTransit;
using MSA.Common.Contracts.Domain.Events.Order;

namespace MSA.ProductService.Consumers;

public class ValidateProductSagaConsumer(ILogger<ValidateProductSagaConsumer> logger) : IConsumer<OrderSubmitted>
{
    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        await Task.Yield();

        var message = context.Message;
        logger.Log(LogLevel.Information, $"[Saga] Receiving message of order {message.OrderId} validating product {message.ProductId}");
    }
}