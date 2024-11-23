using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClassIsland.Core.Abstractions.Controls;

namespace ExtraIsland.Components;

public partial class FluentClockSettings : ComponentBase<FluentClockConfig> {
    public FluentClockSettings() {
        InitializeComponent();
    }
}

[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBooleanConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object? parameter,
        System.Globalization.CultureInfo culture)
    {
        if (targetType != typeof(bool))
            throw new InvalidOperationException("The target must be a boolean");

        return !(bool)value!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter,
        System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    #endregion
}