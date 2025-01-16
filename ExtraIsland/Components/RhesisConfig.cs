using System.Runtime.InteropServices.JavaScript;
using ExtraIsland.Shared;

namespace ExtraIsland.Components;

public class RhesisConfig {
    public RhesisDataSource DataSource { get; set; } = RhesisDataSource.All;
    
    public string HitokotoProp  { get; set; } = string.Empty;
    
    public DateTime LastUpdate { get; set; } = DateTime.Today;
    
    public TimeSpan UpdateTimeGap { get; set; } = TimeSpan.FromSeconds(30);

    public double UpdateTimeGapSeconds {
        get => UpdateTimeGap.TotalSeconds;
        set => UpdateTimeGap = TimeSpan.FromSeconds(value);
    }
    
    public string SainticProp  { get; set; } = string.Empty;
}