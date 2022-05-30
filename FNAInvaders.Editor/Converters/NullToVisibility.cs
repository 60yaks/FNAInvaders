using System.Globalization;
using System.Windows;

namespace FNAInvaders.Editor.Converters;

public class NullToVisibility : MarkupConverter
{
    public bool Invert { get; set; }

    public bool IsHidden { get; set; }

    protected override object OnConvert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ^ Invert ? IsHidden ? Visibility.Hidden : Visibility.Collapsed : Visibility.Visible;
    }
}
