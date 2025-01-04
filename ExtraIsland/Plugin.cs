
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.CommonDialog;
using ClassIsland.Core.Extensions.Registry;
using ExtraIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExtraIsland
{
    [PluginEntrance]
    // ReSharper disable once UnusedType.Global
    public class Plugin : PluginBase
    {
        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            GlobalConstants.PluginConfigFolder = PluginConfigFolder;
            GlobalConstants.Handlers.OnDuty = new ConfigHandlers.OnDutyPersistedConfigHandler();
            //Registering Services
            services.AddComponent<Components.BetterCountdown,Components.BetterCountdownSettings>();
            services.AddComponent<Components.FluentClock,Components.FluentClockSettings>();
            services.AddComponent<Components.Rhesis>();
            services.AddComponent<Components.OnDuty>();
            services.AddSettingsPage<SettingsPages.DutySettingsPage>();
            #if DEBUG
                services.AddSettingsPage<SettingsPages.DebugSettingsPage>();
            #endif
        }
    }
}
