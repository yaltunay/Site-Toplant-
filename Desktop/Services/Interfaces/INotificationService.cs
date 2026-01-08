using System.Windows;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Kullanıcı bildirimlerini yöneten servis arayüzü
/// </summary>
public interface INotificationService
{
    void ShowInfo(string message, string title = "Bilgi");
    void ShowWarning(string message, string title = "Uyari");
    void ShowError(string message, string title = "Hata", Exception? exception = null);
    void ShowSuccess(string message, string title = "Basarili");
    MessageBoxResult ShowConfirmation(string message, string title = "Onay");
    void ShowCriticalError(string message, Exception? exception = null);
}

