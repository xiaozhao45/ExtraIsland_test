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
public partial class FluentClock : ComponentBase<FluentClockConfig> {
    public FluentClock() {
        InitializeComponent();
    }

    void LoadCache() {
        try {
            _tripleEaseCache.Add(0,0.0);
            for (int x = 1; x <= 40; x++) {
                _tripleEaseCache.Add(x,40 * TripleEase(x / 40.0,1));
            }
            for (int x = 1; x <= 40; x++) {
                _tripleEaseCache.Add(-x,-40 * TripleEase(1 - x / 40.0,1));
            }
        }
        catch {
            // ignored
        }
    }
    
    void DetectCycle() {
        LoadCache();
        string hours = string.Empty;
        string minutes = string.Empty;
        string seconds = string.Empty;
        bool sparkSeq = true;
        Settings.IsAccurate ??= true;
        Settings.IsFocusedMode ??= false;
        Settings.IsSecondsSmall ??= false;
        while (true) {
            //Initialization
            if (Settings.IsAccurate.Value) {
                this.Invoke(() => {
                    LSecs.Visibility = Visibility.Visible;
                    SSecs.Visibility = Visibility.Visible; 
                    SMins.Opacity = 1;
                });
            } else {
                this.Invoke(() => {
                    LSecs.Visibility = Visibility.Collapsed;
                    SSecs.Visibility = Visibility.Collapsed;
                });
            }
            //Animation
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
            //While Seconds change:
            if (seconds != now.Second.ToString()) {
                seconds = now.Second.ToString();
                if (Settings.IsAccurate.Value) {
                    //PRE:
                    this.Invoke(() => {
                        LSecs.FontSize = Settings.IsSecondsSmall.Value ? 14 : 18;
                        LSecs.Padding = Settings.IsSecondsSmall.Value ?
                            new Thickness(0,3,0,0)
                            : new Thickness(0);
                        SSecs.Padding = Settings.IsSecondsSmall.Value ?
                            new Thickness(0,2,0,0)
                            : new Thickness(0,0,0,3);
                        SSecs.FontSize = Settings.IsSecondsSmall.Value ? 16 : 20;
                    });
                    //Updater
                    var s = seconds;
                    if (s.Length == 1) {
                        s = "0" + s;
                    }
                    if (Settings.IsFocusedMode.Value) {
                        // Second Sparkling
                        for (int  x = 0;  x <= 40;  x++) {
                            int x1 = x;
                            this.Invoke(() => {
                                LSecs.Opacity = (40 - x1) / 40.0 * 0.7 + 0.3;
                            });
                            Thread.Sleep(1);
                        }
                        this.Invoke(() => {
                            LSecs.Content = s;
                        });
                        for (int x = 0; x <= 40; x++) {
                            int x1 = x;
                            this.Invoke(() => {
                                LSecs.Opacity = (1 - (40 - x1) / 40.0) * 0.7 + 0.3;
                            });
                            Thread.Sleep(1);
                        }
                    } else {
                        SwapAnim(LSecs,STt,s);
                    }
                } else {
                    //Separator Sparkling
                    if (sparkSeq) {
                        for (int  x = 0;  x <= 40;  x++) {
                            int x1 = x;
                            this.Invoke(() => {
                                SMins.Opacity = (40 - x1) / 40.0;
                            });
                            Thread.Sleep(1);
                        }
                        sparkSeq = false;
                    } else {
                        for (int x = 0; x <= 40; x++) {
                            int x1 = x;
                            this.Invoke(() => {
                                SMins.Opacity = 1 - (40 - x1) / 40.0;
                            });
                            Thread.Sleep(1);
                        }
                        sparkSeq = true;
                    }
                }
            }
            Thread.Sleep(50);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    readonly Dictionary<int,double> _tripleEaseCache = new Dictionary<int,double>();
    
    double TripleEase(double tick,double scale = 1,double multiplier = 1) {
        return multiplier * (double.Pow(tick * scale,3));
    }
    
    void SwapAnim(Label target,TranslateTransform targetTransform, string newContent) {
        for (int  x = 0;  x <= 40;  x++) {
            int x1 = x;
            this.Invoke(() => {
                targetTransform.Y = _tripleEaseCache[x1];
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
                targetTransform.Y = _tripleEaseCache[-x1];
                target.Opacity =1 - (40 - x1) / 40.0;
            });
            Thread.Sleep(1);
        }
    }
    
    void FluentClock_OnLoaded(object sender,RoutedEventArgs e) {
        new Thread(DetectCycle).Start();
    }
}