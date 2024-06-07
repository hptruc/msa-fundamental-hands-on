using MSA.BankService.Domain;
using MSA.BankService.Dtos;
using MSA.BankService.Infrastructure.Data;
using MSA.Common.Contracts.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;

namespace MSA.BankService.Services;

public class PaymentService(IRepository<Payment> repository, PostgresUnitOfWork<BankDbContext> uow) : IPaymentService
{
    public async Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId)
    {
        var payment = await repository.GetAsync(x => x.OrderId == orderId);
        if (payment is null)
        {
            return new PaymentDto();
        }

        return new PaymentDto
        {
            OrderId = payment.OrderId,
            Id = payment.Id,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Status = payment.Status,
        };
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto)
    {
        var payment = new Payment
        {
            OrderId = paymentRequestDto.OrderId,
            Amount = paymentRequestDto.Amount,
            PaymentDate = DateTime.UtcNow,
            Status = paymentRequestDto.Amount <= 0 || paymentRequestDto.Amount > 100_000_000 
                ? "Failed" : "Completed"
        };

        await repository.CreateAsync(payment);
        await uow.SaveChangeAsync();

        return new PaymentResponseDto
        {
            PaymentId = payment.Id,
            Status = payment.Status,
            Reason = payment.Status != "Completed" 
                ? "Transaction failed. The payment amount exceeds the allowed limit. Please check the amount and try again."
                : ""
        };
    }
}