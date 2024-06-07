using MSA.OrderService.Dtos;

namespace MSA.OrderService.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto?> PaymentProcessAsync(PaymentRequestDto paymentRequestDto, string token);
}
