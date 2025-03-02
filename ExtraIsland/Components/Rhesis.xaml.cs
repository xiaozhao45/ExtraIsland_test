using System.Windows;
using System.IO;
using System.Linq;
using System.Threading;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ExtraIsland.Shared;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

using SharedRhesisDataSource = ExtraIsland.Shared.RhesisDataSource;

namespace ExtraIsland.Components;

[ComponentInfo(
    "FBB380C2-5480-4FED-8349-BA5F4EAD2688",
    "名句一言",
    PackIconKind.MessageOutline,
    "显示一句古今名言,可使用三个API"
)]
public partial class Rhesis {
    public Rhesis(ILessonsService lessonsService) {
        LessonsService = lessonsService;
        InitializeComponent();
        _labelAnimator = new Animators.ClockTransformControlAnimator(Label);
    }
    
    ILessonsService LessonsService { get; }
    
    public string Showing { get; private set; } = "-----------------";
    readonly RhesisHandler.Instance _rhesisHandler = new RhesisHandler.Instance();
    readonly Animators.ClockTransformControlAnimator _labelAnimator;
    
    void Rhesis_OnLoaded(object sender,RoutedEventArgs e) {
        Settings.LastUpdate = DateTime.Now;
        Update();
        LessonsService.PostMainTimerTicked += UpdateEvent;
    }
    
    void Rhesis_OnUnloaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked -= UpdateEvent;
    }

    void UpdateEvent(object? sender,EventArgs eventArgs) {
        if (EiUtils.GetDateTimeSpan(Settings.LastUpdate,DateTime.Now) < Settings.UpdateTimeGap|Settings.UpdateTimeGapSeconds == 0) return;
        Settings.LastUpdate = DateTime.Now;
        Update();
    }
    
    void Update() {
        new Thread(() => {
            if (Settings.DataSource == RhesisDataSource.LocalFile) {
                try {
                    var lines = File.ReadAllLines(Settings.LocalFilePath)
                                 .Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                                 .ToList();
                    if (lines.Count > 0) {
                        Showing = lines[new Random().Next(lines.Count)].Trim();
                    } else {
                        Showing = "本地文件没有有效语录";
                    }
                } catch (FileNotFoundException) {
                    Showing = "语录文件未找到";
                } catch (DirectoryNotFoundException) {
                    Showing = "目录不存在";
                } catch (IOException ex) {
                    Showing = $"文件读取错误: {ex.Message}";
                } catch (Exception ex) {
                    Showing = $"本地文件读取失败: {ex.Message}";
                }
            }}).Start();
    }
}
