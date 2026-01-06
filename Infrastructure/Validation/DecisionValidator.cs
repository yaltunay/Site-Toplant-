using FluentValidation;
using Toplanti.Models;

namespace Toplanti.Infrastructure.Validation;

/// <summary>
/// Decision entity için FluentValidation validator
/// </summary>
public class DecisionValidator : AbstractValidator<Decision>
{
    public DecisionValidator()
    {
        RuleFor(x => x.MeetingId)
            .GreaterThan(0)
            .WithMessage("Toplantı seçilmelidir.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Karar başlığı boş olamaz.")
            .MaximumLength(200)
            .WithMessage("Karar başlığı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Karar açıklaması boş olamaz.")
            .MaximumLength(2000)
            .WithMessage("Karar açıklaması en fazla 2000 karakter olabilir.");

        RuleFor(x => x.YesVotes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Evet oyları 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.NoVotes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Hayır oyları 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.AbstainVotes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Çekimser oyları 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.YesLandShare)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Evet arsa payı 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.NoLandShare)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Hayır arsa payı 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.AbstainLandShare)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Çekimser arsa payı 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.DecisionText)
            .MaximumLength(5000)
            .WithMessage("Karar metni en fazla 5000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.DecisionText));
    }
}

