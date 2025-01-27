using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClassIsland.Core;
using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using ExtraIsland.TinyFeatures;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.master","ExtraIsland·主设置",PackIconKind.Cogs,PackIconKind.Cogs)]
public partial class MainSettingsPage {
    public MainSettingsPage() {
        Settings = GlobalConstants.Handlers.MainConfig!.Data;
        InitializeComponent();
    }
    public MainConfigData Settings { get; set; }
}