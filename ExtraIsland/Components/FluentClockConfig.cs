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
    
    bool? _isFocusedMode;
    public bool? IsFocusedMode {
        get => _isFocusedMode;
        set {
            if (_isFocusedMode == value) return;
            _isFocusedMode = value;
            OnFocusedModeChanged?.Invoke();         
        }
    }
    
    public event Action? OnFocusedModeChanged;

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
    
    bool? _isOClockEmp;

    public bool? IsOClockEmp {
        get => _isOClockEmp;
        set {
            _isOClockEmp = value;
            if (_isOClockEmp is true) {
                OnOClockEmpEnabled?.Invoke();
            }
        }
    }
    
    public event Action? OnOClockEmpEnabled;

    bool? _useCiFontSize;
    public bool? UseCiFontSize {
        get => _useCiFontSize;
        set {
            if (_useCiFontSize == value) return;
            _useCiFontSize = value;
            OnUseCiFontSizeChanged?.Invoke();
        }
    }
    public event Action? OnUseCiFontSizeChanged;
}