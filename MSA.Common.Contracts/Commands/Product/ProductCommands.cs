namespace MSA.Common.Contracts.Commands.Product;

public record ValidateProduct
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
}