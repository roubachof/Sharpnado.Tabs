using System.ComponentModel;
using Android.Animation;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;

using XamEffects;
using XamEffects.Droid;
using XamEffects.Droid.GestureCollectors;
using Color = Android.Graphics.Color;
using ListView = Android.Widget.ListView;
using ScrollView = Android.Widget.ScrollView;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(TouchEffectPlatform), nameof(TouchEffect))]

namespace XamEffects.Droid {
    public class TouchEffectPlatform : PlatformEffect {
        public bool EnableRipple => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
        public bool IsDisposed => (Container as IVisualElementRenderer)?.Element == null;
        public View View => Control ?? Container;

        Color _color;
        byte _alpha;
        RippleDrawable _ripple;
        FrameLayout _viewOverlay;
        ObjectAnimator _animator;

        public static void Init() {
        }

        protected override void OnAttached() {
            if (Control is ListView || Control is ScrollView) {
                return;
            }

            if (Container is not ViewGroup group)
            {
                throw new InvalidOperationException("Container must be a ViewGroup");
            }

            View.Clickable = true;
            View.LongClickable = true;
            _viewOverlay = new FrameLayout(Container.Context) {
                LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
                Clickable = false,
                Focusable = false,
            };
            Container.LayoutChange += ViewOnLayoutChange;

            if (EnableRipple)
                _viewOverlay.Background = CreateRipple(_color);

            SetEffectColor();
            TouchCollector.Add(View, OnTouch);

            group.AddView(_viewOverlay);
            _viewOverlay.BringToFront();
        }

        protected override void OnDetached() {
            if (IsDisposed) return;

            if (Container is not ViewGroup group)
            {
                throw new InvalidOperationException("Container must be a ViewGroup");
            }

            group.RemoveView(_viewOverlay);
            _viewOverlay.Pressed = false;
            _viewOverlay.Foreground = null;
            _viewOverlay.Dispose();
            Container.LayoutChange -= ViewOnLayoutChange;

            if (EnableRipple)
                _ripple?.Dispose();

            TouchCollector.Delete(View, OnTouch);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == TouchEffect.ColorProperty.PropertyName) {
                SetEffectColor();
            }
        }

        void SetEffectColor() {
            var color = TouchEffect.GetColor(Element);
            if (color == Colors.Transparent) {
                return;
            }

            _color = color.ToAndroid();
            _alpha = _color.A == 255 ? (byte)80 : _color.A;

            if (EnableRipple) {
                _ripple.SetColor(GetPressedColorSelector(_color));
            }
        }

        void OnTouch(View.TouchEventArgs args) {
            switch (args.Event.Action) {
                case MotionEventActions.Down:
                    if (EnableRipple)
                        ForceStartRipple(args.Event.GetX(), args.Event.GetY());
                    else
                        BringLayer();

                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    if (IsDisposed) return;

                    if (EnableRipple)
                        ForceEndRipple();
                    else
                        TapAnimation(250, _alpha, 0);

                    break;
            }
        }

        void ViewOnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs) {
            var group = (ViewGroup)sender;
            if (group == null || IsDisposed) return;
            _viewOverlay.Right = group.Width;
            _viewOverlay.Bottom = group.Height;
        }

        #region Ripple

        RippleDrawable CreateRipple(Color color) {
            if (Element is Layout) {
                var mask = new ColorDrawable(Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }

            var back = View.Background;
            if (back == null) {
                var mask = new ColorDrawable(Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }

            if (back is RippleDrawable) {
                _ripple = (RippleDrawable)back.GetConstantState().NewDrawable();
                _ripple.SetColor(GetPressedColorSelector(color));

                return _ripple;
            }

            return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
        }

        static ColorStateList GetPressedColorSelector(int pressedColor) {
            return new ColorStateList(
                new[] { new int[] { } },
                new[] { pressedColor, });
        }

        void ForceStartRipple(float x, float y) {
            if (IsDisposed || !(_viewOverlay.Background is RippleDrawable bc)) return;

            _viewOverlay.BringToFront();
            bc.SetHotspot(x, y);
            _viewOverlay.Pressed = true;
        }

        void ForceEndRipple() {
            if (IsDisposed) return;

            _viewOverlay.Pressed = false;
        }

        #endregion

        #region Overlay

        void BringLayer() {
            if (IsDisposed)
                return;

            ClearAnimation();

            _viewOverlay.BringToFront();
            var color = _color;
            color.A = _alpha;
            _viewOverlay.SetBackgroundColor(color);
        }

        void TapAnimation(long duration, byte startAlpha, byte endAlpha) {
            if (IsDisposed)
                return;

            _viewOverlay.BringToFront();

            var start = _color;
            var end = _color;
            start.A = startAlpha;
            end.A = endAlpha;

            ClearAnimation();
            _animator = ObjectAnimator.OfObject(_viewOverlay,
                "BackgroundColor",
                new ArgbEvaluator(),
                start.ToArgb(),
                end.ToArgb());
            _animator.SetDuration(duration);
            _animator.RepeatCount = 0;
            _animator.RepeatMode = ValueAnimatorRepeatMode.Restart;
            _animator.Start();
            _animator.AnimationEnd += AnimationOnAnimationEnd;
        }

        void AnimationOnAnimationEnd(object sender, EventArgs eventArgs) {
            if (IsDisposed) return;

            ClearAnimation();
        }

        void ClearAnimation() {
            if (_animator == null) return;
            _animator.AnimationEnd -= AnimationOnAnimationEnd;
            _animator.Cancel();
            _animator.Dispose();
            _animator = null;
        }

        #endregion
    }
}