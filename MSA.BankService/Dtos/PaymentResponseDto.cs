namespace MSA.BankService.Dtos;

public class PaymentResponseDto
{
    public Guid PaymentId { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
}