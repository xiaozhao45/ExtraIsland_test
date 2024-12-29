using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace ExtraIsland.Shared.Converters;

[ValueConversion(typeof(bool),typeof(bool))]
public class InverseBooleanConverter : IValueConverter {

    #region IValueConverter Members

    public object Convert(object? value,Type targetType,object? parameter,
        System.Globalization.CultureInfo culture) {
        if (targetType != typeof(bool))
            throw new InvalidOperationException("The target must be a boolean");

        return !(bool)value!;
    }

    public object ConvertBack(object? value,Type targetType,object? parameter,
        System.Globalization.CultureInfo culture) {
        throw new NotSupportedException();
    }

    #endregion

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

    static string GetEnumDescription(Enum? enumObj)
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