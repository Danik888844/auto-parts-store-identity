using System.Linq.Expressions;

namespace AutoPartsIdentity.Core.DataAccess;

public interface IEntityRepository<T> where T: class, IEntity, new()
{
    List<T> GetAll(Expression<Func<T, bool>>? filter = null);
    T? Get(Expression<Func<T, bool>> filter);
    bool Any(Expression<Func<T, bool>>? filter = null);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Detach(T entity);
    
    // async
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task<T?> GetAsync(Expression<Func<T, bool>> filter);
    Task<bool> AnyAsync(Expression<Func<T, bool>>? filter = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}