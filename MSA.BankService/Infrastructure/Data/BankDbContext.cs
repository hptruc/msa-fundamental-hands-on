using Microsoft.EntityFrameworkCore;
using MSA.BankService.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;

namespace MSA.BankService.Infrastructure.Data;

public class BankDbContext(IConfiguration configuration, DbContextOptions<BankDbContext> options) 
    : AppDbContextBase(configuration, options)
{
    private readonly string _uuidGenerator = "uuid-ossp";

    public DbSet<Payment> Orders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(_uuidGenerator);

        modelBuilder.Entity<Payment>().ToTable("payments");
        modelBuilder.Entity<Payment>().HasKey(x => x.Id);
        modelBuilder.Entity<Payment>().Property(x => x.Id).HasColumnType("uuid");
    }
}