using MassTransit;
using MSA.Common.Contracts.Commands.Order;
using MSA.Common.Contracts.Domain.Events.Order;

namespace MSA.OrderService.Consumers;

public class OrderSubmittedValidationConsumer : IConsumer<OrderSubmittedValidation>
{
    public async Task Consume(ConsumeContext<OrderSubmittedValidation> context)
    {
        var isValid = context.Message.ProductIds.Count != 0;
        if (isValid)
        {
            await context.Publish(new OrderValidated(context.Message.CorrelationId)
            { 
                OrderId = context.Message.OrderId,
                Amount = context.Message.Amount,
                UserIdentity = context.Message.UserIdentity,
            });
        }
        else
        {
            await context.Publish(new OrderValidatedFailed(context.Message.CorrelationId)
            { 
                OrderId = context.Message.OrderId,
                Reason = "Order is empty"
            });
        }
    }
}