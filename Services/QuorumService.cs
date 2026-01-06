using Toplanti.Models;

namespace Toplanti.Services;

public class QuorumService
{
    public (bool achieved, string message) CheckQuorum(
        int totalUnitCount,
        int attendedUnitCount,
        decimal totalLandShare,
        decimal attendedLandShare)
    {
        var unitQuorum = (double)attendedUnitCount / totalUnitCount > 0.5;
        var landShareQuorum = attendedLandShare / totalLandShare > 0.5m;

        var achieved = unitQuorum && landShareQuorum;

        var message = achieved
            ? $"Toplantı yeter sayısı sağlanmıştır. Birim: {attendedUnitCount}/{totalUnitCount} (%{attendedUnitCount * 100.0 / totalUnitCount:F1}), Arsa Payı: {attendedLandShare:F2}/{totalLandShare:F2} (%{attendedLandShare * 100m / totalLandShare:F1})"
            : $"Toplantı yeter sayısı sağlanamamıştır. Birim: {attendedUnitCount}/{totalUnitCount} (%{attendedUnitCount * 100.0 / totalUnitCount:F1}), Arsa Payı: {attendedLandShare:F2}/{totalLandShare:F2} (%{attendedLandShare * 100m / totalLandShare:F1})";

        return (achieved, message);
    }

    public string GenerateQuorumNote(decimal totalLandShare, decimal attendedLandShare)
    {
        return $"KMK 30. Madde uyarınca; toplam {totalLandShare:F2} arsa payının {attendedLandShare:F2} kadarı temsil edilerek toplantı yeter sayısı sağlanmıştır.";
    }
}

