namespace MSA.OrderService.Dtos;

public class PaymentRequestDto
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}