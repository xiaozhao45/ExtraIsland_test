using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClassIsland.Core.Abstractions.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

public partial class BetterCountdownSettings : ComponentBase<BetterCountdownConfig> {
    public BetterCountdownSettings() {
        InitializeComponent();
    }
    
    public List<CountdownAccuracy> CountdownAccuracies { get; set; } = [
        CountdownAccuracy.Day,
        CountdownAccuracy.Hour,
        CountdownAccuracy.Minute,
        CountdownAccuracy.Second
    ]; 
}

public class EnumDescriptionConverter : IValueConverter
{
    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Enum myEnum = (Enum)value!;
        string description = GetEnumDescription(myEnum);
        return description;
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Empty;
    }

    string GetEnumDescription(Enum? enumObj)
    {
        FieldInfo? fieldInfo = enumObj!.GetType().GetField(enumObj.ToString());

        object[] attribArray = fieldInfo!.GetCustomAttributes(false);

        if (attribArray.Length == 0)
        {
            return enumObj.ToString();
        }
        else
        {
            DescriptionAttribute? attrib = attribArray[0] as DescriptionAttribute;
            return attrib!.Description;
        }
    }
}