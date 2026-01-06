using Mapster;
using Toplanti.Models;
using Toplanti.Models.DTOs;

namespace Toplanti.Infrastructure.Mappings;

/// <summary>
/// Entity'lerden DTO'lara mapping işlemlerini yöneten static sınıf
/// Mapster kütüphanesi kullanılarak implement edilmiştir
/// </summary>
public static class EntityMapper
{
    /// <summary>
    /// Meeting entity'den MeetingDto'ya mapping
    /// </summary>
    public static MeetingDto ToDto(Meeting meeting)
    {
        return meeting.Adapt<MeetingDto>();
    }

    /// <summary>
    /// Meeting collection'ından MeetingDto collection'ına mapping
    /// </summary>
    public static IEnumerable<MeetingDto> ToDto(IEnumerable<Meeting> meetings)
    {
        return meetings.Adapt<IEnumerable<MeetingDto>>();
    }

    /// <summary>
    /// Unit entity'den UnitDto'ya mapping
    /// </summary>
    public static UnitDto ToDto(Unit unit)
    {
        return unit.Adapt<UnitDto>();
    }

    /// <summary>
    /// Unit collection'ından UnitDto collection'ına mapping
    /// </summary>
    public static IEnumerable<UnitDto> ToDto(IEnumerable<Unit> units)
    {
        return units.Adapt<IEnumerable<UnitDto>>();
    }

    /// <summary>
    /// Decision entity'den DecisionDto'ya mapping
    /// </summary>
    public static DecisionDto ToDto(Decision decision)
    {
        return decision.Adapt<DecisionDto>();
    }

    /// <summary>
    /// Decision collection'ından DecisionDto collection'ına mapping
    /// </summary>
    public static IEnumerable<DecisionDto> ToDto(IEnumerable<Decision> decisions)
    {
        return decisions.Adapt<IEnumerable<DecisionDto>>();
    }

    /// <summary>
    /// Site entity'den SiteDto'ya mapping
    /// </summary>
    public static SiteDto ToDto(Site site)
    {
        return site.Adapt<SiteDto>();
    }

    /// <summary>
    /// Site collection'ından SiteDto collection'ına mapping
    /// </summary>
    public static IEnumerable<SiteDto> ToDto(IEnumerable<Site> sites)
    {
        return sites.Adapt<IEnumerable<SiteDto>>();
    }

    /// <summary>
    /// AgendaItem entity'den AgendaItemDto'ya mapping
    /// </summary>
    public static AgendaItemDto ToDto(AgendaItem agendaItem)
    {
        return agendaItem.Adapt<AgendaItemDto>();
    }

    /// <summary>
    /// AgendaItem collection'ından AgendaItemDto collection'ına mapping
    /// </summary>
    public static IEnumerable<AgendaItemDto> ToDto(IEnumerable<AgendaItem> agendaItems)
    {
        return agendaItems.Adapt<IEnumerable<AgendaItemDto>>();
    }

    /// <summary>
    /// Document entity'den DocumentDto'ya mapping
    /// </summary>
    public static DocumentDto ToDto(Document document)
    {
        return document.Adapt<DocumentDto>();
    }

    /// <summary>
    /// Document collection'ından DocumentDto collection'ına mapping
    /// </summary>
    public static IEnumerable<DocumentDto> ToDto(IEnumerable<Document> documents)
    {
        return documents.Adapt<IEnumerable<DocumentDto>>();
    }
}

