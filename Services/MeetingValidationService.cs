using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Services;

/// <summary>
/// Toplantı validasyon işlemlerini yöneten servis (SRP, DRY)
/// </summary>
public class MeetingValidationService
{
    private readonly ToplantiDbContext _context;

    public MeetingValidationService(ToplantiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Toplantının null olup olmadığını ve context'in geçerli olup olmadığını kontrol eder
    /// </summary>
    public ValidationResult ValidateMeetingContext(Meeting? meeting, string? errorMessage = null)
    {
        if (meeting == null)
        {
            return ValidationResult.Failure(errorMessage ?? "Lutfen once bir toplanti secin.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Toplantının tamamlanmış olup olmadığını kontrol eder
    /// </summary>
    public ValidationResult ValidateMeetingNotCompleted(Meeting meeting, string? operationName = null)
    {
        if (meeting.IsCompleted)
        {
            var message = operationName != null
                ? $"Bu toplanti tamamlanmis. {operationName} yapilamaz."
                : "Bu toplanti tamamlanmis. Bu islem yapilamaz.";
            return ValidationResult.Failure(message);
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Toplantıya karar eklenip eklenmediğini kontrol eder
    /// </summary>
    public ValidationResult ValidateMeetingHasDecisions(Meeting meeting)
    {
        var decisionCount = _context.Decisions.Count(d => d.MeetingId == meeting.Id);
        if (decisionCount == 0)
        {
            return ValidationResult.Failure(
                "Toplantiyi tamamlamak icin en az bir karar/oylama eklenmis olmalidir.\n\n" +
                "Lutfen once bir karar ekleyin.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Toplantının zaten tamamlanmış olup olmadığını kontrol eder
    /// </summary>
    public ValidationResult ValidateMeetingAlreadyCompleted(Meeting meeting)
    {
        if (meeting.IsCompleted)
        {
            return ValidationResult.Failure("Bu toplanti zaten tamamlanmis.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Toplantı ve context validasyonunu birlikte yapar
    /// </summary>
    public ValidationResult ValidateMeetingAndContext(Meeting? meeting, ToplantiDbContext? context, string? errorMessage = null)
    {
        if (context == null || meeting == null)
        {
            return ValidationResult.Failure(errorMessage ?? "Lutfen once bir toplanti secin.");
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Validasyon sonucu modeli
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
}

