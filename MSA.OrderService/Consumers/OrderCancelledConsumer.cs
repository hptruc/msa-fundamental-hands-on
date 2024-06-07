using MassTransit;
using MSA.Common.Contracts.Commands.Order;
using MSA.Common.Contracts.Domain;
using MSA.Common.Contracts.Domain.Events.Order;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;

namespace MSA.OrderService.Consumers;

public class OrderCancelledConsumer(IRepository<Order> orderRepository, PostgresUnitOfWork<MainDbContext> uoW) : IConsumer<OrderCancelledEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var order = await orderRepository.GetAsync(context.Message.OrderId);
        if (order is not null)
        {
            order.OrderStatus = "Failed";
            
            await orderRepository.UpdateAsync(order);
            await uoW.SaveChangeAsync();

            await context.Publish(new OrderCancelled(context.Message.CorrelationId)
            { 
                OrderId = context.Message.OrderId
            });

            Console.WriteLine($"Order {context.Message.OrderId} cancelled.");
        }
    }
}