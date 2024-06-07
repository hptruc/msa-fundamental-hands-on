using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace MSA.SagaOrchestrationStateMachine.Infrastructure.Saga;

public class OrderStateDbContext(DbContextOptions<OrderStateDbContext> options) : SagaDbContext(options)
{
    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}