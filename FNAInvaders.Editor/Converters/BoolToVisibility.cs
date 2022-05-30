using System.Globalization;
using System.Windows;

namespace FNAInvaders.Editor.Converters;

public class BoolToVisibility : MarkupConverter
{
    public bool Invert { get; set; }

    public bool IsHidden { get; set; }

    protected override object OnConvert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var flag = false;
        if (value is bool b)
        {
            flag = b;
        }
        return flag ^ Invert ? Visibility.Visible : IsHidden ? Visibility.Hidden : Visibility.Collapsed;
    }

    protected override object OnConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = false;
        if (value is Visibility visibility)
        {
            result = visibility == Visibility.Visible;
        }
        return result ^ Invert;
    }
}
