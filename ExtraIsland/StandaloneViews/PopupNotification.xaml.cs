using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.StandaloneViews;

public partial class PopupNotification : Window {
    public PopupNotification(int width,int height,int stayTime) {
        InitializeComponent();
        BodyCard.Margin = new Thickness(height / 2.0,width / 2.0,height / 2.0,width / 2.0);
        HeaderGridTranslate.Y = width / 2.0 - (IconCard.Width + IconCard.Margin.Left) / 2.0;
        HeaderGridTranslate.X = height / 2.0 - (IconCard.Height + IconCard.Margin.Top) / 2.0;
        TitleChip.MaxWidth = 10;   
        CloseButton.Opacity = 0;
        HeaderGrid.Opacity = 0;
        TimeProgressBar.Value = 0;
        
        DoubleAnimationUsingKeyFrames fadeAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = 0
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(150))
        };
        Storyboard.SetTarget(fadeAnimation, this);
        Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(OpacityProperty));
        _fadeStoryBoard = new Storyboard {
            Children = [
                fadeAnimation
            ]
        };
        _fadeStoryBoard.Completed += (_,_) => {
            Close();
        };
        
        DoubleAnimationUsingKeyFrames timeAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new LinearDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = 0,
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(stayTime))
        };
        Storyboard.SetTarget(timeAnimation, TimeProgressBar);
        Storyboard.SetTargetProperty(timeAnimation, new PropertyPath(RangeBase.ValueProperty));
        
        Storyboard holdStoryboard = new Storyboard {
            Children = [
                timeAnimation
            ]
        };
        holdStoryboard.Completed += (_,_) => {
            _fadeStoryBoard.Begin();
        };
        
        ThicknessAnimationUsingKeyFrames bodyCardAnimation = new ThicknessAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingThicknessKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = new Thickness(20,30,20,20),
                    EasingFunction = new SineEase()
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(1000))
        };
        Storyboard.SetTarget(bodyCardAnimation, BodyCard);
        Storyboard.SetTargetProperty(bodyCardAnimation, new PropertyPath(MarginProperty));

        DoubleAnimationUsingKeyFrames headerGridXAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = 0,
                    EasingFunction = new SineEase()
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(1000))
        };
        Storyboard.SetTarget(headerGridXAnimation, HeaderGrid);
        Storyboard.SetTargetProperty(headerGridXAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
        DoubleAnimationUsingKeyFrames headerGridYAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = 0,
                    EasingFunction = new SineEase()
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(1000))
        };
        Storyboard.SetTarget(headerGridYAnimation, HeaderGrid);
        Storyboard.SetTargetProperty(headerGridYAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

        DoubleAnimationUsingKeyFrames titleChipExpandAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = width * 0.8,
                    EasingFunction = new QuadraticEase()
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(1500))
        };
        Storyboard.SetTarget(titleChipExpandAnimation, TitleChip);
        Storyboard.SetTargetProperty(titleChipExpandAnimation, new PropertyPath(MaxWidthProperty));

        DoubleAnimationUsingKeyFrames closeButtonOpacityAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = 1,
                    EasingFunction = new SineEase()
                }
            ],
            BeginTime = TimeSpan.FromMilliseconds(500),
            Duration = new Duration(TimeSpan.FromMilliseconds(1000))
        };
        Storyboard.SetTarget(closeButtonOpacityAnimation, CloseButton);
        Storyboard.SetTargetProperty(closeButtonOpacityAnimation, new PropertyPath(OpacityProperty));
        
        Storyboard expandStoryboard = new Storyboard {
            Children = [
                bodyCardAnimation,
                headerGridXAnimation,
                headerGridYAnimation,
                closeButtonOpacityAnimation,
                titleChipExpandAnimation
            ]
        };

        expandStoryboard.Completed += (_,_) => {
            if (stayTime != 0) holdStoryboard.Begin();
        };
        
        DoubleAnimationUsingKeyFrames headerGridOpacityAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(0.3),
                    Value = 1,
                    EasingFunction = new SineEase()
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(1000))
        };
        Storyboard.SetTarget(headerGridOpacityAnimation, HeaderGrid);
        Storyboard.SetTargetProperty(headerGridOpacityAnimation, new PropertyPath(OpacityProperty));
        
        DoubleAnimationUsingKeyFrames timeInitAnimation = new DoubleAnimationUsingKeyFrames {
            KeyFrames = [
                new EasingDoubleKeyFrame {
                    KeyTime = KeyTime.FromPercent(1),
                    Value = 100,
                    EasingFunction = new SineEase()
                }
            ],
            Duration = new Duration(TimeSpan.FromMilliseconds(1000))
        };
        Storyboard.SetTarget(timeInitAnimation, TimeProgressBar);
        Storyboard.SetTargetProperty(timeInitAnimation, new PropertyPath(RangeBase.ValueProperty));
        
        _waitingStoryboard = new Storyboard {
            Children = [
                headerGridOpacityAnimation,
                timeInitAnimation
            ]
        };
        _waitingStoryboard.Completed += (_,_) => {
            expandStoryboard.Begin();
        };
    }
    
    readonly Storyboard _waitingStoryboard;
    readonly Storyboard _fadeStoryBoard;
    public string? Header { get; set; }
    public Control Body { get; set; } = new Label {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Content = "Empty ;)"
    };
    void ButtonBase_OnClick(object sender,RoutedEventArgs e) {
        _fadeStoryBoard.Begin();
    }
    void PopupNotification_OnLoaded(object sender,RoutedEventArgs e) {
        if (Header == null) {
            TitleChip.Visibility = Visibility.Hidden;
        }
        _waitingStoryboard.Begin();
    }
}