using MassTransit;
using MSA.Common.Contracts.Commands.Order;
using MSA.Common.Contracts.Domain.Events.Order;
using MSA.Common.Contracts.Domain.Events.Payment;

namespace MSA.SagaOrchestrationStateMachine.StateMachine;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public Event<OrderSubmitted> OrderSubmitted { get; private set; } = default!;
    public Event<OrderValidated> OrderValidated { get; private set; } = default!;
    public Event<OrderValidatedFailed> OrderValidatedFailed { get; private set; } = default!;
    public Event<PaymentProcessed> PaymentProcessed { get; private set; } = default!;
    public Event<PaymentProcessedFailed> PaymentProcessedFailed { get; private set; } = default!;
    public Event<OrderCompleted> OrderCompleted { get; private set; } = default!;
    public Event<OrderCancelled> OrderCancelled { get; private set; } = default!;

    public State Submitted { get; private set; } = default!;
    public State Processed { get; private set; } = default!;
    public State Confirmed { get; private set; } = default!;
    public State Cancelled { get; private set; } = default!;

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId)
            .SelectId(x => x.Message.CorrelationId = Guid.NewGuid()));
        Event(() => OrderValidated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderValidatedFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentProcessed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentProcessedFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderCompleted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderCancelled, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(OrderSubmitted)
                .Then(x => Console.WriteLine($"Order {x.Message.OrderId} submitted."))
                .Then(x => x.Saga.OrderId = x.Message.OrderId)
                .Publish(x => new OrderSubmittedValidation {
                    CorrelationId = x.Message.CorrelationId,
                    OrderId = x.Message.OrderId,
                    ProductIds = x.Message.ProductIds,
                    Amount = x.Message.Amount,
                    UserIdentity = x.Message.UserIdentity,
                })
                .TransitionTo(Submitted)
        );

        During(Submitted,
            When(OrderValidated)
                .Then(x => Console.WriteLine($"Order {x.Message.OrderId} validated."))
                .TransitionTo(Processed)
                .Publish(x => new OrderPaymentProcess { 
                    CorrelationId = x.Saga.CorrelationId,
                    OrderId = x.Saga.OrderId,
                    Amount = x.Message.Amount,
                    UserIdentity = x.Message.UserIdentity,
                }),
            When(OrderValidatedFailed)
                .Then(x => Console.WriteLine($"Order {x.Message.OrderId} validation failed."))
                .Then(x => {
                    x.Saga.Reason = x.Message.Reason;
                })
                .TransitionTo(Cancelled),
            Ignore(PaymentProcessed),
            Ignore(PaymentProcessedFailed)
        );

        During(Processed,
            When(PaymentProcessed)
                .Then(x => Console.WriteLine($"Payment for order {x.Message.OrderId} processed."))
                .Then(x => {
                    x.Saga.PaymentId = x.Message.PaymentId;
                })
                .TransitionTo(Confirmed)
                .Publish(x => new OrderCompletedEvent {
                    CorrelationId = x.Saga.CorrelationId,
                    OrderId = x.Message.OrderId,
                }),
            When(PaymentProcessedFailed)
                .Then(x => Console.WriteLine($"Payment for order {x.Message.OrderId} failed."))
                .Then(x => {
                    x.Saga.PaymentId = x.Message.PaymentId;
                    x.Saga.Reason = x.Message.Reason;
                })
                .TransitionTo(Cancelled)
                .Publish(x => new OrderCancelledEvent {
                    CorrelationId = x.Saga.CorrelationId,
                    OrderId = x.Saga.OrderId,
                }),
            Ignore(OrderCompleted),
            Ignore(OrderCancelled)
        );

        During(Confirmed,
            When(OrderCompleted)
                .Then(x => Console.WriteLine($"Order {x.Message.OrderId} completed."))
                .Finalize()
        );

        During(Cancelled,
            When(OrderCancelled)
                .Then(x => Console.WriteLine($"Order {x.Message.OrderId} cancelled."))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}