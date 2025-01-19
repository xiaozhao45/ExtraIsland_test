using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ClassIsland.Core.Abstractions.Controls;
using ExtraIsland.Shared;

namespace ExtraIsland.Components;

public partial class RhesisSettings {
    public RhesisSettings() {
        InitializeComponent();
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumberRegex();
    void TextBoxNumberCheck(object sender,TextCompositionEventArgs e) {
        Regex re = NumberRegex();
        e.Handled = re.IsMatch(e.Text) & e.Text.Length != 0;
    }
    
    public List<RhesisDataSource> DataSources { get; } = [
        RhesisDataSource.All,
        RhesisDataSource.Hitokoto,
        RhesisDataSource.Jinrishici,
        RhesisDataSource.Saint,
        RhesisDataSource.SaintJinrishici
    ];
}