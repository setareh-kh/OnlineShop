using System.Linq.Expressions;
using OnlineShop.Dtos.RequestDtos;
using OnlineShop.Dtos.ResponseDtos;
using OnlineShop.Models;
namespace OnlineShop.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class, ISqlEntity
{
    Task InsertAsync(TEntity entity);
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
    Task UpdateAsync(TEntity entity, TEntity newEntity);
    Task UpdateByIdAsync(TEntity newEntity);
    Task DeleteAsync(TEntity entity);
    Task DeleteByIdAsync(int id);
    Task<int> WhereDeleteAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> SaveChangesAsync();
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<long> SumAsync(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, long>> selector);
    Task<PaginateResponseDto<TEntity>> Paginate(BaseFilterRequest filter,IQueryable<TEntity> queryable);
}