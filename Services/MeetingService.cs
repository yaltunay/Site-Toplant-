using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class MeetingService : IMeetingService
{
    private readonly ToplantiDbContext _context;
    private readonly IQuorumService _quorumService;
    private readonly IProxyService _proxyService;
    private readonly IVotingService _votingService;

    public MeetingService(
        ToplantiDbContext context,
        IQuorumService quorumService,
        IProxyService proxyService,
        IVotingService votingService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _quorumService = quorumService ?? throw new ArgumentNullException(nameof(quorumService));
        _proxyService = proxyService ?? throw new ArgumentNullException(nameof(proxyService));
        _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));
    }

    public async Task<Meeting> CreateMeetingAsync(string title, DateTime meetingDate, Site site)
    {
        var totalUnits = await _context.Units.CountAsync(u => u.SiteId == site.Id && u.IsActive);
        var totalLandShare = site.TotalLandShare;

        var meeting = new Meeting
        {
            Title = title,
            MeetingDate = meetingDate,
            TotalSiteLandShare = totalLandShare,
            TotalUnitCount = totalUnits,
            AttendedUnitCount = 0,
            AttendedLandShare = 0,
            QuorumAchieved = false,
            IsCompleted = false
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        return meeting;
    }

    public async Task<bool> CompleteMeetingAsync(int meetingId)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null) return false;

        meeting.IsCompleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<QuorumResult> CheckQuorumAsync(int meetingId)
    {
        var meeting = await _context.Meetings
            .Include(m => m.Attendances)
            .ThenInclude(a => a.Unit)
            .FirstOrDefaultAsync(m => m.Id == meetingId);

        if (meeting == null)
            return new QuorumResult { Achieved = false, Message = "Toplantı bulunamadı." };

        var attendedUnits = meeting.Attendances
            .Where(a => a.Unit != null)
            .Select(a => a.Unit!)
            .ToList();

        var attendedLandShare = attendedUnits.Sum(u => u.LandShare);
        var attendedCount = attendedUnits.Count;

        var (achieved, message) = _quorumService.CheckQuorum(
            meeting.TotalUnitCount,
            attendedCount,
            meeting.TotalSiteLandShare,
            attendedLandShare);

        meeting.AttendedUnitCount = attendedCount;
        meeting.AttendedLandShare = attendedLandShare;
        meeting.QuorumAchieved = achieved;
        await _context.SaveChangesAsync();

        return new QuorumResult { Achieved = achieved, Message = message };
    }

    public async Task<string> GenerateMeetingMinutesAsync(int meetingId)
    {
        var meeting = await _context.Meetings
            .Include(m => m.Decisions)
            .FirstOrDefaultAsync(m => m.Id == meetingId);

        if (meeting == null)
            throw new InvalidOperationException("Toplantı bulunamadı.");

        var allUnits = await _context.Units.Where(u => u.IsActive).ToListAsync();
        var proxies = await _context.Proxies
            .Where(p => p.MeetingId == meetingId)
            .Include(p => p.GiverUnit)
            .Include(p => p.ReceiverUnit)
            .ToListAsync();

        var minutesService = new MeetingMinutesService(_quorumService, _proxyService, _votingService);
        return minutesService.GenerateMeetingMinutes(meeting, allUnits, proxies);
    }

    public async Task<IEnumerable<Meeting>> GetMeetingsBySiteIdAsync(int siteId)
    {
        var siteUnitIds = await _context.Units
            .Where(u => u.SiteId == siteId && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync();

        return await _context.Meetings
            .Include(m => m.Attendances)
            .Where(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || !m.Attendances.Any())
            .OrderByDescending(m => m.MeetingDate)
            .ToListAsync();
    }

    public async Task<Meeting?> GetMeetingByIdAsync(int meetingId)
    {
        return await _context.Meetings
            .Include(m => m.Attendances)
                .ThenInclude(a => a.Unit)
                    .ThenInclude(u => u!.UnitType)
            .Include(m => m.Proxies)
                .ThenInclude(p => p.GiverUnit)
            .Include(m => m.Proxies)
                .ThenInclude(p => p.ReceiverUnit)
            .Include(m => m.AgendaItems)
            .Include(m => m.Documents)
            .Include(m => m.Decisions)
            .FirstOrDefaultAsync(m => m.Id == meetingId);
    }

    public async Task<DashboardStats> GetDashboardStatsAsync(int siteId)
    {
        var totalUnits = await _context.Units.CountAsync(u => u.SiteId == siteId && u.IsActive);

        var siteUnitIds = await _context.Units
            .Where(u => u.SiteId == siteId && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync();

        var totalMeetings = siteUnitIds.Any()
            ? await _context.Meetings
                .Include(m => m.Attendances)
                .CountAsync(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || !m.Attendances.Any())
            : await _context.Meetings.CountAsync();

        var meetingIds = siteUnitIds.Any()
            ? await _context.MeetingAttendances
                .Where(a => siteUnitIds.Contains(a.UnitId))
                .Select(a => a.MeetingId)
                .Distinct()
                .ToListAsync()
            : new List<int>();

        var totalDecisions = meetingIds.Any()
            ? await _context.Decisions.CountAsync(d => meetingIds.Contains(d.MeetingId))
            : 0;

        var site = await _context.Sites.FindAsync(siteId);
        var totalLandShare = site?.TotalLandShare ?? 0m;

        return new DashboardStats
        {
            TotalUnits = totalUnits,
            TotalMeetings = totalMeetings,
            TotalDecisions = totalDecisions,
            TotalLandShare = totalLandShare
        };
    }
}

