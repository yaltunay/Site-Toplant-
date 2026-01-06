using FluentValidation;
using System.Text.RegularExpressions;
using Toplanti.Models;

namespace Toplanti.Infrastructure.Validation;

/// <summary>
/// Unit entity için FluentValidation validator
/// </summary>
public class UnitValidator : AbstractValidator<Unit>
{
    public UnitValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Birim numarası boş olamaz.")
            .MaximumLength(50)
            .WithMessage("Birim numarası en fazla 50 karakter olabilir.");

        RuleFor(x => x.OwnerName)
            .NotEmpty()
            .WithMessage("Mal sahibi adı boş olamaz.")
            .MaximumLength(200)
            .WithMessage("Mal sahibi adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .WithMessage("Ad en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .WithMessage("Soyad en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.Phone)
            .Must(BeValidTurkishPhone)
            .WithMessage("Telefon numarası geçerli bir Türk telefon formatında olmalıdır. (Format: 0XXX XXX XX XX)")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("E-posta adresi geçerli bir e-posta formatında olmalıdır.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.LandShare)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Arsa payı 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.UnitTypeId)
            .GreaterThan(0)
            .WithMessage("Birim tipi seçilmelidir.");

        RuleFor(x => x.SiteId)
            .GreaterThan(0)
            .WithMessage("Site seçilmelidir.")
            .When(x => x.SiteId.HasValue);
    }

    private static bool BeValidTurkishPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true; // Optional field

        // Türk telefon numarası formatı: 0XXX XXX XX XX veya 0XXXXXXXXX
        var pattern = @"^0[1-9]\d{2}\s?\d{3}\s?\d{2}\s?\d{2}$|^0[1-9]\d{9}$";
        return Regex.IsMatch(phone.Replace(" ", "").Replace("-", ""), pattern);
    }
}

