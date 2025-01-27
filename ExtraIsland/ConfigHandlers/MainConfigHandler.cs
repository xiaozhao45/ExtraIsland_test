using System.Data.Common;
using System.IO;
using System.Windows.Forms;
using ClassIsland.Shared.Helpers;
using ExtraIsland.Shared;

namespace ExtraIsland.ConfigHandlers;

public class MainConfigHandler {
    public MainConfigData Data { get; set; }
    
    public MainConfigHandler() {
        Data = new MainConfigData();
        if (!File.Exists(Path.Combine(GlobalConstants.PluginConfigFolder!,"Main.json"))) {
            ConfigureFileHelper.SaveConfig<MainConfigData>(
                Path.Combine(GlobalConstants.PluginConfigFolder!,"Main.json"),
                Data);
        }
        try {
            Data = ConfigureFileHelper.LoadConfig<MainConfigData>(
                Path.Combine(GlobalConstants.PluginConfigFolder!,"Main.json"));
        }
        catch {
            File.Delete(Path.Combine(GlobalConstants.PluginConfigFolder!,"Main.json"));
            ConfigureFileHelper.SaveConfig<MainConfigData>(
                Path.Combine(GlobalConstants.PluginConfigFolder!,"Main.json"),
                Data);
        }
        Data.TinyFeatures.JuniorGuide.PropertyChanged += Save;
        Data.PropertyChanged += Save;
    }

    void Save() {
        ConfigureFileHelper.SaveConfig<MainConfigData>(
            Path.Combine(GlobalConstants.PluginConfigFolder!,"Main.json"),
            Data);
    }
}

public class MainConfigData { 
    public event Action? PropertyChanged;
    bool _isLifeModeActivated;
    public bool IsLifeModeActivated {
        get => _isLifeModeActivated;
        set {
            _isLifeModeActivated = value;
            PropertyChanged?.Invoke();
        }
    }
    public TinyFeaturesConfig TinyFeatures { get; set; } = new TinyFeaturesConfig();
    public class TinyFeaturesConfig {
        public JuniorGuideConfig JuniorGuide { get; set; } = new JuniorGuideConfig();
        public class JuniorGuideConfig {
            public event Action? PropertyChanged;
            bool _enabled;
            public bool Enabled {
                get => _enabled;
                set {
                    _enabled = value;
                    PropertyChanged?.Invoke();
                }
            }
            string _header = "欢迎 · 电子白板使用导引";
            public string Header {
                get => _header;
                set {
                    _header = value;
                    PropertyChanged?.Invoke();
                }
            }
            int _keepTime = 60000;
            public int KeepTime {
                get => _keepTime;
                set {
                    _keepTime = value;
                    PropertyChanged?.Invoke();
                }
            }
            string _content = "# 一体机交接指南\n这是一个示列\n\n*支持Markdown*\n\n> 请按需修改\n> awa!";
            public string Content {
                get => _content;
                set {
                    _content = value;
                    PropertyChanged?.Invoke();
                }
            }
            string _link = "https://docs.classisland.tech/";
            public string Link {
                get => _link;
                set {
                    _link = value;
                    PropertyChanged?.Invoke();
                }
            }
        }
    }
}