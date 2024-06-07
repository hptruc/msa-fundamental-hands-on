// using MassTransit;
// using MSA.BankService.Dtos;
// using MSA.BankService.Services;
// using MSA.Common.Contracts.Commands.Payment;
// using MSA.Common.Contracts.Domain.Events.Payment;

// namespace MSA.BankService.Consumers;

// public class PaymentProcessConsumer(IPaymentService paymentService) : IConsumer<PayOrder>
// {
//     public async Task Consume(ConsumeContext<PayOrderext)
//     {
//         PaymentResponseDto? paymentResponse = await paymentService.ProcessPaymentAsync(new PaymentRequestDto {
//             OrderId = context.Message.OrderId,
//             Amount = context.Message.Amount
//         });

//         if (paymentResponse is not null && paymentResponse.Status == "Completed")
//         {
//             await context.Publish(new PaymentProcessed(context.Message.CorrelationId)
//             { 
//                 OrderId = context.Message.OrderId,
//                 PaymentId = paymentResponse.PaymentId,
//             });
//         }
//         else
//         {
//             await context.Publish(new PaymentProcessedFailed(context.Message.CorrelationId)
//             { 
//                 PaymentId = paymentResponse?.PaymentId,
//                 OrderId = context.Message.OrderId,
//                 Reason = paymentResponse?.Reason ?? "Payment service is down. Please try again later."
//             });
//         }
//     }
// }