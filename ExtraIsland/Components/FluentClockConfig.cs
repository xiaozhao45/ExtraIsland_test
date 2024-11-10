namespace ExtraIsland.Components;

// ReSharper disable once ClassNeverInstantiated.Global
public class FluentClockConfig {

    bool? _isAccurate;
    public bool? IsAccurate {
        get => _isAccurate;
        set {
            if (_isAccurate == value) return;
            _isAccurate = value; 
            OnAccurateChanged?.Invoke();
        }
    }
    public event Action? OnAccurateChanged;
    
    public bool? IsFocusedMode { get; set; }

    bool? _isSecondsSmall;
    public bool? IsSecondsSmall {
        get => _isSecondsSmall;
        set {
            if (_isSecondsSmall == value) return;
            _isSecondsSmall = value;
            OnSecondsSmallChanged?.Invoke();
        }
    }
    public event Action? OnSecondsSmallChanged;

    public bool? IsSystemTime { get; set; }
}