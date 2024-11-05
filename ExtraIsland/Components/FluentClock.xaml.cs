using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "0EA67B3B-E4CB-56C1-AFDC-F3EA7F38924D",
    "流畅时钟",
    PackIconKind.ClockDigital,
    "拥有动画支持"
)]
public partial class FluentClock : ComponentBase {
    public FluentClock() {
        InitializeComponent();
    }

    void DetectCycle() {
        string days = string.Empty;
        string hours = string.Empty;
        string minutes = string.Empty;
        string seconds = string.Empty;
        while (true) {
            var now = DateTime.Now;
            if (hours != now.Hour.ToString()) {
                hours = now.Hour.ToString();
                var h = hours;
                SwapAnim(LHours,HTt,h);
            }
            if (minutes != now.Minute.ToString()) {
                minutes = now.Minute.ToString();
                var m = minutes;
                if (m.Length == 1) {
                    m = "0" + m;
                }
                SwapAnim(LMins,MTt,m);
            }
            if (seconds != now.Second.ToString()) {
                seconds = now.Second.ToString();
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
            Thread.Sleep(1);
        }
    }
    
    void FluentClock_OnLoaded(object sender,RoutedEventArgs e) {
        new Thread(DetectCycle).Start();
    }
}