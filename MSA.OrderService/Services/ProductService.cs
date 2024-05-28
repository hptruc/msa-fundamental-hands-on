namespace MSA.OrderService.Services;

public class ProductService(HttpClient httpClient) : IProductService
{
    public async Task<bool> IsProductExisted(Guid id)
    {
        var result = await httpClient.GetStringAsync($"v1/product/{id}");
        _ = Guid.TryParse(result.Trim('"'), out Guid existedId);

        return existedId == id ? await Task.FromResult(true) : await Task.FromResult(false);
    }
}