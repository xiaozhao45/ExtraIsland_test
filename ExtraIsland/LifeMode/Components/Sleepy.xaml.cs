using System.Windows;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ExtraIsland.Shared;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.LifeMode.Components;

[ComponentInfo(
    "529C3038-F1B8-49F8-AA15-165B770770DC",
    "Sleepy",
    PackIconKind.Bed,
    "不要视奸我啊啊啊啊啊"
)]
public partial class Sleepy {
    public Sleepy(ILessonsService lessonsService) {
        LessonsService = lessonsService;
        InitializeComponent();
    }

    ILessonsService LessonsService { get; }
    void Sleepy_OnLoaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked += Check;
    }
    void Sleepy_OnUnloaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked -= Check;
    }
    void Check(object? sender,EventArgs eventArgs) {
        this.BeginInvoke(() => {
            if (EiUtils.GetDateTimeSpan(Settings.LastUpdate,DateTime.Now) < Settings.UpdateInterval
                | Settings.UpdateIntervalSeconds == 0) return;
            Settings.LastUpdate = DateTime.Now;
            Update();
        });
    }

    void Update() {
        Data = SleepyHandler.SleepyApiData.Fetch(Settings.ApiUrl);
        StatLabel.Content = $"{Data.Info.Name}·{Data.Devices.Count}设备在线";
    }

    public SleepyHandler.SleepyApiData Data { get; private set; } = new SleepyHandler.SleepyApiData();
}