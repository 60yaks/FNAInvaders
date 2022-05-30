using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace FNAInvaders.Editor.Converters;

public class NameSimplifier : MarkupConverter
{
    protected override object OnConvert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string name)
        {
            return DependencyProperty.UnsetValue;
        }

        var match = Regex.Match(name, @"
            ^(?<simplename>[A-Za-z0-9_.]+)$
            |
            ^(?<genericname>[A-Za-z0-9_.]+)`\d+\[(?<genericargs>[A-Za-z0-9_.,]+)\]$
            ", RegexOptions.IgnorePatternWhitespace);

        if (!match.Success)
        {
            return DependencyProperty.UnsetValue;
        }
        var simpleName = match.Groups["simplename"].Value;
        var genericName = match.Groups["genericname"].Value;
        var genericArgs = match.Groups["genericargs"].Value;

        if (!string.IsNullOrEmpty(simpleName))
        {
            return simpleName.Split('.')[^1];
        }
        if (!string.IsNullOrEmpty(genericName))
        {
            var args = match.Groups["genericargs"].Value.Split(',')
                .Select(a => a.Split('.')[^1]);
            return $"{genericName.Split('.')[^1]}<{string.Join(", ", args)}>";
        }

        return DependencyProperty.UnsetValue;
    }
}
