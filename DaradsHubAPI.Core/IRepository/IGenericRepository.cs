using System.Linq.Expressions;

namespace DaradsHubAPI.Core.IRepository;
public interface IGenericRepository<T> where T : class
{
    bool Any(Expression<Func<T, bool>> expression);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    Task<int> CountWhere(Expression<Func<T, bool>> expression);
    Task Delete(int id, bool save = true);
    Task DeleteWhere(Expression<Func<T, bool>> expression, bool save = true);
    IQueryable<T> GetAll();
    IQueryable<TResult> GetAll<TResult>(Expression<Func<T, TResult>> select);
    Task<T?> GetById(int id);
    T? GetSingleWhere(Expression<Func<T, bool>> expression);
    Task<T?> GetSingleWhereAsync(Expression<Func<T, bool>> expression);
    IEnumerable<T> GetWhere(Expression<Func<T, bool>> expression);
    IEnumerable<TResult> GetWhere<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, TResult>> select);
    Task Insert(T entity, bool save = true);
    Task InsertRange(IEnumerable<T> entityList, bool save = true);
    IQueryable<T> Query(Expression<Func<T, bool>> expression);
    Task SaveAsync();
    Task Update(T entity, bool save = true);
    Task UpdateRange(IEnumerable<T> entityList, bool save = true);
}