using MSA.Common.Contracts.Settings;
using MSA.Common.PostgresMassTransit.MassTransit;
using MassTransit;
using MSA.SagaOrchestrationStateMachine.StateMachine;
using Microsoft.EntityFrameworkCore;
using MSA.SagaOrchestrationStateMachine.Infrastructure.Saga;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

SqliteDBSetting? serviceSetting = builder.Configuration.GetSection(nameof(SqliteDBSetting))?.Get<SqliteDBSetting>();

// Add services to the container.

builder.Services.AddDbContext<OrderStateDbContext>(x => {
    x.UseSqlite(serviceSetting?.ConnectionString ?? throw new ArgumentException("SqliteDB connection string is null"), n =>
    {
        n.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
        n.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
    });
});

builder.Services
    .AddMassTransitWithRabbitMQ(configure => {
        configure.AddSagaStateMachine<OrderStateMachine, OrderState>()
            .EntityFrameworkRepository(r =>
            {
                r.ExistingDbContext<OrderStateDbContext>();
                r.UseSqlite();
            });
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderStateDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();