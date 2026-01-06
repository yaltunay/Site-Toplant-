using System.Text.RegularExpressions;

namespace Toplanti.Services;

/// <summary>
/// Telefon numarası formatlama ve validasyon servisi
/// </summary>
public class PhoneNumberService
{
    private static readonly Regex _numericRegex = new(@"^[0-9]+$", RegexOptions.Compiled);
    private const int MaxPhoneLength = 11; // 0 + 10 rakam
    private const string PhoneFormat = "0 5XX XXX XX XX";

    /// <summary>
    /// Girdinin sadece rakam olup olmadığını kontrol eder
    /// </summary>
    public static bool IsNumericInput(string input)
    {
        return !string.IsNullOrEmpty(input) && _numericRegex.IsMatch(input);
    }

    /// <summary>
    /// Telefon numarasını temizler (boşluk, tire, parantez, + karakterlerini kaldırır)
    /// </summary>
    public static string CleanPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        return phone.Replace(" ", "")
                   .Replace("-", "")
                   .Replace("(", "")
                   .Replace(")", "")
                   .Replace("+", "");
    }

    /// <summary>
    /// Telefon numarasını Türkiye formatına çevirir: 0 5XX XXX XX XX
    /// </summary>
    public static string FormatPhoneNumber(string phone)
    {
        var cleaned = CleanPhoneNumber(phone);
        
        if (string.IsNullOrEmpty(cleaned))
            return string.Empty;

        // Maksimum 11 karakter
        if (cleaned.Length > MaxPhoneLength)
            cleaned = cleaned.Substring(0, MaxPhoneLength);

        var formatted = string.Empty;
        
        if (cleaned.StartsWith("0"))
        {
            formatted = cleaned.Substring(0, 1);
            if (cleaned.Length > 1) formatted += " " + cleaned.Substring(1, Math.Min(3, cleaned.Length - 1));
            if (cleaned.Length > 4) formatted += " " + cleaned.Substring(4, Math.Min(3, cleaned.Length - 4));
            if (cleaned.Length > 7) formatted += " " + cleaned.Substring(7, Math.Min(2, cleaned.Length - 7));
            if (cleaned.Length > 9) formatted += " " + cleaned.Substring(9);
        }
        else if (cleaned.Length <= 10)
        {
            formatted = "0";
            if (cleaned.Length > 0) formatted += " " + cleaned.Substring(0, Math.Min(3, cleaned.Length));
            if (cleaned.Length > 3) formatted += " " + cleaned.Substring(3, Math.Min(3, cleaned.Length - 3));
            if (cleaned.Length > 6) formatted += " " + cleaned.Substring(6, Math.Min(2, cleaned.Length - 6));
            if (cleaned.Length > 8) formatted += " " + cleaned.Substring(8);
        }

        return formatted;
    }

    /// <summary>
    /// Telefon numarasının geçerli olup olmadığını kontrol eder
    /// </summary>
    public static bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        var cleaned = CleanPhoneNumber(phone);

        // +90 ile başlıyorsa kaldır
        if (cleaned.StartsWith("+90"))
            cleaned = cleaned.Substring(3);

        // 0 ile başlıyorsa kaldır
        if (cleaned.StartsWith("0"))
            cleaned = cleaned.Substring(1);

        // Sadece rakam olmalı ve 10 haneli olmalı
        if (cleaned.Length != 10 || !cleaned.All(char.IsDigit))
            return false;

        // 5 ile başlamalı (cep telefonu)
        return cleaned.StartsWith("5");
    }

    /// <summary>
    /// Telefon numarası girişi için maksimum uzunluk kontrolü
    /// </summary>
    public static bool CanAddMoreCharacters(string currentPhone, string newInput)
    {
        var cleaned = CleanPhoneNumber(currentPhone);
        return cleaned.Length + newInput.Length <= MaxPhoneLength;
    }

    /// <summary>
    /// Telefon formatı açıklaması
    /// </summary>
    public static string GetFormatDescription() => PhoneFormat;
}

