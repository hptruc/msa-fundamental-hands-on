namespace MSA.Common.PostgresMassTransit.PostgresDB;

public class PostgresUnitOfWork<TDbContext>(TDbContext dbcontext)
    where TDbContext : AppDbContextBase
{
    public async Task SaveChangeAsync() => await dbcontext.SaveChangesAsync();
}