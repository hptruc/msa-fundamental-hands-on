using MSA.BankService.Dtos;

namespace MSA.BankService.Services;

public interface IPaymentService
{
    Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId);
    Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto);
}
