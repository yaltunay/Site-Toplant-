using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Data.Repositories;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class MeetingService : IMeetingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ToplantiDbContext _context; // Proxies için gerekli
    private readonly IQuorumService _quorumService;
    private readonly IProxyService _proxyService;
    private readonly IVotingService _votingService;

    public MeetingService(
        IUnitOfWork unitOfWork,
        ToplantiDbContext context,
        IQuorumService quorumService,
        IProxyService proxyService,
        IVotingService votingService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _quorumService = quorumService ?? throw new ArgumentNullException(nameof(quorumService));
        _proxyService = proxyService ?? throw new ArgumentNullException(nameof(proxyService));
        _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));
    }

    public async Task<Meeting> CreateMeetingAsync(string title, DateTime meetingDate, Site site)
    {
        var totalUnits = await _unitOfWork.Units.GetActiveUnitCountBySiteIdAsync(site.Id);
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

        await _unitOfWork.Meetings.AddAsync(meeting);
        await _unitOfWork.SaveChangesAsync();

        return meeting;
    }

    public async Task<bool> CompleteMeetingAsync(int meetingId)
    {
        var meeting = await _unitOfWork.Meetings.GetByIdAsync(meetingId);
        if (meeting == null) return false;

        meeting.IsCompleted = true;
        _unitOfWork.Meetings.Update(meeting);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<QuorumResult> CheckQuorumAsync(int meetingId)
    {
        var meeting = await _unitOfWork.Meetings.GetMeetingWithDetailsAsync(meetingId);

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
        _unitOfWork.Meetings.Update(meeting);
        await _unitOfWork.SaveChangesAsync();

        return new QuorumResult { Achieved = achieved, Message = message };
    }

    public async Task<string> GenerateMeetingMinutesAsync(int meetingId)
    {
        var meeting = await _unitOfWork.Meetings.GetMeetingWithDetailsAsync(meetingId);

        if (meeting == null)
            throw new InvalidOperationException("Toplantı bulunamadı.");

        var allUnits = await _unitOfWork.Units.FindAsync(u => u.IsActive);
        var proxies = await _context.Proxies
            .Where(p => p.MeetingId == meetingId)
            .Include(p => p.GiverUnit)
            .Include(p => p.ReceiverUnit)
            .ToListAsync();

        var minutesService = new MeetingMinutesService(_quorumService, _proxyService, _votingService);
        return minutesService.GenerateMeetingMinutes(meeting, allUnits.ToList(), proxies);
    }

    public async Task<IEnumerable<Meeting>> GetMeetingsBySiteIdAsync(int siteId)
    {
        return await _unitOfWork.Meetings.GetMeetingsBySiteIdAsync(siteId);
    }

    public async Task<Meeting?> GetMeetingByIdAsync(int meetingId)
    {
        return await _unitOfWork.Meetings.GetMeetingWithDetailsAsync(meetingId);
    }

    public async Task<DashboardStats> GetDashboardStatsAsync(int siteId)
    {
        var totalUnits = await _unitOfWork.Units.GetActiveUnitCountBySiteIdAsync(siteId);

        var siteUnitIds = await _unitOfWork.Units.FindAsync(u => u.SiteId == siteId && u.IsActive);
        var siteUnitIdList = siteUnitIds.Select(u => u.Id).ToList();

        var meetings = await _unitOfWork.Meetings.GetMeetingsBySiteIdAsync(siteId);
        var totalMeetings = meetings.Count();

        var meetingIds = siteUnitIdList.Any()
            ? await _unitOfWork.MeetingAttendances.GetMeetingIdsByUnitIdsAsync(siteUnitIdList)
            : Enumerable.Empty<int>();

        var totalDecisions = meetingIds.Any()
            ? await _unitOfWork.Decisions.CountDecisionsByMeetingIdsAsync(meetingIds)
            : 0;

        var site = await _unitOfWork.Sites.GetByIdAsync(siteId);
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

