using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace FNAInvaders.Editor.Converters;

public abstract class MarkupConverter : MarkupExtension, IValueConverter
{
    protected abstract object OnConvert(object value, Type targetType, object parameter, CultureInfo culture);

    protected virtual object OnConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return OnConvert(value, targetType, parameter, culture);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return OnConvertBack(value, targetType, parameter, culture);
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
