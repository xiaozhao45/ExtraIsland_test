using System.Text.Json.Serialization;
using System.Windows.Controls;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Plugin;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "FBB380C2-5480-4FED-8349-BA5F4EAD2688",
    "(未完成)古诗名言",
    PackIconKind.MessageOutline,
    "显示一句名言"
)]
public partial class Rhesis : ComponentBase {
    public Rhesis() {
        InitializeComponent();
    }
    
    public RhesisData ShowingContent { get; set; } = new RhesisData();
}