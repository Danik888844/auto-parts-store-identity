using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsIdentity.Core.DataAccess;

public class EfEntityRepository<TEntity, TContext>: IEntityRepository<TEntity>
where TEntity: class,IEntity, new()
where TContext: DbContext, new()
{
    private readonly TContext _context;
    
    public EfEntityRepository(TContext context)
    {
        _context = context;
    }

    public List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null)
    {
        return filter == null
            ? _context.Set<TEntity>().OrderByDescending(x => x.Id).ToList()
            : _context.Set<TEntity>().Where(filter).OrderByDescending(x => x.Id).ToList();
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> filter)
    {
        return _context.Set<TEntity>().FirstOrDefault(filter);
    }

    public bool Any(Expression<Func<TEntity, bool>>? filter = null)
    {
        return filter == null
            ? _context.Set<TEntity>().Any()
            : _context.Set<TEntity>().Any(filter);
    }

    public void Add(TEntity entity)
    {
        if (entity is IEntity newEntity)
        {
            newEntity.CreatedDate = DateTime.UtcNow.Ticks;
            newEntity.ModifiedDate = DateTime.UtcNow.Ticks;
        }
        
        _context.Entry(entity).State = EntityState.Added;
        _context.SaveChanges();
    }

    public void Update(TEntity entity)
    {
        if (entity is IEntity updatedEntity)
        {
            updatedEntity.ModifiedDate = DateTime.UtcNow.Ticks;
        }
        
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }
    
    public void Delete(TEntity entity)
    {
        if (entity is IEntity deletedEntity)
        {
            deletedEntity.IsDeleted = true;
            deletedEntity.ModifiedDate = DateTime.UtcNow.Ticks;
        }
        
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Detach(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Detached;
        _context.SaveChanges();
    }
    
    
    // ASYNC
    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        return filter == null
            ? await _context.Set<TEntity>().OrderByDescending(x => x.Id).ToListAsync()
            : await _context.Set<TEntity>().Where(filter).OrderByDescending(x => x.Id).ToListAsync();
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(filter);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        return filter == null
            ? await _context.Set<TEntity>().AnyAsync()
            : await _context.Set<TEntity>().AnyAsync(filter);
    }

    public async Task AddAsync(TEntity entity)
    {
        if (entity is IEntity newEntity)
        {
            newEntity.CreatedDate = DateTime.UtcNow.Ticks;
            newEntity.ModifiedDate = DateTime.UtcNow.Ticks;
        }
        
        _context.Entry(entity).State = EntityState.Added;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (entity is IEntity updatedEntity)
        {
            updatedEntity.ModifiedDate = DateTime.UtcNow.Ticks;
        }
        
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(TEntity entity)
    {
        if (entity is IEntity deletedEntity)
        {
            deletedEntity.IsDeleted = true;
            deletedEntity.ModifiedDate = DateTime.UtcNow.Ticks;
        }
        
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}