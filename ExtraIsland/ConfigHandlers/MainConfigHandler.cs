using System.IO;
using ClassIsland.Shared.Helpers;
using ExtraIsland.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExtraIsland.ConfigHandlers;

public class MainConfigHandler {
    private readonly string _configPath;
    public MainConfigData Data { get; set; }
    
    public MainConfigHandler() {
        _configPath = Path.Combine(GlobalConstants.PluginConfigFolder!, "Main.json");
        Data = new MainConfigData();
        
        InitializeConfig();
        SubscribeToChanges();
    }

    private void InitializeConfig() {
        if (string.IsNullOrEmpty(GlobalConstants.PluginConfigFolder)) {
            throw new InvalidOperationException("配置文件夹路径未设置");
        }

        if (!File.Exists(_configPath)) {
            Save();
            return;
        }

        try {
            Data = ConfigureFileHelper.LoadConfig<MainConfigData>(_configPath);
        }
        catch (Exception ex) {
            Console.WriteLine($"加载配置文件失败: {ex.Message}");
            File.Delete(_configPath);
            Save();
        }
    }

    private void SubscribeToChanges() {
        Data.PropertyChanged += OnPropertyChanged;
        Data.TinyFeatures.JuniorGuide.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        Save();
    }

    private void Save() {
        try
        {
            ConfigureFileHelper.SaveConfig(_configPath, Data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存配置文件失败: {ex.Message}");
            throw;
        }
    }
}

public partial class MainConfigData : ObservableObject 
{
    private bool _isLifeModeActivated;
    public bool IsLifeModeActivated
    {
        get => _isLifeModeActivated;
        set
        {
            if (value == _isLifeModeActivated) return;
            _isLifeModeActivated = value;
            OnPropertyChanged();
        }
    }

    public TinyFeaturesConfig TinyFeatures { get; set; } = new TinyFeaturesConfig();

    public partial class TinyFeaturesConfig : ObservableObject
    {
        public JuniorGuideConfig JuniorGuide { get; set; } = new JuniorGuideConfig();

        public partial class JuniorGuideConfig : ObservableObject
        {
            private bool _enabled;
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    if (value == _enabled) return;
                    _enabled = value;
                    OnPropertyChanged();
                }
            }

            private string _header = "欢迎 · 电子白板使用导引";
            public string Header
            {
                get => _header;
                set
                {
                    if (value == _header) return;
                    _header = value;
                    OnPropertyChanged();
                }
            }

            private int _keepTime = 60000;
            public int KeepTime
            {
                get => _keepTime;
                set
                {
                    if (value == _keepTime) return;
                    _keepTime = value;
                    OnPropertyChanged();
                }
            }

            private string _content = "# 一体机交接指南\r\n"
                                  + "这是一个示列\r\n"
                                  + "*支持Markdown*\r\n"
                                  + "> 请按需修改\r\n"
                                  + "> awa!";
            public string Content
            {
                get => _content;
                set
                {
                    if (value == _content) return;
                    _content = value;
                    OnPropertyChanged();
                }
            }

            private string _link = "https://docs.classisland.tech/";
            public string Link
            {
                get => _link;
                set
                {
                    if (value == _link) return;
                    _link = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}