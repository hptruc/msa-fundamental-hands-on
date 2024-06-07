using Microsoft.AspNetCore.Mvc;
using MSA.ProductService.Dtos;
using MSA.ProductService.Entities;
using MSA.Common.Contracts.Domain;
using MassTransit;
using MSA.Common.Contracts.Domain.Events.Product;
using Microsoft.AspNetCore.Authorization;

namespace MSA.ProductService.Controllers;

[ApiController]
[Route("v1/product")]
[Authorize]
public class ProductController(IRepository<Product> repository, IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpGet]
    [Authorize("read_access")]
    public async Task<IEnumerable<ProductDto>> GetAsync()
    {
        var products = (await repository.GetAllAsync())
                        .Select(p => p.AsDto());
        return products;
    }

    //Get v1/product/123
    [HttpGet("{id}")]
    [Authorize("read_access")]
    public async Task<ActionResult<Guid>> GetByIdAsync(Guid id)
    {
        var product = await repository.GetAsync(id);
        if (product is null) 
        {
            return Ok(Guid.Empty);
        }

        return Ok(product.Id);
    }

    [HttpPost]
    [Authorize("write_access")]
    public async Task<ActionResult<ProductDto>> PostAsync(
        CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Id = new Guid(),
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };
        await repository.CreateAsync(product);

        await publishEndpoint.Publish(new ProductCreated{
            ProductId = product.Id
        });

        return CreatedAtAction(nameof(PostAsync), product.AsDto());
    }
}