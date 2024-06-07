using MSA.Common.Contracts.Domain;

namespace MSA.BankService.Domain;

public class Payment : IEntity
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Status { get; set; }
}