using System.Windows;
using ExtraIsland.Shared;

namespace ExtraIsland.Components;

public partial class LiveActivitySettings {
    public LiveActivitySettings() {
        IsLyricsIslandLoaded = EiUtils.IsLyricsIslandInstalled();
        InitializeComponent();
        ConflictHintContainer.Visibility = IsLyricsIslandLoaded ? Visibility.Visible : Visibility.Collapsed;
    }

    bool IsLyricsIslandLoaded { get; }
    public bool IsLyricsIslandNotLoaded { get => !IsLyricsIslandLoaded; }
}