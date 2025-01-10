using ExtraIsland.Shared;

namespace ExtraIsland.Components;

public class RhesisConfig {
    public RhesisDataSource DataSource { get; set; } = RhesisDataSource.All;
    
    public string HitokotoProp  { get; set; } = string.Empty;
    
    public string SainticProp  { get; set; } = string.Empty;
}