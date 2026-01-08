using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Document entity için özel repository interface
/// </summary>
public interface IDocumentRepository : IRepository<Document>
{
    /// <summary>
    /// Meeting ID'ye göre belgeleri getirir
    /// </summary>
    Task<IEnumerable<Document>> GetDocumentsByMeetingIdAsync(int meetingId);
}

