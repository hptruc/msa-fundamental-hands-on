using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.Common.Contracts.Settings;

namespace MSA.Common.PostgresMassTransit.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            configure.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceSetting = configuration?.GetSection(nameof(ServiceSetting))?.Get<ServiceSetting>();

                RabbitMQSetting? rabitMQSetting = configuration?.GetSection(nameof(RabbitMQSetting))?.Get<RabbitMQSetting>();

                configurator.Host(rabitMQSetting?.Host ?? throw new ArgumentException("RabbitMQ host is null"));
                configurator.ConfigureEndpoints(context,
                    new KebabCaseEndpointNameFormatter(serviceSetting?.ServiceName ?? throw new ArgumentException("Service name is null"), false));
                configurator.UseMessageRetry(retryPoilicy =>
                {
                    retryPoilicy.Interval(3, TimeSpan.FromSeconds(10));
                });
            });
        });

        return services;
    }

    public static IServiceCollection AddMassTransitWithPostgresOutbox<TDbContext>(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator>? furtherConfig = null)
        where TDbContext : AppDbContextBase
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            configure.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceSetting = configuration?.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();

                RabbitMQSetting? rabitMQSetting = configuration?.GetSection(nameof(RabbitMQSetting))?.Get<RabbitMQSetting>();

                configurator.Host(rabitMQSetting?.Host ?? throw new ArgumentException("RabbitMQ host is null"));
                configurator.ConfigureEndpoints(context,
                    new KebabCaseEndpointNameFormatter(serviceSetting?.ServiceName ?? throw new ArgumentException("Service name is null"), false));
                configurator.UseMessageRetry(retryPoilicy =>
                {
                    retryPoilicy.Interval(3, TimeSpan.FromSeconds(10));
                });
            });

            configure.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            furtherConfig?.Invoke(configure);
        });

        return services;
    }
}