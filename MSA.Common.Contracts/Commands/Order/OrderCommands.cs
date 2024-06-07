using MSA.Common.Contracts.Application.Common.Models;

namespace MSA.Common.Contracts.Commands.Order;

public record OrderSubmittedValidation : BaseEvent
{
    public Guid OrderId { get; set; }
    public List<Guid> ProductIds { get; set; } = [];
    public decimal Amount { get; set; }
}

public record OrderPaymentProcess : BaseEvent
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}

public record OrderCompletedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
}

public record OrderCancelledEvent : BaseEvent
{
    public Guid OrderId { get; set; }
}