using MassTransit;

namespace MSA.SagaOrchestrationStateMachine.StateMachine;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public string? Reason { get; set; }
    public string? CurrentState { get; set; }
}