using Toplanti.Models;

namespace Toplanti.Services;

public class MeetingMinutesService
{
    public string GenerateMeetingMinutes(Meeting meeting, ICollection<Unit> allUnits, ICollection<Proxy> proxies)
    {
        var minutes = new System.Text.StringBuilder();
        
        minutes.AppendLine("=== TOPLANTI TUTANAĞI ===");
        minutes.AppendLine();
        minutes.AppendLine($"Toplantı Tarihi: {meeting.MeetingDate:dd.MM.yyyy HH:mm}");
        minutes.AppendLine($"Toplantı Konusu: {meeting.Title}");
        if (!string.IsNullOrEmpty(meeting.Description))
        {
            minutes.AppendLine($"Açıklama: {meeting.Description}");
        }
        minutes.AppendLine();
        
        minutes.AppendLine("--- YETER SAYI (NİSAP) ---");
        minutes.AppendLine($"Toplam Birim Sayısı: {meeting.TotalUnitCount}");
        minutes.AppendLine($"Katılan Birim Sayısı: {meeting.AttendedUnitCount}");
        minutes.AppendLine($"Toplam Arsa Payı: {meeting.TotalSiteLandShare:F2}");
        minutes.AppendLine($"Katılan Arsa Payı: {meeting.AttendedLandShare:F2}");
        minutes.AppendLine();
        
        var quorumService = new QuorumService();
        var quorumNote = quorumService.GenerateQuorumNote(meeting.TotalSiteLandShare, meeting.AttendedLandShare);
        minutes.AppendLine(quorumNote);
        minutes.AppendLine($"Yeter Sayı Durumu: {(meeting.QuorumAchieved ? "SAĞLANDI" : "SAĞLANAMADI")}");
        minutes.AppendLine();
        
        if (proxies.Any())
        {
            minutes.AppendLine("--- VEKALETLER (KMK 31) ---");
            var proxyService = new ProxyService();
            var maxProxies = proxyService.CalculateMaxProxyCount(meeting.TotalUnitCount);
            minutes.AppendLine($"Maksimum Vekalet Sayısı: {maxProxies}");
            minutes.AppendLine($"Kullanılan Vekalet Sayısı: {proxies.Count}");
            minutes.AppendLine();
            
            foreach (var proxy in proxies)
            {
                var receiverInfo = proxy.ReceiverUnit != null 
                    ? $"{proxy.ReceiverUnit.Number} numaralı birime" 
                    : $"{proxy.ReceiverName} adlı kişiye";
                minutes.AppendLine($"  - {proxy.GiverUnit?.Number} numaralı birim, {receiverInfo} vekalet vermiştir.");
            }
            minutes.AppendLine();
        }
        
        if (meeting.Decisions.Any())
        {
            minutes.AppendLine("--- KARARLAR ---");
            var votingService = new VotingService();
            
            foreach (var decision in meeting.Decisions.OrderBy(d => d.CreatedAt))
            {
                minutes.AppendLine();
                minutes.AppendLine($"Karar: {decision.Title}");
                minutes.AppendLine($"Açıklama: {decision.Description}");
                minutes.AppendLine();
                minutes.AppendLine("Oylama Sonuçları:");
                minutes.AppendLine($"  Evet: {decision.YesVotes} birim ({decision.YesLandShare:F2} arsa payı)");
                minutes.AppendLine($"  Hayır: {decision.NoVotes} birim ({decision.NoLandShare:F2} arsa payı)");
                minutes.AppendLine($"  Çekimser: {decision.AbstainVotes} birim ({decision.AbstainLandShare:F2} arsa payı)");
                minutes.AppendLine($"  Karar Durumu: {(decision.IsApproved ? "KABUL EDİLDİ" : "REDDEDİLDİ")}");
                
                if (!string.IsNullOrEmpty(decision.DecisionText))
                {
                    minutes.AppendLine();
                    minutes.AppendLine($"Karar Metni: {decision.DecisionText}");
                }
            }
        }
        
        minutes.AppendLine();
        minutes.AppendLine($"Tutanak Oluşturulma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}");
        
        return minutes.ToString();
    }
}

