using MassTransit;
using MSA.Common.Contracts.Commands.Order;
using MSA.Common.Contracts.Domain.Events.Payment;
using MSA.OrderService.Dtos;
using MSA.OrderService.Services;

namespace MSA.OrderService.Consumers;

public class OrderPaymentProcessConsumer(IPaymentService paymentService) : IConsumer<OrderPaymentProcess>
{
    public async Task Consume(ConsumeContext<OrderPaymentProcess> context)
    {
        var accessToken = context.Message.UserIdentity?.AccessToken;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return;
        }
        
        PaymentResponseDto? paymentResponse = await paymentService.PaymentProcessAsync(new PaymentRequestDto
        {
            CorrelationId = context.Message.CorrelationId,
            OrderId = context.Message.OrderId,
            Amount = context.Message.Amount
        }, accessToken);

        if (paymentResponse is not null &&
            paymentResponse.Status == "Completed")
        {
            await context.Publish(new PaymentProcessed(context.Message.CorrelationId)
            {
                OrderId = context.Message.OrderId,
                PaymentId = paymentResponse.PaymentId,
            });
        }
        else
        {
            await context.Publish(new PaymentProcessedFailed(context.Message.CorrelationId)
            {
                PaymentId = paymentResponse?.PaymentId,
                OrderId = context.Message.OrderId,
                Reason = paymentResponse?.Reason ?? "Payment service is down. Please try again later."
            });
        }
    }
}

