using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
    }

    public OnDutyPersistedConfig Settings { get; set; }
    
    public string PeopleOnDuty { get; set; } = string.Empty;

    void DataGrid_SelectedCellsChanged(object sender,SelectedCellsChangedEventArgs e) {
        Settings.Save();
        UpdateOnDuty();
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
        UpdateOnDuty();
    }

    void UpdateOnDuty() {
        PeopleOnDutyLabel.Content = Settings.PeopleOnDuty.Name;
    }
}