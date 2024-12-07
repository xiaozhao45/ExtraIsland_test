using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.CommonDialog;
using ExtraIsland.StutterEngine;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "759FFA33-309F-6494-3903-E0693036197E",
    "(未完成)更好的倒计时",
    PackIconKind.CalendarOutline,
    "提供更高级的功能"
)]
public partial class BetterCountdown : ComponentBase<BetterCountdownConfig> {
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
        Settings.TargetDate ??= DateTime.Now.ToString("s");
        Settings.IsSystemTime ??= false;
        Settings.Prefix ??= "";
        Settings.Suffix ??= "";
        OnTimeChanged += DetectEvent;
        LessonsService.PostMainTimerTicked += UpdateTime;
    }

    TimeSpan DiffDays(DateTime startTime, DateTime endTime) {
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
        Now = !Settings.IsSystemTime!.Value ? 
            ExactTimeService.GetCurrentLocalDateTime()
            : DateTime.Now;
    }
    
    string _days = string.Empty;
    string _hours = string.Empty;
    string _minutes = string.Empty;
    string _seconds = string.Empty;
    void DetectEvent() {
            TimeSpan span = DiffDays(DateTime.Now, Convert.ToDateTime(Settings.TargetDate));
            if (_days != span.Days.ToString()) {
                _days = span.Days.ToString();
                _dyAnimator.TargetContent = _days;
            }
            if (_hours != span.Hours.ToString()) {
                _hours = span.Hours.ToString();
                _hrAnimator.TargetContent = _hours;
            }
            if (_minutes != span.Minutes.ToString()) {
                _minutes = span.Minutes.ToString();
                string m = _minutes;
                if (m.Length == 1) {
                    m = "0" + m;
                }
                _mnAnimator.TargetContent = m;
            }
            if (_seconds != span.Seconds.ToString()) {
                _seconds = span.Seconds.ToString();
                var s = _seconds;
                if (s.Length == 1) {
                    s = "0" + s;
                }
                _scAnimator.TargetContent = s;
            }
    }
    
    void BetterCountdown_OnLoaded(object sender, RoutedEventArgs e) {
        this.BeginInvoke(OnLoad);
    }
    
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object? value, Type targetType, object? parameter,
            System.Globalization.CultureInfo culture) {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");
            return !(bool)value!;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter,
            System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }

        #endregion
    }
}