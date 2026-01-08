using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Document entity için repository implementation
/// </summary>
public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Document>> GetDocumentsByMeetingIdAsync(int meetingId)
    {
        return await _dbSet
            .Where(d => d.MeetingId == meetingId)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync();
    }
}

