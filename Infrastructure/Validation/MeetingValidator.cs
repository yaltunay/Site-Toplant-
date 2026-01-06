using FluentValidation;
using Toplanti.Models;

namespace Toplanti.Infrastructure.Validation;

/// <summary>
/// Meeting entity için FluentValidation validator
/// </summary>
public class MeetingValidator : AbstractValidator<Meeting>
{
    public MeetingValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Toplantı başlığı boş olamaz.")
            .MaximumLength(200)
            .WithMessage("Toplantı başlığı en fazla 200 karakter olabilir.");

        RuleFor(x => x.MeetingDate)
            .NotEmpty()
            .WithMessage("Toplantı tarihi boş olamaz.")
            .Must(date => date >= DateTime.Today.AddYears(-10))
            .WithMessage("Toplantı tarihi geçerli bir tarih olmalıdır.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Açıklama en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.TotalSiteLandShare)
            .GreaterThan(0)
            .WithMessage("Toplam arsa payı 0'dan büyük olmalıdır.");

        RuleFor(x => x.TotalUnitCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Toplam birim sayısı 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.AttendedUnitCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Katılan birim sayısı 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.AttendedLandShare)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Katılan arsa payı 0 veya daha büyük olmalıdır.");

        RuleFor(x => x)
            .Must(meeting => meeting.AttendedUnitCount <= meeting.TotalUnitCount)
            .WithMessage("Katılan birim sayısı toplam birim sayısından fazla olamaz.")
            .When(x => x.TotalUnitCount > 0);

        RuleFor(x => x)
            .Must(meeting => meeting.AttendedLandShare <= meeting.TotalSiteLandShare)
            .WithMessage("Katılan arsa payı toplam arsa payından fazla olamaz.")
            .When(x => x.TotalSiteLandShare > 0);
    }
}

