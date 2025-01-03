using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using ClassIsland.Shared.Helpers;
using ExtraIsland.Shared;

namespace ExtraIsland.ConfigHandlers;

public class OnDutyPersistedConfig {
    public OnDutyPersistedConfig() {
        Data = new OnDutyPersistedConfigData();
        if (!File.Exists(Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/OnDuty.json"))) {
            if (!Directory.Exists(Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/"))) {
                Directory.CreateDirectory(Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/"));
            }
            ConfigureFileHelper.SaveConfig<OnDutyPersistedConfigData>(
                Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/OnDuty.json"),
                Data);
        }
        try {
            Data = ConfigureFileHelper.LoadConfig<OnDutyPersistedConfigData>(
                Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/OnDuty.json"));
        }
        catch {
            File.Delete(Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/OnDuty.json"));
            ConfigureFileHelper.SaveConfig<OnDutyPersistedConfigData>(
                Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/OnDuty.json"),
                Data);
        }
        PeopleOnDuty = Data.GetWhoOnDuty();
        Data.PropertyChanged += Save;
    }

    public void Save() {
        ConfigureFileHelper.SaveConfig<OnDutyPersistedConfigData>(
            Path.Combine(GlobalConstants.PluginConfigFolder!,"Persisted/OnDuty.json"),
            Data);
        UpdateOnDuty();
    }

    public void UpdateOnDuty() {
        PeopleOnDuty = Data.GetWhoOnDuty();
        OnDutyUpdated?.Invoke();
    }

    public OnDutyPersistedConfigData.PeopleItem PeopleOnDuty { get; set; }

    public OnDutyPersistedConfigData Data { get; set; }
    
    public event Action? OnDutyUpdated;
}

public class OnDutyPersistedConfigData {

    public event Action? PropertyChanged;

    ObservableCollection<PeopleItem> _peoples = [
        new PeopleItem { Index = 0,Name = "张三" },
        new PeopleItem { Index = 1,Name = "李四" }
    ];

    public ObservableCollection<PeopleItem> Peoples {
        get => _peoples;
        set {
            _peoples = value;
            PropertyChanged?.Invoke();
        }
    }

    DateTime _lastUpdate = DateTime.Now;
    public DateTime LastUpdate {
        get => _lastUpdate;
        set {
            _lastUpdate = value;
            PropertyChanged?.Invoke();
        }
    }

    bool? _doubleState;
    public bool? DoubleState {
        get => _doubleState;
        set {
            _doubleState = value;
            PropertyChanged?.Invoke();
        }
    }

    int _currentPeopleIndex;
    public int CurrentPeopleIndex {
        get => _currentPeopleIndex;
        set {
            _currentPeopleIndex = value;
            PropertyChanged?.Invoke();
        }
    }

    bool _isCycled = true;
    public bool IsCycled {
        get => _isCycled;
        set {
            _isCycled = value;
            PropertyChanged?.Invoke();
        }
    }

    DutyStateData _dutyState = DutyStateData.Single;
    public DutyStateData DutyState {
        get => _dutyState;
        set {
            _dutyState = value;
            PropertyChanged?.Invoke();
        }
    }

    public enum DutyStateData {
        [Description("单人值日")] Single,
        [Description("双人值日")] Double,
        [Description("内/外 双人轮换值日")] InOut
    }

    TimeSpan _dutyChangeDuration;
    public TimeSpan DutyChangeDuration {
        get => _dutyChangeDuration;
        set {
            _dutyChangeDuration = value;
            PropertyChanged?.Invoke();
        }
    }

    [JsonIgnore]
    public double DutyChangeDurationDays {
        get => DutyChangeDuration.TotalDays;
        set => DutyChangeDuration = TimeSpan.FromDays(value);
    }

    public PeopleItem GetWhoOnDuty() {
        PeopleItem? item = Peoples.FirstOrDefault(p => p.Index == CurrentPeopleIndex);
        item ??= new PeopleItem {
            Index = CurrentPeopleIndex,
            Name = "没有值日生"
        };
        return item;
    }

    public class PeopleItem {
        public string Name { get; set; } = string.Empty;
        public int Index { get; set; }
    }
}