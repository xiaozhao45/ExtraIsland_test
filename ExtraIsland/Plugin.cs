
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
            GlobalConstants.Handlers.MainConfig = new ConfigHandlers.MainConfigHandler();
            //Registering Services
            services.AddComponent<Components.BetterCountdown,Components.BetterCountdownSettings>();
            services.AddComponent<Components.FluentClock,Components.FluentClockSettings>();
            services.AddComponent<Components.Rhesis,Components.RhesisSettings>();
            services.AddComponent<Components.OnDuty>();
            services.AddSettingsPage<SettingsPages.MainSettingsPage>();
            services.AddSettingsPage<SettingsPages.DutySettingsPage>();
            if (GlobalConstants.Handlers.MainConfig.Data.IsLifeModeActivated) {
                services.AddComponent<LifeMode.Components.Sleepy,LifeMode.Components.SleepySettings>();
            }
            #if DEBUG
                services.AddSettingsPage<SettingsPages.TinyFeaturesSettingsPage>();
                services.AddSettingsPage<SettingsPages.DebugSettingsPage>();
            #endif
            //OnInitialization triggers
            TinyFeatures.JuniorGuide.Trigger();
        }
    }
}
