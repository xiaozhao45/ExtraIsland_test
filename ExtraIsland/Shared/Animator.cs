using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ExtraIsland.Shared;

public static class Animator {
    public class ClockTransformControlAnimator {
        public ClockTransformControlAnimator(Label targetLabel) {
            _targetLabel = targetLabel;
            DoubleAnimationUsingKeyFrames swapFadeAnimation = new DoubleAnimationUsingKeyFrames {
                KeyFrames = [
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0.5),
                        Value = 0,
                        EasingFunction = new CubicEase {
                            EasingMode = EasingMode.EaseInOut
                        }
                    },
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(1),
                        Value = 1,
                        EasingFunction = new CubicEase {
                            EasingMode = EasingMode.EaseIn
                        }
                    }
                ],
                Duration = new Duration(TimeSpan.FromMilliseconds(220))
            };
            Storyboard.SetTarget(swapFadeAnimation, targetLabel);
            Storyboard.SetTargetProperty(swapFadeAnimation, new PropertyPath(UIElement.OpacityProperty));
            
            DoubleAnimationUsingKeyFrames fadeAnimation = new DoubleAnimationUsingKeyFrames {
                KeyFrames = [
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0.5),
                        Value = 0.1
                    },
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(1),
                        Value = 1
                    }
                ],
                Duration = new Duration(TimeSpan.FromMilliseconds(220))
            };
            Storyboard.SetTarget(fadeAnimation, targetLabel);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(UIElement.OpacityProperty));
            
            DoubleAnimationUsingKeyFrames swapAnimation = new DoubleAnimationUsingKeyFrames {
                KeyFrames = [
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0.5),
                        Value = 40,
                        EasingFunction = new CubicEase {
                            EasingMode = EasingMode.EaseIn
                        }
                    },
                    new DiscreteDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(0.501),
                        Value = -40
                    },
                    new EasingDoubleKeyFrame {
                        KeyTime = KeyTime.FromPercent(1),
                        Value = 0,
                        EasingFunction = new CubicEase {
                            EasingMode = EasingMode.EaseIn
                        }
                    }
                ],
                Duration = new Duration(TimeSpan.FromMilliseconds(220)),
                FillBehavior = FillBehavior.Stop
            };
            Storyboard.SetTarget(swapAnimation,targetLabel);
            Storyboard.SetTargetProperty(swapAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            
            _swapStoryboard = new Storyboard {
                Children = [swapAnimation,swapFadeAnimation]
            };
            _swapStoryboard.Completed += (_,_) => {
                _renderLock = false;
            };
            //Timeline.SetDesiredFrameRate(_swapStoryboard, 60);
            
            _fadeStoryboard = new Storyboard {
                Children = [fadeAnimation]
            };
            _fadeStoryboard.Completed += (_,_) => {
                _renderLock = false;
            };
            //Timeline.SetDesiredFrameRate(_fadeStoryboard, 60);
        }

        readonly Storyboard _swapStoryboard;

        readonly Storyboard _fadeStoryboard;

        bool _renderLock;

        readonly Label _targetLabel;
        
        string _targetContent = string.Empty;
        public string TargetContent {
            get => _targetContent;
            set {
                if (_targetContent == value) return;
                Update(value);
            }
        }
        
        public void Update(string targetContent, bool isSwapAnimEnabled = true) {
            _targetContent = targetContent;
            if (_renderLock) return;
            _renderLock = true;
            if (isSwapAnimEnabled) {
                _swapStoryboard.Begin();
            } else {
                _fadeStoryboard.Begin();
            }
            new Thread(() => {
                Thread.Sleep(110);
                _targetLabel.Dispatcher.InvokeAsync(() => {
                    _targetLabel.Content = _targetContent;
                });
            }).Start();
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