using Microsoft.AspNetCore.Mvc;
using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;
using MSA.OrderService.Dtos;
using MSA.Common.Contracts.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MassTransit;
using MSA.Common.Contracts.Domain.Events.Order;
using MSA.Common.Contracts.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace MSA.OrderService.Controllers;

[ApiController]
[Route("v1/order")]
[Authorize]
public class OrderController(IRepository<Order> repository, PostgresUnitOfWork<MainDbContext> uow, 
    IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpGet]
    [Authorize("read_access")]
    public async Task<IEnumerable<Order>> GetAsync()
    {
        var orders = (await repository.GetAllAsync()).ToList();
        return orders;
    }

    [HttpGet("{id}")]
    [Authorize("read_access")]
    public async Task<ActionResult<Order>> GetByIdAsync(Guid id)
    {
        var order = await repository.GetAsync(id);
        if (order is null)
        {
            return Ok();
        }
        return order;
    }

    [HttpPost]
    [Authorize("write_access")]
    public async Task<ActionResult<Order>> PostAsync(CreateOrderDto createOrderDto)
    {
        var order = new Order { 
            UserId = createOrderDto.UserId,
            OrderStatus = "Submitted"
        };

        await repository.CreateAsync(order);

        // Async Orchestrator
        await publishEndpoint.Publish(
            new OrderSubmitted {
                OrderId = order.Id,
                ProductIds = createOrderDto.ProductIds,
                Amount = new Random().Next(0, 101),
                UserIdentity = new UserIdentityModel
                { 
                    AccessToken = HttpContext.Request.Headers.Authorization.ToString() 
                }
            });

        await uow.SaveChangeAsync(); 

        return CreatedAtAction(nameof(PostAsync), order);
    }
}