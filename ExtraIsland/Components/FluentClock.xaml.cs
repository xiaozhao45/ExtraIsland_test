using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ExtraIsland.StutterEngine;
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
    
    Animator.ClockTransformControlAnimator? _hourAnimator;
    Animator.ClockTransformControlAnimator? _minuAnimator;
    Animator.ClockTransformControlAnimator? _secoAnimator;
    
    void LoadedAction() {
        //Prepare local variable
        _hourAnimator ??= new Animator.ClockTransformControlAnimator(LHours);
        _minuAnimator ??= new Animator.ClockTransformControlAnimator(LMins);
        _secoAnimator ??= new Animator.ClockTransformControlAnimator(LSecs);
        
        var animator = new LegacyAnimator();
        
        string hours;
        string minutes;
        string seconds;

        bool sparkSeq = true;
        bool updLock = false;
        //Null check
        Settings.IsAccurate ??= true;
        Settings.IsFocusedMode ??= false;
        Settings.IsSecondsSmall ??= false;
        Settings.IsSystemTime ??= false;
        Settings.IsOClockEmp ??= true;
        Settings.UseCiFontSize ??= false;
        //Initialization
        CiFontChangedUpdater();
        AccurateModeUpdater();
        UpdateTime();
        SilentUpdater();
        FocusModeUpdater();
        //Register Events
        Settings.OnSecondsSmallChanged += SmallSecondsUpdater;
        Settings.OnAccurateChanged += AccurateModeUpdater;
        Settings.OnUseCiFontSizeChanged += CiFontChangedUpdater;
        Settings.OnUseCiFontSizeChanged += FocusModeUpdater;
        Settings.OnOClockEmpEnabled += () => {
            new Thread(ShowEmpEffect).Start();
        };
        LessonsService.PostMainTimerTicked += (_,_) => {
            UpdateTime();
        };
        OnTimeChanged += () => {
            if (updLock) return;
            updLock = true;
            this.BeginInvoke(MainUpdater);
        };
        return;
        void MainUpdater() {
            var handlingTime = Now;
            if (hours != Now.Hour.ToString()) {
                if (Settings.IsOClockEmp.Value & Now.Second == 0) {
                    this.BeginInvoke(()=> {
                        animator.OpacityEmphasize(EmpBack,3000);
                    });
                }
                hours = Now.Hour.ToString();
                var h = hours;
                _hourAnimator.TargetContent = h;
            }
            if (minutes != Now.Minute.ToString()) {
                minutes = Now.Minute.ToString();
                var m = minutes;
                if (m.Length == 1) {
                    m = "0" + m;
                }
                _minuAnimator.TargetContent = m;
            }
            if (seconds != Now.Second.ToString()) {
                seconds = Now.Second.ToString();
                if (Settings.IsAccurate.Value) {
                    var s = seconds;
                    if (s.Length == 1) {
                        s = "0" + s;
                    }
                    if (Settings.IsFocusedMode.Value) {
                        _secoAnimator.Update(s,false);
                    } else {
                        _secoAnimator.TargetContent = s;
                    }
                } else {
                    animator.Fade(SMins,sparkSeq);
                    sparkSeq = !sparkSeq;
                }
            }
            // Unlocker
            if (handlingTime == Now) {
                updLock = false;
            } else {
                MainUpdater();
            }
        }
        
        void SilentUpdater(){
            hours = Now.Hour.ToString();
            minutes = Now.Minute.ToString();
            seconds = Now.Second.ToString();
            this.Invoke(() => {
                LHours.Content = hours;
                LMins.Content = minutes;
                LSecs.Content = seconds;
            });
        }
    }

    void UpdateTime() {
        Now = !Settings.IsSystemTime!.Value ? 
            ExactTimeService.GetCurrentLocalDateTime()
            : DateTime.Now;
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

    void FocusModeUpdater() {
        _secoAnimator!.IsSwapAnimEnabled = !Settings.IsFocusedMode!.Value;
        _minuAnimator!.IsSwapAnimEnabled = !Settings.IsFocusedMode!.Value;
        _hourAnimator!.IsSwapAnimEnabled = !Settings.IsFocusedMode!.Value;
    }
    
    bool _firstLoad = true;
    void CiFontChangedUpdater() {
        if (Settings.UseCiFontSize!.Value) {
            if (!_firstLoad) {
                this.Invoke(InvalidateVisual);
            }
        } else {
            this.Invoke(() => {
                LHours.FontSize = 18;
                LMins.FontSize = 18;
            });
            SmallSecondsUpdater();
        }
        _firstLoad = false;
    }
    
    void SmallSecondsUpdater() {
        this.Invoke(() => {
            if (Settings.UseCiFontSize!.Value) return;
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
            EmpBack.Width = Settings.IsAccurate!.Value ? 95 : 60;
            BakT.X = Settings.IsAccurate!.Value ? 1 : 0;
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
        this.Invoke(LoadedAction);
    }
}