using System.Text.Json.Serialization;
using System.Windows.Controls;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Plugin;
using ExtraIsland.Shared;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "FBB380C2-5480-4FED-8349-BA5F4EAD2688",
    "(未完成)名句一言",
    PackIconKind.MessageOutline,
    "[早期阶段]显示一句古今名言,可使用三个API"
)]
public partial class Rhesis : ComponentBase {
    public Rhesis() {
        InitializeComponent();
        ShowingContent = _rhesisHandler.Get();
    }

    readonly RhesisHandler.Instance _rhesisHandler = new RhesisHandler.Instance();
    public RhesisData ShowingContent { get; }
}