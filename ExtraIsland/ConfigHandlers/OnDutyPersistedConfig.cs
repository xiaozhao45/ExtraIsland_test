using ClassIsland.Core;
using ClassIsland.Core.Abstractions;

namespace ExtraIsland.ConfigHandlers;

public class OnDutyPersistedConfig {
    public OnDutyPersistedConfig() {
        
    }
}

public class OnDutyPersistedConfigData {
    public List<string> Peoples { get; set; } = [];
}