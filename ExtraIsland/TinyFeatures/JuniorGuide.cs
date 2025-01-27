using System.Windows;
using System.Windows.Controls;
using ClassIsland.Core.Controls.CommonDialog;
using ClassIsland.Core.Helpers;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using ExtraIsland.StandaloneViews;
using MaterialDesignThemes.Wpf;
using MdXaml;
using Brushes = System.Windows.Media.Brushes;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace ExtraIsland.TinyFeatures;

public static class JuniorGuide {
    public static void Trigger() {
        if (!GlobalConstants.Handlers.MainConfig!.Data.TinyFeatures.JuniorGuide.Enabled) return;
        Show();
    }

    public static void Show(bool isPreview = false) {
        MainConfigData.TinyFeaturesConfig.JuniorGuideConfig settings = 
            GlobalConstants.Handlers.MainConfig!.Data.TinyFeatures.JuniorGuide;
        
        Button openButton = new Button {
            Margin = new Thickness(6),
            Content = new StackPanel {
                Orientation = Orientation.Horizontal,
                Children = {
                    new PackIcon {
                        Foreground = Brushes.White,
                        Kind = PackIconKind.Launch,
                        Height = 25,Width = 25
                    },
                    new Label {
                        Foreground = Brushes.White,
                        Content = "打开指南"
                    }
                }
            }
        };

        Button noMoreButton = new Button {
            Margin = new Thickness(6),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Content = new StackPanel {
                Orientation = Orientation.Horizontal,
                Children = {
                    new PackIcon {
                        Foreground = Brushes.DimGray,
                        Kind = PackIconKind.Close,
                        Height = 25,Width = 25
                    },
                    new Label {
                        Foreground = Brushes.DimGray,
                        Content = "不再显示"
                    }
                }
            }
        };

        PopupNotification popup = new PopupNotification(350,575,settings.KeepTime) {
            Title = "ExtraIsland·导引",
            Header = settings.Header,
            PackIconControl = new PackIcon {
                Foreground = Brushes.White,
                Kind = PackIconKind.HumanMaleBoard,
                Height = 25, Width = 25
            },
            Body = new Label {
                Content = new StackPanel {
                    Orientation = Orientation.Vertical,
                    Children = {
                        new MarkdownScrollViewer {
                            Height = 230,
                            IsSelectionEnabled = false,
                            Document = MarkdownConvertHelper.ConvertMarkdown(settings.Content)
                        },
                        new StackPanel {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Children = {
                                openButton,
                                noMoreButton
                            }
                        }
                    }
                },
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        openButton.Click += (_, _) => {
            try {
                popup.Topmost = false;
                System.Diagnostics.Process.Start("explorer.exe", settings.Link);
            }
            catch {
                CommonDialog.ShowError($"无法打开指南链接[{settings.Link}]");
            }
        };

        noMoreButton.Click += (_, _) => {
            if (isPreview) {
                popup.Topmost = false;
                CommonDialog.ShowHint("预览模式,将保持功能启用状态不变");
            } else {
                GlobalConstants.Handlers.MainConfig!.Data.TinyFeatures.JuniorGuide.Enabled = false;
            }
            popup.Close();
        };
        new Thread(() => {
            if (!isPreview) {
                Thread.Sleep(9000);
            }
            popup.Dispatcher.BeginInvoke(() => {
                popup.Show();
            });
        }).Start();
    }
}