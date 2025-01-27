using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ExtraIsland.LifeMode.Components;

public partial class SleepySettings {
    public SleepySettings() {
        InitializeComponent();
    }
    
    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumberRegex();
    void TextBoxNumberCheck(object sender,TextCompositionEventArgs e) {
        Regex re = NumberRegex();
        e.Handled = re.IsMatch(e.Text) & e.Text.Length != 0;
    }
}