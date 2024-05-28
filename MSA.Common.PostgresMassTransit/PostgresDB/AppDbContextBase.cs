using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MSA.Common.Contracts.Settings;

namespace MSA.Common.PostgresMassTransit.PostgresDB;

public class AppDbContextBase(IConfiguration configuration, DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var serviceSetting = configuration
            .GetSection(nameof(ServiceSetting))
            .Get<ServiceSetting>();

        modelBuilder.HasDefaultSchema(serviceSetting?.ServiceName ?? throw new ArgumentException("Service name is null"));
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity(i => {
            i.ToTable("InboxState");
        });
        modelBuilder.AddOutboxMessageEntity(o => {
            o.ToTable("OutboxMessage");
        });
        modelBuilder.AddOutboxStateEntity(o => {
            o.ToTable("OutboxState");
        });
    }
}