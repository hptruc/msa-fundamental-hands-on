using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MSA.Common.Contracts.Domain;

namespace MSA.Common.PostgresMassTransit.PostgresDB;

public class PostgresRepository<TDbContext, TEntity>(TDbContext dbcontext) : IRepository<TEntity>
        where TDbContext : AppDbContextBase
        where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = dbcontext.Set<TEntity>();

    public async Task CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task DeleteAsync(TEntity entity)
    {
        await Task.FromResult(_dbSet.Remove(entity));
    }

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter) => await _dbSet.Where(filter).ToListAsync();

    public async Task<TEntity?> GetAsync(Guid id) => await _dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter) => await _dbSet.Where(filter).FirstOrDefaultAsync();

    public async Task UpdateAsync(TEntity entity)
    {
        await Task.FromResult(_dbSet.Update(entity));
    }
}