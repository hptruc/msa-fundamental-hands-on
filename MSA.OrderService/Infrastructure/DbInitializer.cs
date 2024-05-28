using MSA.OrderService.Infrastructure.Data;

namespace MSA.OrderService.Infrastructure;

public class DbInitializer
{
    public static void InitDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var mainDbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
        mainDbContext?.Database.EnsureCreated();
    }
}