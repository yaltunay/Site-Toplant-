using Toplanti.Data.Repositories;

namespace Toplanti.Data;

/// <summary>
/// Unit of Work pattern interface
/// Transaction yönetimi ve repository'lerin merkezi yönetimi için
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository'ler
    IMeetingRepository Meetings { get; }
    IUnitRepository Units { get; }
    ISiteRepository Sites { get; }
    IDecisionRepository Decisions { get; }
    IAgendaItemRepository AgendaItems { get; }
    IDocumentRepository Documents { get; }
    IMeetingAttendanceRepository MeetingAttendances { get; }
    IUnitTypeRepository UnitTypes { get; }

    /// <summary>
    /// Tüm değişiklikleri veritabanına kaydeder
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction başlatır
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction'ı commit eder
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction'ı rollback eder
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

