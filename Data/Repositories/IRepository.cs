using System.Linq.Expressions;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Generic repository interface
/// Tüm entity'ler için temel CRUD işlemlerini tanımlar
/// </summary>
/// <typeparam name="TEntity">Entity tipi</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Tüm entity'leri getirir
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Belirli bir koşula göre entity'leri getirir
    /// </summary>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// ID'ye göre entity getirir
    /// </summary>
    Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// İlk eşleşen entity'yi getirir
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Yeni entity ekler
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Birden fazla entity ekler
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Entity'yi günceller
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Entity'yi siler
    /// </summary>
    void Remove(TEntity entity);

    /// <summary>
    /// Birden fazla entity'yi siler
    /// </summary>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Koşula göre entity sayısını getirir
    /// </summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Koşula göre entity'nin var olup olmadığını kontrol eder
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
}

