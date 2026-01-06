using System.Globalization;
using System.Windows.Data;

namespace Toplanti.Converters;

public class BoolToApprovedConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isApproved)
        {
            return isApproved ? "Kabul" : "Red";
        }
        return "Beklemede";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

