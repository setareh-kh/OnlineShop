using System.Linq.Expressions;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
namespace OnlineShop.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class, ISqlEntity
{
    //Create 
    Task<int> InsertAsync(TEntity entity);
    Task Insert(TEntity entity);

    //Read
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate);

    //Update 
    void Update(TEntity entity, TEntity newEntity);
    Task UpdateAsync(TEntity entity, TEntity newEntity);

    //Delete
    void Delete(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task DeleteByIdAsync(int id);
    Task<int> WhereDeleteAsync(Expression<Func<TEntity, bool>> predicate);

    //other common operations
    Task<bool> SaveChangesAsync();
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<long> SumAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, long>> selector);
    Task<PaginateResponse<TEntity>> Paginate(BaseFilterRequest filter, IQueryable<TEntity> queryable);
}