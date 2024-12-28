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
        _hourAnimator = new Animator.ClockTransformControlAnimator(LHours);
        _minuAnimator = new Animator.ClockTransformControlAnimator(LMins);
        _secoAnimator = new Animator.ClockTransformControlAnimator(LSecs);
        _separatorAnimator = new Animator.SeparatorControlAnimator(SMins);
        _emphasizeAnimator = new Animator.EmphasizeUiElementAnimator(EmpBack);
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

    readonly Animator.ClockTransformControlAnimator _hourAnimator;
    readonly Animator.ClockTransformControlAnimator _minuAnimator;
    readonly Animator.ClockTransformControlAnimator _secoAnimator;
    readonly Animator.SeparatorControlAnimator _separatorAnimator;
    readonly Animator.EmphasizeUiElementAnimator _emphasizeAnimator;
    
    void LoadedAction() {
        //Prepare local variable
        
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
        //Initialization
        AccurateModeUpdater();
        UpdateTime();
        SilentUpdater();
        if (Settings.IsSecondsSmall.Value) {
            SmallSecondsUpdater();
        }
        //Register Events
        Settings.OnSecondsSmallChanged += SmallSecondsUpdater;
        Settings.OnAccurateChanged += AccurateModeUpdater;
        Settings.OnOClockEmpEnabled += ShowEmphasise;
        LessonsService.PostMainTimerTicked += UpdateTime;
        OnTimeChanged += () => {
            if (updLock) return;
            updLock = true;
            new Thread(MainUpdater).Start();
        };
        return;
        void MainUpdater() {
            var handlingTime = Now;
            if (hours != Now.Hour.ToString()) {
                if (Settings.IsOClockEmp.Value & Now.Second == 0) {
                    this.Invoke(()=> {
                        _emphasizeAnimator.Update();
                    });
                }
                hours = Now.Hour.ToString();
                var h = hours;
                this.Invoke(() => {
                    _hourAnimator.TargetContent = h;
                });
            }
            if (minutes != Now.Minute.ToString()) {
                minutes = Now.Minute.ToString();
                var m = minutes;
                if (m.Length == 1) {
                    m = "0" + m;
                }
                this.Invoke(() => {
                    _minuAnimator.TargetContent = m;
                });
            }
            if (seconds != Now.Second.ToString()) {
                seconds = Now.Second.ToString();
                if (Settings.IsAccurate.Value) {
                    this.Invoke(() => {
                        SMins.Opacity = 1;
                    });
                    string s = seconds;
                    if (s.Length == 1) {
                        s = "0" + s;
                    }
                    this.Invoke(() => {
                        _secoAnimator.Update(s,!Settings.IsFocusedMode.Value);  
                    });
                } else {
                    bool seq = sparkSeq;
                    this.Invoke(() => {
                        _separatorAnimator.Update(seq);
                    });
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
            if (minutes.Length == 1) {
                minutes = "0" + minutes;
            }
            seconds = Now.Second.ToString();
            if (seconds.Length == 1) {
                seconds = "0" + seconds;
            }
            this.Invoke(() => {
                LHours.Content = hours;
                LMins.Content = minutes;
                LSecs.Content = seconds;
            });
        }
    }
    
    void ShowEmphasise() {
        this.BeginInvoke(() => {
            _emphasizeAnimator?.Update();
        });
    }
    
    void UpdateTime(object? sender,EventArgs e) {
        UpdateTime();
    }

    void UpdateTime() {
        Now = !Settings.IsSystemTime!.Value ? 
            ExactTimeService.GetCurrentLocalDateTime()
            : DateTime.Now;
    }
    
    void SmallSecondsUpdater() {
        this.Invoke(() => {
            LSecs.FontSize = Settings.IsSecondsSmall!.Value ? 14 : LHours.FontSize;
            LSecs.Padding = Settings.IsSecondsSmall.Value ?
                new Thickness(0,3,0,0)
                : new Thickness(0);
            SSecs.Padding = Settings.IsSecondsSmall.Value ?
                new Thickness(0,2,0,0)
                : new Thickness(0,0,0,3);
            SSecs.FontSize = Settings.IsSecondsSmall.Value ? 16 : SMins.FontSize;
            TSecs.X = Settings.IsSecondsSmall.Value ? 2 : 0;
        });
    }

    void AccurateModeUpdater() {
        this.Invoke(() => {
            SMins.Opacity = 1;
            LSecs.Visibility = Settings.IsAccurate!.Value ? Visibility.Visible : Visibility.Collapsed;
            SSecs.Visibility = Settings.IsAccurate!.Value ? Visibility.Visible : Visibility.Collapsed;
            EmpBack.Width = Settings.IsAccurate!.Value ? 95 : 60;
        });
    }
    
    void FluentClock_OnLoaded(object sender,RoutedEventArgs e) {
        this.Invoke(LoadedAction);
    }
    void FluentClock_OnUnloaded(object sender,RoutedEventArgs e) {
        Settings.OnAccurateChanged -= AccurateModeUpdater;
        Settings.OnSecondsSmallChanged -= SmallSecondsUpdater;
        Settings.OnOClockEmpEnabled -= ShowEmphasise;
        LessonsService.PostMainTimerTicked -= UpdateTime;
    }
}