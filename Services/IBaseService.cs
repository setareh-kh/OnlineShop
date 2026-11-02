using System.Linq.Expressions;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;


namespace OnlineShop.Services;

public interface IBaseService<TEntity> where TEntity : class, ISqlEntity
{
    Task<IEnumerable<TEntity>?> GetAll();
    Task<TEntity?> GetAsync(int id);
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> SaveChanges();
}