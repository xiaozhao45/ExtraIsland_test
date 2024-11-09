namespace ExtraIsland.Components;

// ReSharper disable once ClassNeverInstantiated.Global
public class FluentClockConfig {

    bool? _isAccurate;
    public bool? IsAccurate {
        get => _isAccurate;
        set {
            if (_isAccurate != value) {
                _isAccurate = value;
                OnAccurateChanged?.Invoke();
            } else {
                _isAccurate = value;
            }
        }
    }
    public event Action? OnAccurateChanged;
    
    public bool? IsFocusedMode { get; set; }

    bool? _isSecondsSmall;
    public bool? IsSecondsSmall {
        get => _isSecondsSmall;
        set {
            if (_isSecondsSmall != value) {
                _isSecondsSmall = value;
                OnSecondsSmallChanged?.Invoke();
            } else {
                _isSecondsSmall = value;
            }
        }
    }
    public event Action? OnSecondsSmallChanged;
}