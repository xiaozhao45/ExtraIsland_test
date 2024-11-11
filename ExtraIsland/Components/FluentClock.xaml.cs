using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClassIsland.Core.Abstractions.Services;
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
public partial class FluentClock {
    public FluentClock(ILessonsService lessonsService, IExactTimeService exactTimeService) {
        ExactTimeService = exactTimeService;
        LessonsService = lessonsService;
        InitializeComponent();
    }

    IExactTimeService ExactTimeService { get; }
    ILessonsService LessonsService { get; }

    DateTime _nowTime;
    DateTime Now {
        get => _nowTime;
        set {
            if (_nowTime == value) return;
            _nowTime = value;
            OnTimeChanged?.Invoke();
        }   
    }
    event Action? OnTimeChanged;
    
    Dictionary<int,double> _tripleEaseCache = new Dictionary<int,double>();
    static double TripleEase(double tick,double scale = 1,double multiplier = 1) {
        return multiplier * (double.Pow(tick * scale,3));
    }
    void LoadCache() {
        try {
            _tripleEaseCache = new Dictionary<int,double> { { 0,0.0 } };
            for (int x = 1; x <= 40; x++) {
                _tripleEaseCache.Add(x,40 * TripleEase(x / 40.0));
            }
            for (int x = 1; x <= 40; x++) {
                _tripleEaseCache.Add(-x,-40 * TripleEase(1 - x / 40.0));
            }
        }
        catch {
            // ignored
        }
    }
    
    void LoadedAction() {
        //Prepare local variable
        LoadCache();
        
        string hours = string.Empty;
        string minutes = string.Empty;
        string seconds = string.Empty;
        bool isFirstRun = true;
        
        bool sparkSeq = true;
        bool updLock = false;
        //Null check
        Settings.IsAccurate ??= true;
        Settings.IsFocusedMode ??= false;
        Settings.IsSecondsSmall ??= false;
        Settings.IsSystemTime ??= false;
        Settings.IsOClockEmp ??= true;
        //Register Events
        Settings.OnSecondsSmallChanged += SmallSecondsUpdater;
        Settings.OnAccurateChanged += AccurateModeUpdater;
        Settings.OnOClockEmpEnabled += () => {
            new Thread(ShowEmpEffect).Start();
        };
        LessonsService.PostMainTimerTicked += (_,_) => {
            Now = !Settings.IsSystemTime!.Value ? 
                ExactTimeService.GetCurrentLocalDateTime()
                : DateTime.Now;
        };
        OnTimeChanged += () => {
            if (updLock) return;
            updLock = true;
            new Thread(MainUpdater).Start();
        };
        //Initialization
        SmallSecondsUpdater();
        AccurateModeUpdater();
        return;
        void MainUpdater() {
            var handlingTime = Now;
            if (hours != Now.Hour.ToString()) {
                if (Settings.IsOClockEmp.Value & Now.Second == 0) {
                    new Thread(ShowEmpEffect).Start();
                }
                hours = Now.Hour.ToString();
                var h = hours;
                SwapAnim(LHours,HTt,h);
            }
            if (minutes != Now.Minute.ToString()) {
                minutes = Now.Minute.ToString();
                var m = minutes;
                if (m.Length == 1) {
                    m = "0" + m;
                }
                SwapAnim(LMins,MTt,m);
            }
            if (seconds != Now.Second.ToString()) {
                seconds = Now.Second.ToString();
                if (Settings.IsAccurate.Value) {
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
            // Unlocker
            isFirstRun = false;
            if (handlingTime == Now) {
                updLock = false;
            } else {
                MainUpdater();
            }
        }
    }

    void ShowEmpEffect() {
        for (int x = 0; x <= 40; x++) {
            int x1 = x;
            this.Invoke(() => {
                EmpBack.Opacity = (1 - (40 - x1) / 40.0);
            });
            Thread.Sleep(1);
        }
        Thread.Sleep(3000);
        for (int  x = 0;  x <= 40;  x++) {
            int x1 = x;
            this.Invoke(() => {
                EmpBack.Opacity = (40 - x1) / 40.0;
            });
            Thread.Sleep(1);
        }
    }
    
    void SmallSecondsUpdater() {
        this.Invoke(() => {
            LSecs.FontSize = Settings.IsSecondsSmall!.Value ? 14 : 18;
            LSecs.Padding = Settings.IsSecondsSmall.Value ?
                new Thickness(0,3,0,0)
                : new Thickness(0);
            SSecs.Padding = Settings.IsSecondsSmall.Value ?
                new Thickness(0,2,0,0)
                : new Thickness(0,0,0,3);
            SSecs.FontSize = Settings.IsSecondsSmall.Value ? 16 : 20;
        });
    }

    void AccurateModeUpdater() {
        this.Invoke(() => {
            SMins.Opacity = 1;
            LSecs.Visibility = Settings.IsAccurate!.Value ? Visibility.Visible : Visibility.Collapsed;
            SSecs.Visibility = Settings.IsAccurate!.Value ? Visibility.Visible : Visibility.Collapsed;
        });

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
        new Thread(LoadedAction).Start();
    }
}