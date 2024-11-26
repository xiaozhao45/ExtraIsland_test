using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ExtraIsland.StutterEngine;

public static class Animator {
    public class ClockTransformControlAnimator {
        public ClockTransformControlAnimator(Label targetLabel) {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            var easeFunc = new CircleEase();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(60))
            };
            Storyboard.SetTarget(fadeOutAnimation, targetLabel);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(60))
            };
            Storyboard.SetTarget(fadeInAnimation, targetLabel);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
            
            DoubleAnimationUsingKeyFrames swapOutAnimation = new DoubleAnimationUsingKeyFrames {
                KeyFrames = [
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0),
                        Value = 0,
                        EasingFunction = easeFunc
                    },
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(1),
                        Value = 35,
                        EasingFunction = easeFunc
                    }
                ],
                Duration = new Duration(TimeSpan.FromMilliseconds(90))
            };
            Storyboard.SetTarget(swapOutAnimation,targetLabel);
            Storyboard.SetTargetProperty(swapOutAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            
            
            DoubleAnimationUsingKeyFrames swapInAnimation = new DoubleAnimationUsingKeyFrames {
                KeyFrames = [
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0),
                        Value = -40,
                        EasingFunction = easeFunc
                    },
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(1),
                        Value = 0,
                        EasingFunction = easeFunc
                    }
                ],
                Duration = new Duration(TimeSpan.FromMilliseconds(90))
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
                _swapOutStoryboard.Stop();
                Dispatcher.CurrentDispatcher.Invoke(() => {
                    swapInStoryboard.Begin();
                });
            };
            swapInStoryboard.Completed += (_,_) => {
                swapInStoryboard.Stop();
                _renderLock = false;
            };
            Timeline.SetDesiredFrameRate(swapInStoryboard, 60);
            Timeline.SetDesiredFrameRate(_swapOutStoryboard, 60);

            Storyboard fadeInStoryboard = new Storyboard {
                Children = [fadeInAnimation]
            };
            _fadeOutStoryboard = new Storyboard {
                Children = [fadeOutAnimation]
            };
            _fadeOutStoryboard.Completed += (_,_) => {
                targetLabel.Content = _targetContent;
                _fadeOutStoryboard.Stop();
                Dispatcher.CurrentDispatcher.Invoke(() => {
                    fadeInStoryboard.Begin();
                });
            };
            fadeInStoryboard.Completed += (_,_) => {
                fadeInStoryboard.Stop();
                _renderLock = false;
            };
            Timeline.SetDesiredFrameRate(fadeInStoryboard, 60);
            Timeline.SetDesiredFrameRate(_fadeOutStoryboard, 60);
        }

        readonly Storyboard _swapOutStoryboard;

        readonly Storyboard _fadeOutStoryboard;

        bool _renderLock;
        
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
            if (_renderLock) return;
            _renderLock = true;
            if (isSwapAnimEnabled) {
                _swapOutStoryboard.Begin();
            } else {
                _fadeOutStoryboard.Begin();
            }
        }
    }
    
    public class SeparatorControlAnimator {
        public SeparatorControlAnimator(Control targetControl) {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            var easeFunc = new SineEase();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(60)),
                EasingFunction = easeFunc
            };
            Storyboard.SetTarget(fadeOutAnimation, targetControl);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));
            _fadeOutStoryboard = new Storyboard {
                Children = [fadeOutAnimation]
            };

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(60)),
                EasingFunction = easeFunc
            };
            Storyboard.SetTarget(fadeInAnimation, targetControl);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
            _fadeInStoryboard = new Storyboard {
                Children = [fadeInAnimation]
            };
        }

        readonly Storyboard _fadeOutStoryboard;
        readonly Storyboard _fadeInStoryboard;
        
        public void Update(bool isInverse = false) {
            if (isInverse) {
                _fadeOutStoryboard.Begin();
            } else {
                _fadeInStoryboard.Begin();
            }
        }
    }
    
    public class EmphasizeUiElementAnimator {
        public EmphasizeUiElementAnimator(UIElement targetControl) {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            var easeFunc = new SineEase();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(60)),
                EasingFunction = easeFunc
            };
            Storyboard.SetTarget(fadeOutAnimation, targetControl);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(60)),
                BeginTime = TimeSpan.FromSeconds(3),
                EasingFunction = easeFunc
            };
            Storyboard.SetTarget(fadeInAnimation, targetControl);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));

            _fadeOutStoryboard = new Storyboard {
                Children = [fadeOutAnimation,fadeInAnimation]
            };
        }

        readonly Storyboard _fadeOutStoryboard;

        public void Update() {
            _fadeOutStoryboard.Begin();
        }
    }
}