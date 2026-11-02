using System.Linq.Expressions;
using OnlineShop.Dtos.ResponseDtos;
using OnlineShop.Models;


namespace OnlineShop.Services;

public interface IBaseService<TEntity> where TEntity : class, ISqlEntity
{
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
    static StandardResponseDto NotFound() =>new() { Success = false, Object = false, Message = "NotFound" };
    static StandardResponseDto Success(object obj) =>new() { Success = true, Object = obj };
    Task<bool> SaveChanges();
    Task<IEnumerable<TEntity>?> GetAll();
    Task<TEntity?> GetAsync(int id);
}