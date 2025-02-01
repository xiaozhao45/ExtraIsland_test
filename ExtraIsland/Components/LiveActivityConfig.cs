namespace ExtraIsland.Components;

// ReSharper disable once ClassNeverInstantiated.Global
public class LiveActivityConfig {
    public event Action? OnMarginChanged;
    bool _isLeftNegativeMargin;
    public bool IsLeftNegativeMargin {
        get => _isLeftNegativeMargin; 
        set { 
            if (_isLeftNegativeMargin == value) return;
            _isLeftNegativeMargin = value;
            OnMarginChanged?.Invoke();
        }
    }
    bool _isRightNegativeMargin;
    public bool IsRightNegativeMargin {
        get => _isRightNegativeMargin; 
        set {
            if (_isRightNegativeMargin == value) return;
            _isRightNegativeMargin = value;
            OnMarginChanged?.Invoke();
        }
    }
    
    public bool IsAnimationEnabled { get; set; } = true;
}