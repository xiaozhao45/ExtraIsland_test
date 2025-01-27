namespace ExtraIsland.LifeMode.Components;

public class SleepyConfig {
    public string ApiUrl { get; set; } = "https://sleepy.developers.classisland.tech/query";
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.Zero;

    public int UpdateIntervalSeconds {
        get => (int)UpdateInterval.TotalSeconds;
        set => UpdateInterval = TimeSpan.FromSeconds(value);
    }
    
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}