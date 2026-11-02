using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
namespace OnlineShop.Repositories.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, ISqlEntity
{
    protected readonly SqlContext SqlContext;

    protected BaseRepository(SqlContext sqlContext)
    {
        SqlContext = sqlContext;
    }

    //Create
    public async Task<int> InsertAsync(TEntity entity)
    {
        await SqlContext.Set<TEntity>().AddAsync(entity);
        await SaveChangesAsync();
        return entity.Id;
    }
    public async Task Insert(TEntity entity)
    {
        await SqlContext.Set<TEntity>().AddAsync(entity);
    }

    //Read
    public async Task<List<TEntity>> GetAllAsync()
    {
        return await SqlContext.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await SqlContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await SqlContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    public async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await SqlContext.Set<TEntity>().Where(predicate).ToListAsync();
    }

    //Update 
    public void Update(TEntity entity, TEntity newEntity)
    {
        SqlContext.Entry(entity).CurrentValues.SetValues(newEntity);
    }
    public async Task UpdateAsync(TEntity entity, TEntity newEntity)
    {
        SqlContext.Entry(entity).CurrentValues.SetValues(newEntity);
        await SaveChangesAsync();

    }

    //Delete
    public void Delete(TEntity entity)
    {
        SqlContext.Set<TEntity>().Remove(entity);
    }
    public async Task DeleteAsync(TEntity entity)
    {
        SqlContext.Set<TEntity>().Remove(entity);
        await SaveChangesAsync();

    }

    public async Task DeleteByIdAsync(int id)
    {
        var entity = await SqlContext.Set<TEntity>().FindAsync(id);
        if (entity != null) await DeleteAsync(entity);
    }

    public async Task<int> WhereDeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = await SqlContext.Set<TEntity>().Where(predicate).ToListAsync();
        SqlContext.Set<TEntity>().RemoveRange(entities);
        return await SqlContext.SaveChangesAsync();
    }


    //other common operations
    public async Task<bool> SaveChangesAsync() => await SqlContext.SaveChangesAsync() > 0;
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) =>
        await SqlContext.Set<TEntity>().CountAsync(predicate);

    public async Task<long> SumAsync(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, long>> selector) =>
        await SqlContext.Set<TEntity>()
            .Where(predicate)
            .SumAsync(selector);
    public string ToCamelCase(string s)
    {
        if (s.Length < 2) return s.ToLower();
        return char.ToUpper(s[0]) + s[1..];
    }

    public async Task<PaginateResponse<TEntity>> Paginate(BaseFilterRequest filter, IQueryable<TEntity> queryable)
    {
        var page = filter.Page is > 0 ? filter.Page : 1;
        var pageSize = filter.Pager > 0 ? filter.Pager : 12;

        if (!string.IsNullOrWhiteSpace(filter.OrderBy))
        {
            var items = filter.OrderBy.Split(":");
            var orderBy = items[0];
            var desc = items[1];

            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderFunc =
                desc == "desc"
                    ? data => data.OrderByDescending(x => EF.Property<object>(x!, ToCamelCase(orderBy)))
                    : data => data.OrderBy(x => EF.Property<object>(x!, ToCamelCase(orderBy)));

            queryable = orderFunc(queryable);
        }
        else
        {
            queryable = queryable.OrderByDescending(x => EF.Property<object>(x!, ToCamelCase("Id")));
        }

        var res = await queryable
            .Skip((int)((page - 1) * pageSize))
            .Take((int)pageSize)
            .ToListAsync();

        var counts = 0;
        if (filter.Countable == true)
        {
            counts = await queryable.CountAsync();
        }

        return new PaginateResponse<TEntity>()
        {
            Items = res,
            Page = page,
            Pages = (int)Math.Ceiling(counts / (float)pageSize),
            Pager = pageSize,
            Total = counts
        };
    }



}