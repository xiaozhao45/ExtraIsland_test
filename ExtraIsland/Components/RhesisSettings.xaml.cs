using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClassIsland.Core.Abstractions.Controls;
using ExtraIsland.Shared;

namespace ExtraIsland.Components;

public partial class RhesisSettings {
    public RhesisSettings() {
        InitializeComponent();
    }

    public List<RhesisDataSource> DataSources { get; } = [
        RhesisDataSource.All,
        RhesisDataSource.Hitokoto,
        RhesisDataSource.Jinrishici,
        RhesisDataSource.Saint,
        RhesisDataSource.SaintJinrishici
    ]; 
}