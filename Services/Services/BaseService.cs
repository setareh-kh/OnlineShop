using System.Linq.Expressions;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services;

public class BaseService<TEntity>(IBaseRepository<TEntity> repository) : IBaseService<TEntity> where TEntity : class, ISqlEntity
{
    public async Task<IEnumerable<TEntity>?> GetAll() => await repository.GetAllAsync();
    public async Task<TEntity?> GetAsync(int id) => await repository.GetByIdAsync(id);
    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
        await repository.FindAsync(predicate);
    public async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate) =>
        await repository.WhereAsync(predicate);
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) =>
        await repository.CountAsync(predicate);
    public async Task<bool> SaveChanges() => await repository.SaveChangesAsync();
    protected static StandardResponse NotFound() =>
        new() { Success = false, Object = false, Message = "NotFound" };

    protected static StandardResponse Success(object obj) =>
        new() { Success = true, Object = obj };
  }