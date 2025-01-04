using ExtraIsland.ConfigHandlers;

namespace ExtraIsland.Shared;

public static class GlobalConstants {
    public static string? PluginConfigFolder { get; set; }

    public static class Handlers {
        public static OnDutyPersistedConfigHandler? OnDuty { get; set; }
    }
}