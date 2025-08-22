using DaradsHubAPI.Infrastructure;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DaradsHubAPI.Core.IRepository;

namespace DaradsHubAPI.Core.Repository;
public class GenericRepository<T>(AppDbContext _context) : IGenericRepository<T> where T : class
{
    private readonly DbSet<T> entities = _context.Set<T>();

    public IEnumerable<T> GetAll()
    {
        return entities;
    }

    public IEnumerable<TResult> GetAll<TResult>(Expression<Func<T, TResult>> select)
    {
        return entities.Select(select);
    }

    public IEnumerable<T> GetWhere(Expression<Func<T, bool>> expression)
    {
        return entities.Where(expression);
    }

    public IQueryable<T> Query(Expression<Func<T, bool>> expression)
    {
        return entities.Where(expression);
    }

    public IEnumerable<TResult> GetWhere<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, TResult>> select)
    {
        return entities.Where(expression).Select(select);
    }
    public async Task<T?> GetById(int id)
    {
        return await entities.FindAsync(id);
    }
    public async Task<T?> GetSingleWhereAsync(Expression<Func<T, bool>> expression)
    {
        return await entities.Where(expression).FirstOrDefaultAsync();
    }
    public T? GetSingleWhere(Expression<Func<T, bool>> expression)
    {
        return entities.Where(expression).FirstOrDefault();
    }

    public async Task Insert(T entity, bool save = true)
    {
        if (entity == null) throw new ArgumentNullException("Entity cannot be null");
        await entities.AddAsync(entity);
        if (save)
            await _context.SaveChangesAsync();
    }

    public async Task InsertRange(IEnumerable<T> entityList, bool save = true)
    {
        if (entityList == null) throw new ArgumentNullException("Entity list cannot be null");

        if (entityList.Count() > 0)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            await entities.AddRangeAsync(entityList);
            _context.ChangeTracker.DetectChanges();

            if (save)
                await _context.SaveChangesAsync();

            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    public async Task Update(T entity, bool save = true)
    {
        if (entity == null) throw new ArgumentNullException("Entity cannot be null");
        _context.Update<T>(entity);
        if (save)
            await _context.SaveChangesAsync();
    }

    public async Task UpdateRange(IEnumerable<T> entityList, bool save = true)
    {
        if (entityList == null) throw new ArgumentNullException("Entity list cannot be null");
        entityList.Select(u =>
        {
            return u;
        });
        _context.ChangeTracker.AutoDetectChangesEnabled = false;

        _context.UpdateRange(entityList);
        _context.ChangeTracker.DetectChanges();
        if (save)
            await _context.SaveChangesAsync();

        _context.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public async Task Delete(int id, bool save = true)
    {
        T? entity = await entities.FindAsync(id);
        if (entity == null)
            throw new ArgumentNullException($"Entity with id '{id}' does not exist");

        entities.Remove(entity);
        if (save)
            await _context.SaveChangesAsync();
    }

    public async Task DeleteWhere(Expression<Func<T, bool>> expression, bool save = true)
    {
        IEnumerable<T> entityList = entities.Where(expression);
        if (entityList.Count() > 0)
        {
            entities.RemoveRange(entityList);
            if (save)
                await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await entities.AnyAsync(expression);
    }
    public bool Any(Expression<Func<T, bool>> expression)
    {
        return entities.Any(expression);
    }
    public async Task<int> CountWhere(Expression<Func<T, bool>> expression)
    {
        return await entities.CountAsync(expression);
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}