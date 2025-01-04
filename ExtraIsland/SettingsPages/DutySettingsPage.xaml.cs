using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;
namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.duty","ExtraIsland·值日",PackIconKind.UsersOutline,PackIconKind.Users)]
public partial class DutySettingsPage {
    public DutySettingsPage() {
        Settings = GlobalConstants.ConfigHandlers.OnDuty!;
        InitializeComponent();
        UpdateOnDuty();
        Settings.OnDutyUpdated += UpdateOnDuty;
    }

    public OnDutyPersistedConfig Settings { get; set; }
    
    public string PeopleOnDuty { get; set; } = string.Empty;

    void DutySettingsPage_OnUnloaded(object sender,RoutedEventArgs e) {
        Settings.OnDutyUpdated -= UpdateOnDuty;
        Settings.Save();
    }
    void DataGrid_SelectedCellsChanged(object sender,SelectedCellsChangedEventArgs e) {
        Settings.Save();
    }
    void DeleteButton_Click(object sender,RoutedEventArgs e) {
        Button button = (sender as Button)!;
        if (button.DataContext is OnDutyPersistedConfigData.PeopleItem peopleItem) {
            Settings.Data.Peoples.Remove(peopleItem);
        }
    }

    void AddButton_Click(object sender, RoutedEventArgs e) {
        Settings.Data.Peoples.Add(new OnDutyPersistedConfigData.PeopleItem {
            Index = Settings.Data.Peoples.Count,
            Name = "新同学"
        });
        Settings.Save();
    }

    void UpdateOnDuty() {
        PeopleOnDutyLabel.Content = Settings.PeoplesOnDutyString;
    }
    
    [GeneratedRegex("[^0-9.-]+")]
    private static partial Regex NumberRegex();
    void TextBoxNumberCheck(object sender,TextCompositionEventArgs e) {
        Regex re = NumberRegex();
        e.Handled = re.IsMatch(e.Text);
    }
    
    public List<OnDutyPersistedConfigData.DutyStateData> DutyStates { get; } = [
        OnDutyPersistedConfigData.DutyStateData.Single,
        OnDutyPersistedConfigData.DutyStateData.Double,
        OnDutyPersistedConfigData.DutyStateData.InOut
    ]; 
}