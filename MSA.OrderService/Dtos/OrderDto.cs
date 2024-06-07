using Microsoft.AspNetCore.Identity;

namespace MSA.OrderService.Dtos;

public record CreateOrderDto
(
    Guid UserId,
    List<Guid> ProductIds
);