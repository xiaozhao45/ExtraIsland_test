using ClassIsland.Core.Attributes;
using ExtraIsland.Shared;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

// TODO: 调试结束后移除此组件
[ComponentInfo(
    "D61B565D-5BC9-9999-6666-2EDB22F9756E",
    "调试 · 歌词",
    PackIconKind.Bug,
    "测试歌词岛接口封装类LyricsIslandHandler()"
)]
public partial class DebugLyricsHandler {
    public DebugLyricsHandler() {
        GlobalConstants.Handlers.LyricsIsland ??= new LyricsIslandHandler();
        InitializeComponent();
        _handler = GlobalConstants.Handlers.LyricsIsland;
        _handler.OnLyricsChanged += UpdateLyrics;
        _animator = new Animators.ClockTransformControlAnimator(LyricsLabel,-0.3);
        new Thread(CounterDaemon).Start();
    }

    readonly LyricsIslandHandler _handler;
    readonly Animators.ClockTransformControlAnimator _animator;

    int _timeCounter = 10;
    
    void UpdateLyrics() {
        this.BeginInvoke(() => {
            _timeCounter = 10;
            _animator.Update(_handler.Lyrics, isForced:true);
        });
    }

    void CounterDaemon() {
        while (true) {
            _timeCounter -= 1;
            if (_timeCounter <= 0 & _animator.TargetContent != string.Empty) {
                this.BeginInvoke(() => {
                    _animator.Update(string.Empty);
                });
            }
            Thread.Sleep(1000);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}