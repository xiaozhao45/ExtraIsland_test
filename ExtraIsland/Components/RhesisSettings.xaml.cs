using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ExtraIsland.Shared;
using Microsoft.Win32;

namespace ExtraIsland.Components;

public partial class RhesisSettings {
    public RhesisSettings() {
        InitializeComponent();
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumberRegex();
    
    void TextBoxNumberCheck(object sender, TextCompositionEventArgs e) {
        Regex re = NumberRegex();
        e.Handled = re.IsMatch(e.Text) & e.Text.Length != 0;
    }
    
    public List<RhesisDataSource> DataSources { get; } = [
        RhesisDataSource.All,
        RhesisDataSource.Hitokoto,
        RhesisDataSource.Jinrishici,
        RhesisDataSource.Saint,
        RhesisDataSource.SaintJinrishici,
        RhesisDataSource.LocalFile
    ];

    void BrowseFile_Click(object sender, RoutedEventArgs e) {
        var dialog = new OpenFileDialog {
            Filter = "文本文件|*.txt|所有文件|*.*",
            InitialDirectory = Path.GetFullPath(Environment.CurrentDirectory)
        };
        if (dialog.ShowDialog() == true) {
            Settings.LocalFilePath = dialog.FileName;
        }
    }
}

public class LimitIntToArgConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter,
        System.Globalization.CultureInfo culture) {
        return (int)value! switch {
            0 => "",
            _ => $"max_length={((int)value).ToString()}&"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter,
        System.Globalization.CultureInfo culture) {
        return 0;
    }
}
