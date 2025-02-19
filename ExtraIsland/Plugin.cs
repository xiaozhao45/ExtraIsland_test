using System.Reflection;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ExtraIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExtraIsland
{/*
            ___________          __                   .___         .__                       .___
            \_   _____/___  ____/  |_ _______ _____   |   |  ______|  |  _____     ____    __| _/
             |    __)_ \  \/  /\   __\\_  __ \\__  \  |   | /  ___/|  |  \__  \   /    \  / __ |
             |        \ >    <  |  |   |  | \/ / __ \_|   | \___ \ |  |__ / __ \_|   |  \/ /_/ |
            /_______  //__/\_ \ |__|   |__|   (____  /|___|/____  >|____/(____  /|___|  /\____ |
                    \/       \/                    \/           \/            \/      \/      \/
*/
    [PluginEntrance]
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Plugin : PluginBase
    {
        public override void Initialize(HostBuilderContext context, IServiceCollection services) {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[ExIsLand][Splash]-------------------------------------------------------------------\r\n" 
                             + GlobalConstants.Assets.AsciiLogo
                             + "\r\n Copyright (C) 2024-2025 LiPolymer \r\n Licensed under GNU AGPLv3. \r\n" 
                             + "[ExIsLand][EarlyLoad]正在初始化...-------------------------------------------------");
            Console.ForegroundColor = defaultColor;
            Console.WriteLine("[ExIsLand][EarlyLoad]正在注册...");
            //Initialize GlobalConstants/ConfigHandlers
            GlobalConstants.PluginConfigFolder = PluginConfigFolder;
            GlobalConstants.Handlers.OnDuty = new ConfigHandlers.OnDutyPersistedConfigHandler();
            GlobalConstants.Handlers.MainConfig = new ConfigHandlers.MainConfigHandler();
            //Registering Services
            services.AddHostedService<ServicesFetcherService>();
            services.AddComponent<Components.BetterCountdown,Components.BetterCountdownSettings>();
            services.AddComponent<Components.FluentClock,Components.FluentClockSettings>();
            services.AddComponent<Components.Rhesis,Components.RhesisSettings>();
            services.AddComponent<Components.OnDuty,Components.OnDutySettings>();
            services.AddComponent<Components.LiveActivity,Components.LiveActivitySettings>();
            services.AddSettingsPage<SettingsPages.MainSettingsPage>();
            services.AddSettingsPage<SettingsPages.DutySettingsPage>();
            services.AddSettingsPage<SettingsPages.TinyFeaturesSettingsPage>();
            if (GlobalConstants.Handlers.MainConfig.Data.IsLifeModeActivated) {
                services.AddComponent<LifeMode.Components.Sleepy,LifeMode.Components.SleepySettings>();
            }
            #if DEBUG
                services.AddSettingsPage<SettingsPages.DebugSettingsPage>();
                services.AddComponent<Components.DebugLyricsHandler>();
                services.AddComponent<Components.DebugSubLyricsHandler>();
            #endif
            Console.WriteLine("[ExIsLand][EarlyLoad]完成");
            //OnInitialization triggers
            Console.WriteLine("[ExIsLand][EarlyLoadTrigger]触发加载事件...");
            TinyFeatures.JuniorGuide.Trigger();
            Console.WriteLine("[ExIsLand][EarlyLoadTrigger]完成!");
            Console.WriteLine("[ExIsLand][EarlyLoad]等待服务主机启动...");
        }
    }
}
