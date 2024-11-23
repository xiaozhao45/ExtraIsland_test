using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ExtraIsland.StutterEngine;

public static class Animator {
    public class ClockTransformControlAnimator {
        public ClockTransformControlAnimator(Label targetLabel) {
            IsSwapAnimEnabled = true;
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(70))
            };
            Storyboard.SetTarget(fadeOutAnimation, targetLabel);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(70))
            };
            Storyboard.SetTarget(fadeInAnimation, targetLabel);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
            
            DoubleAnimationUsingKeyFrames swapOutAnimation = new DoubleAnimationUsingKeyFrames {
                KeyFrames = [
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0),
                        Value = 0,
                        EasingFunction = new QuadraticEase()
                    },
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(1),
                        Value = 40,
                        EasingFunction = new QuadraticEase()
                    }
                ],
                Duration = new Duration(TimeSpan.FromMilliseconds(70))
            };
            Storyboard.SetTarget(swapOutAnimation,targetLabel);
            Storyboard.SetTargetProperty(swapOutAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            
            
            DoubleAnimationUsingKeyFrames swapInAnimation = new DoubleAnimationUsingKeyFrames {
                KeyFrames = [
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0),
                        Value = -40,
                        EasingFunction = new QuadraticEase()
                    },
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(1),
                        Value = 0,
                        EasingFunction = new QuadraticEase()
                    }
                ],
                Duration = new Duration(TimeSpan.FromMilliseconds(70))
            };
            Storyboard.SetTarget(swapInAnimation, targetLabel);
            Storyboard.SetTargetProperty(swapInAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            Storyboard swapInStoryboard = new Storyboard {
                Children = [fadeInAnimation, swapInAnimation]
            };
            _swapOutStoryboard = new Storyboard {
                Children = [fadeInAnimation, swapOutAnimation]
            };
            _swapOutStoryboard.Completed += (_,_) => {
                targetLabel.Content = _targetContent;
                swapInStoryboard.Begin();
            };

            Storyboard fadeInStoryboard = new Storyboard {
                Children = [fadeInAnimation]
            };
            _fadeOutStoryboard = new Storyboard {
                Children = [fadeOutAnimation]
            };
            _fadeOutStoryboard.Completed += (_,_) => {
                targetLabel.Content = _targetContent;
                fadeInStoryboard.Begin();
            };
        }

        readonly Storyboard _swapOutStoryboard;

        readonly Storyboard _fadeOutStoryboard;

        string _targetContent = string.Empty;
        public string TargetContent {
            get => _targetContent;
            set {
                if (_targetContent == value) return;
                Update(value);
            }
        }

        public bool IsSwapAnimEnabled { get; set; }
        
        public void Update(string targetContent, bool isSwapAnimEnabled = true) {
            _targetContent = targetContent;
            if (isSwapAnimEnabled) {
                _swapOutStoryboard.Begin();
            } else {
                _fadeOutStoryboard.Begin();
            }
        }
    }
}