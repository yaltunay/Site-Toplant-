using System.Globalization;
using System.Windows.Data;

namespace Toplanti.Converters;

public class BoolToYesNoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? "Evet" : "Hayır";
        }
        return "Hayır";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

