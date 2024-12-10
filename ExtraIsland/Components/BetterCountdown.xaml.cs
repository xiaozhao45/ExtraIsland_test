using System.Windows;
using System.Windows.Data;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ExtraIsland.StutterEngine;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "759FFA33-309F-6494-3903-E0693036197E",
    "更好的倒计时",
    PackIconKind.CalendarOutline,
    "提供更高级的功能与动画支持"
)]
public partial class BetterCountdown {
    public BetterCountdown(ILessonsService lessonsService, IExactTimeService exactTimeService) {
        ExactTimeService = exactTimeService;
        LessonsService = lessonsService;
        InitializeComponent();
        _dyAnimator = new Animator.ClockTransformControlAnimator(LDays);
        _hrAnimator = new Animator.ClockTransformControlAnimator(LHours);
        _mnAnimator = new Animator.ClockTransformControlAnimator(LMins);
        _scAnimator = new Animator.ClockTransformControlAnimator(LSecs);
    }
    
    IExactTimeService ExactTimeService { get; }
    ILessonsService LessonsService { get; }

    readonly Animator.ClockTransformControlAnimator _dyAnimator;
    readonly Animator.ClockTransformControlAnimator _hrAnimator;
    readonly Animator.ClockTransformControlAnimator _mnAnimator;
    readonly Animator.ClockTransformControlAnimator _scAnimator;
    
    void OnLoad() {
        UpdateAccuracy();
        OnTimeChanged += DetectEvent;
        Settings.OnAccuracyChanged += UpdateAccuracy;
        LessonsService.PostMainTimerTicked += UpdateTime;
    }

    bool isAccurateChanged;
    void UpdateAccuracy() {
        isAccurateChanged = true;
        LSecs.Visibility = BoolToCollapsedVisible((int)Settings.Accuracy >= 3);
        SSec.Visibility = BoolToCollapsedVisible((int)Settings.Accuracy >= 3);
        LMins.Visibility = BoolToCollapsedVisible((int)Settings.Accuracy >= 2);
        SMin.Visibility = BoolToCollapsedVisible((int)Settings.Accuracy >= 2);
        LHours.Visibility = BoolToCollapsedVisible((int)Settings.Accuracy >= 1);
        SHour.Visibility = BoolToCollapsedVisible((int)Settings.Accuracy >= 1);
    }

    static Visibility BoolToCollapsedVisible(bool isVisible) {
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    static TimeSpan DiffDays(DateTime startTime, DateTime endTime) {
        TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return daysSpan;
    }
    
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
    
    void UpdateTime(object? sender,EventArgs eventArgs) {
        Now = !Settings.IsSystemTime ? 
            ExactTimeService.GetCurrentLocalDateTime()
            : DateTime.Now;
    }
    
    string _days = string.Empty;
    string _hours = string.Empty;
    string _minutes = string.Empty;
    string _seconds = string.Empty;
    void DetectEvent() {
            TimeSpan span = DiffDays(Now, Convert.ToDateTime(Settings.TargetDate));
            if (_days != span.Days.ToString()|isAccurateChanged) {
                int dayI = span.Days;
                _days = (int)Settings.Accuracy == 0 ? (dayI + 1).ToString() : dayI.ToString();
                _dyAnimator.TargetContent = _days;
            }
            if (_hours != span.Hours.ToString()|isAccurateChanged) {
                int hourI = span.Hours;
                _hours = (int)Settings.Accuracy == 1 ? (hourI + 1).ToString() : hourI.ToString();
                _hrAnimator.TargetContent = _hours;
            }
            if (_minutes != span.Minutes.ToString()|isAccurateChanged) {
                int minuteI = span.Minutes;
                _minutes = (int)Settings.Accuracy == 2 ? (minuteI + 1).ToString() : minuteI.ToString();
                string m = _minutes;
                if (m.Length == 1) {
                    m = "0" + m;
                }
                _mnAnimator.TargetContent = m;
            }
            // ReSharper disable once InvertIf
            if (_seconds != span.Seconds.ToString()|isAccurateChanged) {
                _seconds = span.Seconds.ToString();
                string s = _seconds;
                if (s.Length == 1) {
                    s = "0" + s;
                }
                _scAnimator.TargetContent = s;
                isAccurateChanged = false;
            }
    }
    
    void BetterCountdown_OnLoaded(object sender, RoutedEventArgs e) {
        this.BeginInvoke(OnLoad);
    }
}