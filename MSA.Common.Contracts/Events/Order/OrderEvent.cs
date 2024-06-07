using MSA.Common.Contracts.Application.Common.Models;

namespace MSA.Common.Contracts.Domain.Events.Order;

public record OrderSubmitted : BaseEvent
{
    public Guid OrderId { get; init; }
    public List<Guid> ProductIds { get; init; } = [];
    public decimal Amount { get; init; }
};

public record OrderValidated : BaseEvent
{
    public OrderValidated(Guid correlationId) => CorrelationId = correlationId;
    public Guid OrderId { get; set; }
    public decimal Amount { get; init; }
}

public record OrderValidatedFailed : BaseEvent
{
    public OrderValidatedFailed(Guid correlationId) => CorrelationId = correlationId;
    public Guid OrderId { get; set; }
    public string? Reason { get; set; }
}

public record OrderCompleted : BaseEvent
{
    public OrderCompleted(Guid correlationId) => CorrelationId = correlationId;
    public Guid OrderId { get; set; }
}

public record OrderCancelled : BaseEvent
{
    public OrderCancelled(Guid correlationId) => CorrelationId = correlationId;
    public Guid OrderId { get; set; }
}