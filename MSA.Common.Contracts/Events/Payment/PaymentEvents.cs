namespace MSA.Common.Contracts.Domain.Events.Payment;

public record PaymentProcessed
{
    public PaymentProcessed(Guid correlationId) => CorrelationId = correlationId;

    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
}

public record PaymentProcessedFailed
{
    public PaymentProcessedFailed(Guid correlationId) => CorrelationId = correlationId;

    public Guid CorrelationId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string? Reason { get; set; }
}

