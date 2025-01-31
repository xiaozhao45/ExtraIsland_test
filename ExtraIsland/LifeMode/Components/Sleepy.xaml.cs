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
    "(生活模式)不要视奸我啊啊啊啊啊啊啊"
)]
public partial class Sleepy {
    public Sleepy(ILessonsService lessonsService) {
        LessonsService = lessonsService;
        InitializeComponent();
        labelAnimator = new Animators.ClockTransformControlAnimator(StatLabel);
    }

    Animators.ClockTransformControlAnimator labelAnimator;
    
    ILessonsService LessonsService { get; }
    void Sleepy_OnLoaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked += Check;
        LessonsService.PostMainTimerTicked += SlideShow;
    }
    void Sleepy_OnUnloaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked -= Check;
        LessonsService.PostMainTimerTicked -= SlideShow;
    }
    void Check(object? sender,EventArgs eventArgs) {
        new Thread(() => {
            if (EiUtils.GetDateTimeSpan(Settings.LastUpdate,DateTime.Now) < Settings.UpdateInterval
                | Settings.UpdateIntervalSeconds == 0) return;
            Settings.LastUpdate = DateTime.Now;
            Update();
        }).Start();
    }

    void Update() {
        Data = SleepyHandler.SleepyApiData.Fetch(Settings.ApiUrl);
        if (Settings.DeviceInfoShowingIntervalSeconds == 0) {
            this.BeginInvoke(() => {
                labelAnimator.Update($"{Data.Info.Name}·{Data.Devices.Count}设备在线",
                    Settings.IsAnimationEnabled,
                    Settings.IsSwapAnimationEnabled);
            });
        }
    }

    bool _renderLock;
    void SlideShow(object? sender,EventArgs eventArgs) {
        if (Settings.DeviceInfoShowingIntervalSeconds == 0 | _renderLock) return;
        _renderLock = true;
        new Thread(() => {
            this.BeginInvoke(() => {
                labelAnimator.Update($"{Data.Info.Name}·{Data.Devices.Count}设备在线",
                    Settings.IsAnimationEnabled,
                    Settings.IsSwapAnimationEnabled);
            });
            Thread.Sleep(Settings.DeviceInfoShowingInterval);
            if (Data.Devices.Count != 0) {
                foreach (SleepyHandler.SleepyApiData.SleepyDevice device in Data.Devices.Values.Where(device => device.Using)) {
                    this.BeginInvoke(() => {
                        labelAnimator.Update($"{device.ShowName}·{device.AppName}",
                            Settings.IsAnimationEnabled,
                            Settings.IsSwapAnimationEnabled);
                    });
                    Thread.Sleep(Settings.DeviceInfoShowingInterval);
                }
            }
            _renderLock = false;
        }).Start();
    }

    public SleepyHandler.SleepyApiData Data { get; private set; } = new SleepyHandler.SleepyApiData();
}