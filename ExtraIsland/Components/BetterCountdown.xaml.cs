using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.CommonDialog;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "759FFA33-309F-6494-3903-E0693036197E",
    "更好的倒计时",
    PackIconKind.CalendarOutline,
    "提供更高级的功能"
)]
public partial class BetterCountdown : ComponentBase<BetterCountdownConfig> {
    public BetterCountdown() {
        InitializeComponent();
    }
    
    void OnLoad() {
        Thread t = new Thread(o => {
            Settings.TargetDate ??= DateTime.Now.ToString("s");
            Settings.Prefix ??= "现在";
            Settings.Suffix ??= "过去了";
            this.Invoke(() => {
                L1.Content = Settings.Prefix;
                Ls.Content = Settings.Suffix;
            });
            DetectCycle();
        });
        t.Start();
    }

    TimeSpan DiffDays(DateTime startTime, DateTime endTime) {
        TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return daysSpan;
    }
    
    void DetectCycle() {
        string days = string.Empty;
        string hours = string.Empty;
        string minutes = string.Empty;
        string seconds = string.Empty;
        while (true) {
            var span = DiffDays(DateTime.Now, Convert.ToDateTime(Settings.TargetDate));
            if (days != span.Days.ToString()) {
                days = span.Days.ToString();
                var d = days;
                SwapAnim(LDays,DTt,d);
            }
            if (hours != span.Hours.ToString()) {
                hours = span.Hours.ToString();
                var h = hours;
                SwapAnim(LHours,HTt,h);
            }
            if (minutes != span.Minutes.ToString()) {
                minutes = span.Minutes.ToString();
                var m = minutes;
                if (m.Length == 1) {
                    m = "0" + m;
                }
                SwapAnim(LMins,MTt,m);
            }
            if (seconds != span.Seconds.ToString()) {
                seconds = span.Seconds.ToString();
                var s = seconds;
                if (s.Length == 1) {
                    s = "0" + s;
                }
                SwapAnim(LSecs,STt,s);
            }
            Thread.Sleep(100);
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    static readonly Duration AnimDuration = new Duration(TimeSpan.FromSeconds(0.2));

    readonly DoubleAnimationUsingKeyFrames _dakLeave = new() {
        Duration = AnimDuration,
        KeyFrames = [
            new EasingDoubleKeyFrame{
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)),
                Value = 40,
                EasingFunction = new SineEase()
            }
        ]
    };

    readonly DoubleAnimationUsingKeyFrames _dakSwap = new() {
        Duration = new Duration(TimeSpan.FromSeconds(0)),
        KeyFrames = [
            new EasingDoubleKeyFrame{
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                Value = 0,
                EasingFunction = new SineEase()
            }
        ]
    };
    
    readonly DoubleAnimationUsingKeyFrames _dakEnter = new() {
        Duration = AnimDuration,
        KeyFrames = [
            new EasingDoubleKeyFrame{
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)),
                Value = 0,
                EasingFunction = new SineEase()
            }
        ]
    };

    double SineEase(double tick, double scale = 1, double multiplier = 1) {
        return double.Sin(scale * double.Pi * tick) * multiplier;
    }

    double TripleEase(double tick,double scale = 1,double multiplier = 1) {
        return multiplier * (double.Pow(tick * scale,3));
    }
    
    void SwapAnim(Label target,TranslateTransform targetTransform, string newContent) {
        for (int  x = 0;  x <= 40;  x++) {
            int x1 = x;
            this.Invoke(() => {
                targetTransform.Y = 40 * TripleEase(x1/40.0,1);
                target.Opacity = (40 - x1) / 40.0;
            });
            //AccurateWait(0.1);
            Thread.Sleep(1);
        }
        this.Invoke(() => {
            target.Content = newContent;
        });
        for (int  x = 0;  x <= 40;  x++) {
            int x1 = x;
            this.Invoke(() => {
                targetTransform.Y = -40 * TripleEase(1 - x1/40.0,1);
                target.Opacity =1 - (40 - x1) / 40.0;
            });
            //AccurateWait(0.1);
            Thread.Sleep(1);
        }
    }

    void AccurateWait(double ms) {
        System.Diagnostics.Stopwatch stopTime = new System.Diagnostics.Stopwatch();
        stopTime.Start();
        while (stopTime.Elapsed.TotalMilliseconds < ms) { }
        stopTime.Stop();
    }
    
    void BetterCountdown_OnLoaded(object sender, RoutedEventArgs e) {
        OnLoad();
    }
}