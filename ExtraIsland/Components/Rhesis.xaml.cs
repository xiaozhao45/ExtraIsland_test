using System.Configuration;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Abstractions.Services.Management;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Plugin;
using ClassIsland.Shared.Abstraction.Models;
using ClassIsland.Shared.Interfaces;
using ExtraIsland.Shared;
using Google.Protobuf;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "FBB380C2-5480-4FED-8349-BA5F4EAD2688",
    "(未完成)名句一言",
    PackIconKind.MessageOutline,
    "显示一句古今名言,可使用三个API"
)]
public partial class Rhesis {
    public Rhesis(ILessonsService lessonsService) {
        LessonsService = lessonsService;
        InitializeComponent();
    }
    
    ILessonsService LessonsService { get; }
    
    public string Showing { get; private set; } = "-----------------";
    readonly RhesisHandler.Instance _rhesisHandler = new RhesisHandler.Instance();
    void Rhesis_OnLoaded(object sender,RoutedEventArgs e) {
        Settings.LastUpdate = DateTime.Now;
        Update();
        LessonsService.PostMainTimerTicked += UpdateEvent;
    }
    
    void Rhesis_OnUnloaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked -= UpdateEvent;
    }

    void UpdateEvent(object? sender,EventArgs eventArgs) {
        if (EiUtils.GetDateTimeSpan(Settings.LastUpdate,DateTime.Now) < Settings.UpdateTimeGap|Settings.UpdateTimeGapSeconds == 0) return;
        Settings.LastUpdate = DateTime.Now;
        Update();
    }
    
    void Update() {
        this.BeginInvoke(() => {
            Showing = _rhesisHandler.LegacyGet(Settings.DataSource,Settings.HitokotoProp switch {
                    "" => "https://v1.hitokoto.cn/",
                    _ => $"https://v1.hitokoto.cn/?{Settings.HitokotoLengthArgs}{Settings.HitokotoProp}"
                },
                Settings.SainticProp switch {
                    "" => "https://open.saintic.com/api/sentence/",
                    _ => $"https://open.saintic.com/api/sentence/{Settings.HitokotoProp}.json"
                },
                Settings.LengthLimitation).Content; 
            Label.Content = Showing;
        });
    }
}