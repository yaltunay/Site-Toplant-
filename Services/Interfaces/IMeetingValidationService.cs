using Toplanti.Models;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Toplantı validasyon işlemlerini yöneten servis arayüzü
/// </summary>
public interface IMeetingValidationService
{
    ValidationResult ValidateMeetingContext(Meeting? meeting, string? errorMessage = null);
    ValidationResult ValidateMeetingNotCompleted(Meeting meeting, string? operationName = null);
    ValidationResult ValidateMeetingHasDecisions(Meeting meeting);
    ValidationResult ValidateMeetingAlreadyCompleted(Meeting meeting);
    ValidationResult ValidateMeetingAndContext(Meeting? meeting, Toplanti.Data.ToplantiDbContext? context, string? errorMessage = null);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };
}

