using FluentValidation;
using FluentValidation.Results;
using Toplanti.Models;

namespace Toplanti.Infrastructure.Validation;

/// <summary>
/// Validation işlemlerini yöneten servis
/// FluentValidation validator'larını kullanır
/// </summary>
public class ValidationService
{
    private readonly IValidator<Meeting> _meetingValidator;
    private readonly IValidator<Unit> _unitValidator;
    private readonly IValidator<Decision> _decisionValidator;
    private readonly IValidator<Site> _siteValidator;

    public ValidationService(
        IValidator<Meeting> meetingValidator,
        IValidator<Unit> unitValidator,
        IValidator<Decision> decisionValidator,
        IValidator<Site> siteValidator)
    {
        _meetingValidator = meetingValidator ?? throw new ArgumentNullException(nameof(meetingValidator));
        _unitValidator = unitValidator ?? throw new ArgumentNullException(nameof(unitValidator));
        _decisionValidator = decisionValidator ?? throw new ArgumentNullException(nameof(decisionValidator));
        _siteValidator = siteValidator ?? throw new ArgumentNullException(nameof(siteValidator));
    }

    /// <summary>
    /// Meeting entity'sini validate eder
    /// </summary>
    public ValidationResult ValidateMeeting(Meeting meeting)
    {
        var result = _meetingValidator.Validate(meeting);
        return ConvertToValidationResult(result);
    }

    /// <summary>
    /// Unit entity'sini validate eder
    /// </summary>
    public ValidationResult ValidateUnit(Unit unit)
    {
        var result = _unitValidator.Validate(unit);
        return ConvertToValidationResult(result);
    }

    /// <summary>
    /// Decision entity'sini validate eder
    /// </summary>
    public ValidationResult ValidateDecision(Decision decision)
    {
        var result = _decisionValidator.Validate(decision);
        return ConvertToValidationResult(result);
    }

    /// <summary>
    /// Site entity'sini validate eder
    /// </summary>
    public ValidationResult ValidateSite(Site site)
    {
        var result = _siteValidator.Validate(site);
        return ConvertToValidationResult(result);
    }

    private static ValidationResult ConvertToValidationResult(FluentValidation.Results.ValidationResult result)
    {
        if (result.IsValid)
        {
            return ValidationResult.Success();
        }

        var errorMessages = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
        return ValidationResult.Failure(errorMessages);
    }
}

/// <summary>
/// Validation sonucu
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };
}

