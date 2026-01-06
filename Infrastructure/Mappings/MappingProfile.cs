using Mapster;
using Toplanti.Models;
using Toplanti.Models.DTOs;

namespace Toplanti.Infrastructure.Mappings;

/// <summary>
/// Mapster mapping configuration
/// Entity'lerden DTO'lara mapping tanımlamaları
/// </summary>
public static class MappingProfile
{
    /// <summary>
    /// Mapping konfigürasyonlarını yapılandırır
    /// </summary>
    public static void ConfigureMappings()
    {
        // Meeting -> MeetingDto
        TypeAdapterConfig<Meeting, MeetingDto>
            .NewConfig()
            .Map(dest => dest.AgendaItemCount, src => src.AgendaItems != null ? src.AgendaItems.Count : 0)
            .Map(dest => dest.DocumentCount, src => src.Documents != null ? src.Documents.Count : 0)
            .Map(dest => dest.DecisionCount, src => src.Decisions != null ? src.Decisions.Count : 0);

        // Unit -> UnitDto
        TypeAdapterConfig<Unit, UnitDto>
            .NewConfig()
            .Map(dest => dest.UnitTypeName, src => src.UnitType != null ? src.UnitType.Name : null);

        // Decision -> DecisionDto
        TypeAdapterConfig<Decision, DecisionDto>
            .NewConfig()
            .Map(dest => dest.MeetingTitle, src => src.Meeting != null ? src.Meeting.Title : string.Empty)
            .Map(dest => dest.TotalVotes, src => src.YesVotes + src.NoVotes + src.AbstainVotes);

        // Site -> SiteDto
        TypeAdapterConfig<Site, SiteDto>
            .NewConfig()
            .Map(dest => dest.UnitCount, src => src.Units != null ? src.Units.Count(u => u.IsActive) : 0);
    }
}

