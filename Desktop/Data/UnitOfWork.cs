using Microsoft.EntityFrameworkCore.Storage;
using Toplanti.Data.Repositories;

namespace Toplanti.Data;

/// <summary>
/// Unit of Work pattern implementation
/// Transaction yönetimi ve repository'lerin merkezi yönetimi
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ToplantiDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository'ler - lazy initialization
    private IMeetingRepository? _meetings;
    private IUnitRepository? _units;
    private ISiteRepository? _sites;
    private IDecisionRepository? _decisions;
    private IAgendaItemRepository? _agendaItems;
    private IDocumentRepository? _documents;
    private IMeetingAttendanceRepository? _meetingAttendances;
    private IUnitTypeRepository? _unitTypes;

    public UnitOfWork(ToplantiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IMeetingRepository Meetings =>
        _meetings ??= new MeetingRepository(_context);

    public IUnitRepository Units =>
        _units ??= new UnitRepository(_context);

    public ISiteRepository Sites =>
        _sites ??= new SiteRepository(_context);

    public IDecisionRepository Decisions =>
        _decisions ??= new DecisionRepository(_context);

    public IAgendaItemRepository AgendaItems =>
        _agendaItems ??= new AgendaItemRepository(_context);

    public IDocumentRepository Documents =>
        _documents ??= new DocumentRepository(_context);

    public IMeetingAttendanceRepository MeetingAttendances =>
        _meetingAttendances ??= new MeetingAttendanceRepository(_context);

    public IUnitTypeRepository UnitTypes =>
        _unitTypes ??= new UnitTypeRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Zaten aktif bir transaction var.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Aktif bir transaction yok.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return;
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

