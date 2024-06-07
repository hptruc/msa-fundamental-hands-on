using MSA.BankService.Infrastructure.Data;

namespace MSA.BankService.Infrastructure;

public class DbInitializer
{
    public static void InitDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();
        dbContext?.Database.EnsureCreated();
    }
}