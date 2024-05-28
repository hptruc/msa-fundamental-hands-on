using MassTransit;
using MSA.Common.Contracts.Domain;
using MSA.Common.Contracts.Domain.Events.Product;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;

namespace MSA.OrderService.Consumers;

public class ProductCreatedConsumer(
    IRepository<Product> productRepository,
    PostgresUnitOfWork<MainDbContext> uoW) : IConsumer<ProductCreated>
{
    public async Task Consume(ConsumeContext<ProductCreated> context)
    {
        var message = context.Message;
        
        Product product = new()
        {
            Id = new Guid(),
            ProductId = message.ProductId
        };

        await productRepository.CreateAsync(product);
        await uoW.SaveChangeAsync();
    }
}