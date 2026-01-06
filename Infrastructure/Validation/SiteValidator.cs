using FluentValidation;
using Toplanti.Models;

namespace Toplanti.Infrastructure.Validation;

/// <summary>
/// Site entity için FluentValidation validator
/// </summary>
public class SiteValidator : AbstractValidator<Site>
{
    public SiteValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Site adı boş olamaz.")
            .MaximumLength(200)
            .WithMessage("Site adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.TotalLandShare)
            .GreaterThan(0)
            .WithMessage("Toplam arsa payı 0'dan büyük olmalıdır.");
    }
}

