using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Toplanti.ViewModels;

/// <summary>
/// Tüm ViewModel'lerin türeyeceği base sınıf
/// INotifyPropertyChanged implementasyonu sağlar
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Property değişikliklerini bildirir
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Property değerini set eder ve değişikliği bildirir
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

