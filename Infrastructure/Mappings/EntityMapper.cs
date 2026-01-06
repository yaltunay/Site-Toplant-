using Toplanti.Models;
using Toplanti.Models.DTOs;

namespace Toplanti.Infrastructure.Mappings;

/// <summary>
/// Entity'lerden DTO'lara mapping işlemlerini yöneten static sınıf
/// </summary>
public static class EntityMapper
{
    /// <summary>
    /// Meeting entity'den MeetingDto'ya mapping
    /// </summary>
    public static MeetingDto ToDto(Meeting meeting)
    {
        return new MeetingDto
        {
            Id = meeting.Id,
            MeetingDate = meeting.MeetingDate,
            Title = meeting.Title,
            Description = meeting.Description,
            TotalSiteLandShare = meeting.TotalSiteLandShare,
            TotalUnitCount = meeting.TotalUnitCount,
            AttendedUnitCount = meeting.AttendedUnitCount,
            AttendedLandShare = meeting.AttendedLandShare,
            QuorumAchieved = meeting.QuorumAchieved,
            IsCompleted = meeting.IsCompleted,
            CreatedAt = meeting.CreatedAt,
            AgendaItemCount = meeting.AgendaItems?.Count ?? 0,
            DocumentCount = meeting.Documents?.Count ?? 0,
            DecisionCount = meeting.Decisions?.Count ?? 0
        };
    }

    /// <summary>
    /// Meeting collection'ından MeetingDto collection'ına mapping
    /// </summary>
    public static IEnumerable<MeetingDto> ToDto(IEnumerable<Meeting> meetings)
    {
        return meetings.Select(ToDto);
    }

    /// <summary>
    /// Unit entity'den UnitDto'ya mapping
    /// </summary>
    public static UnitDto ToDto(Unit unit)
    {
        return new UnitDto
        {
            Id = unit.Id,
            Number = unit.Number,
            OwnerName = unit.OwnerName,
            FirstName = unit.FirstName,
            LastName = unit.LastName,
            Phone = unit.Phone,
            Email = unit.Email,
            LandShare = unit.LandShare,
            UnitTypeId = unit.UnitTypeId,
            UnitTypeName = unit.UnitType?.Name,
            Block = unit.Block,
            SiteId = unit.SiteId,
            IsActive = unit.IsActive
        };
    }

    /// <summary>
    /// Unit collection'ından UnitDto collection'ına mapping
    /// </summary>
    public static IEnumerable<UnitDto> ToDto(IEnumerable<Unit> units)
    {
        return units.Select(ToDto);
    }

    /// <summary>
    /// Decision entity'den DecisionDto'ya mapping
    /// </summary>
    public static DecisionDto ToDto(Decision decision)
    {
        return new DecisionDto
        {
            Id = decision.Id,
            MeetingId = decision.MeetingId,
            MeetingTitle = decision.Meeting?.Title ?? string.Empty,
            Title = decision.Title,
            Description = decision.Description,
            YesVotes = decision.YesVotes,
            NoVotes = decision.NoVotes,
            AbstainVotes = decision.AbstainVotes,
            YesLandShare = decision.YesLandShare,
            NoLandShare = decision.NoLandShare,
            AbstainLandShare = decision.AbstainLandShare,
            IsApproved = decision.IsApproved,
            DecisionText = decision.DecisionText,
            CreatedAt = decision.CreatedAt,
            TotalVotes = decision.YesVotes + decision.NoVotes + decision.AbstainVotes
        };
    }

    /// <summary>
    /// Decision collection'ından DecisionDto collection'ına mapping
    /// </summary>
    public static IEnumerable<DecisionDto> ToDto(IEnumerable<Decision> decisions)
    {
        return decisions.Select(ToDto);
    }

    /// <summary>
    /// Site entity'den SiteDto'ya mapping
    /// </summary>
    public static SiteDto ToDto(Site site)
    {
        return new SiteDto
        {
            Id = site.Id,
            Name = site.Name,
            TotalLandShare = site.TotalLandShare,
            UnitCount = site.Units?.Count(u => u.IsActive) ?? 0
        };
    }

    /// <summary>
    /// Site collection'ından SiteDto collection'ına mapping
    /// </summary>
    public static IEnumerable<SiteDto> ToDto(IEnumerable<Site> sites)
    {
        return sites.Select(ToDto);
    }

    /// <summary>
    /// AgendaItem entity'den AgendaItemDto'ya mapping
    /// </summary>
    public static AgendaItemDto ToDto(AgendaItem agendaItem)
    {
        return new AgendaItemDto
        {
            Id = agendaItem.Id,
            MeetingId = agendaItem.MeetingId,
            Title = agendaItem.Title,
            Description = agendaItem.Description,
            Order = agendaItem.Order,
            CreatedAt = agendaItem.CreatedAt
        };
    }

    /// <summary>
    /// AgendaItem collection'ından AgendaItemDto collection'ına mapping
    /// </summary>
    public static IEnumerable<AgendaItemDto> ToDto(IEnumerable<AgendaItem> agendaItems)
    {
        return agendaItems.Select(ToDto);
    }

    /// <summary>
    /// Document entity'den DocumentDto'ya mapping
    /// </summary>
    public static DocumentDto ToDto(Document document)
    {
        return new DocumentDto
        {
            Id = document.Id,
            MeetingId = document.MeetingId,
            Title = document.Title,
            DocumentType = document.DocumentType,
            Content = document.Content,
            CreatedAt = document.CreatedAt
        };
    }

    /// <summary>
    /// Document collection'ından DocumentDto collection'ına mapping
    /// </summary>
    public static IEnumerable<DocumentDto> ToDto(IEnumerable<Document> documents)
    {
        return documents.Select(ToDto);
    }
}

