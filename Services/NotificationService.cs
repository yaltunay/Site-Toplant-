using System.Windows;

namespace Toplanti.Services;

/// <summary>
/// Kullanıcı bildirimlerini yöneten servis (DRY prensibi)
/// </summary>
public static class NotificationService
{
    /// <summary>
    /// Bilgi mesajı gösterir
    /// </summary>
    public static void ShowInfo(string message, string title = "Bilgi")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Uyarı mesajı gösterir
    /// </summary>
    public static void ShowWarning(string message, string title = "Uyari")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    /// <summary>
    /// Hata mesajı gösterir
    /// </summary>
    public static void ShowError(string message, string title = "Hata", Exception? exception = null)
    {
        var fullMessage = message;
        if (exception != null)
        {
            fullMessage += $"\n\nDetay: {exception.Message}";
        }
        MessageBox.Show(fullMessage, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// Başarı mesajı gösterir
    /// </summary>
    public static void ShowSuccess(string message, string title = "Basarili")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Onay mesajı gösterir
    /// </summary>
    public static MessageBoxResult ShowConfirmation(string message, string title = "Onay")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
    }

    /// <summary>
    /// Kritik hata mesajı gösterir ve uygulamayı kapatır
    /// </summary>
    public static void ShowCriticalError(string message, Exception? exception = null)
    {
        var fullMessage = $"{message}\n\nDetay: {exception?.ToString() ?? "Bilinmeyen hata"}";
        MessageBox.Show(fullMessage, "Kritik Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        Environment.Exit(1);
    }
}

