using System.ComponentModel;

using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;

using JetBrains.Annotations;

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using Sharpnado.Tabs.Effects.Droid.GestureCollectors;
using Color = Android.Graphics.Color;
using ListView = Android.Widget.ListView;
using ScrollView = Android.Widget.ScrollView;
using View = Android.Views.View;

namespace Sharpnado.Tabs.Effects.Droid;

public class TouchEffectPlatform : PlatformEffect
{
    public class RippleOverlay : View
    {
        private const string Tag = "RippleOverlay";

        protected RippleOverlay(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public RippleOverlay([NotNull] Context context)
            : base(context)
        {
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            InternalLogger.Debug(Tag, () => $"DispatchTouchEvent {e.Action}");

            if (e.Action == MotionEventActions.Move)
            {
                return false;
            }

            return base.DispatchTouchEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InternalLogger.Debug(Tag, () => $"OnTouchEvent {e.Action}");

            return base.OnTouchEvent(e);
        }
    }

    private const string Tag = "TouchEffectAndroid";

    private byte _alpha;

    private ObjectAnimator _animator;

    private Color _color;

    private RippleDrawable _ripple;

    private View _viewOverlay;

    public static bool EnableRipple => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;

    public bool IsDisposed => Container == null || Container.Handle == IntPtr.Zero;

    public View View => Control ?? Container;

    public static void Init()
    {
    }

    protected override void OnAttached()
    {
        InternalLogger.Debug(Tag, () => "OnAttached");

        if (Control is ListView || Control is ScrollView)
        {
            return;
        }

        if (Container is not ViewGroup group)
        {
            throw new InvalidOperationException(
                "Touch color effect requires to be attached to a container like a ContentView or a layout (Grid, StackLayout, etc...)");
        }

        View.Clickable = true;
        View.LongClickable = true;
        View.SoundEffectsEnabled = true;

        _viewOverlay = new RippleOverlay(Container.Context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
            Clickable = false,
            Focusable = false,
        };

        Container.LayoutChange += ViewOnLayoutChange;

        if (EnableRipple)
        {
            _viewOverlay.Background = CreateRipple(_color);
        }

        SetEffectColor();
        TouchCollector.Add(View, OnTouch, ActionType.Ripple);

        group.AddView(_viewOverlay);
        _viewOverlay.BringToFront();
    }

    protected override void OnDetached()
    {
        InternalLogger.Debug(Tag, () => "OnDetached");

        if (IsDisposed)
        {
            return;
        }

        if (Container is not ViewGroup group)
        {
            return;
        }

        group.RemoveView(_viewOverlay);
        _viewOverlay.Pressed = false;
        _viewOverlay.Foreground = null;
        _viewOverlay.Dispose();
        Container.LayoutChange -= ViewOnLayoutChange;

        if (EnableRipple)
        {
            _ripple?.Dispose();
        }

        TouchCollector.Delete(View, OnTouch, ActionType.Ripple);
    }

    protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnElementPropertyChanged(e);

        if (e.PropertyName == TouchEffect.ColorProperty.PropertyName)
        {
            SetEffectColor();
        }
    }

    private void SetEffectColor()
    {
        var color = TouchEffect.GetColor(Element);
        if (color == Colors.Transparent)
        {
            return;
        }

        _color = color.ToPlatform();
        _alpha = _color.A == 255 ? (byte)80 : _color.A;

        if (EnableRipple)
        {
            _ripple.SetColor(GetPressedColorSelector(_color));
        }
    }

    private void OnTouch(View.TouchEventArgs args)
    {
        InternalLogger.Debug(Tag, () => $"OnTouch:{args.Event.Action}");

        switch (args.Event.Action)
        {
            case MotionEventActions.Down:
                if (EnableRipple)
                {
                    InternalLogger.Debug(Tag, () => "ripple start");
                    ForceStartRipple(args.Event.GetX(), args.Event.GetY());
                }
                else
                {
                    BringLayer();
                }

                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
            case MotionEventActions.Move:
                if (IsDisposed)
                {
                    return;
                }

                if (EnableRipple)
                {
                    InternalLogger.Debug(Tag, () => "ripple end");
                    ForceEndRipple();
                }
                else
                {
                    TapAnimation(250, _alpha, 0);
                }

                break;
        }
    }

    private void ViewOnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs)
    {
        var group = (ViewGroup)sender;
        if (group == null || IsDisposed)
        {
            return;
        }

        _viewOverlay.Right = group.Width;
        _viewOverlay.Bottom = group.Height;
    }

    private RippleDrawable CreateRipple(Color color)
    {
        if (Element is Layout)
        {
            var mask = new ColorDrawable(Color.White);
            return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
        }

        var back = View.Background;
        if (back == null)
        {
            var mask = new ColorDrawable(Color.White);
            return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
        }

        if (back is RippleDrawable)
        {
            _ripple = (RippleDrawable)back.GetConstantState()
                .NewDrawable();
            _ripple.SetColor(GetPressedColorSelector(color));

            return _ripple;
        }

        return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
    }

    private static ColorStateList GetPressedColorSelector(int pressedColor)
    {
        return new ColorStateList([Array.Empty<int>()], [pressedColor]);
    }

    private void ForceStartRipple(float x, float y)
    {
        if (IsDisposed || _viewOverlay.Background is not RippleDrawable bc)
        {
            return;
        }

        _viewOverlay.BringToFront();
        bc.SetHotspot(x, y);
        _viewOverlay.Pressed = true;
    }

    private void ForceEndRipple()
    {
        if (IsDisposed)
        {
            return;
        }

        _viewOverlay.Pressed = false;
    }

    private void BringLayer()
    {
        if (IsDisposed)
        {
            return;
        }

        ClearAnimation();

        _viewOverlay.BringToFront();
        var color = _color;
        color.A = _alpha;
        _viewOverlay.SetBackgroundColor(color);
    }

    private void TapAnimation(long duration, byte startAlpha, byte endAlpha)
    {
        if (IsDisposed)
        {
            return;
        }

        _viewOverlay.BringToFront();

        var start = _color;
        var end = _color;
        start.A = startAlpha;
        end.A = endAlpha;

        ClearAnimation();
        _animator = ObjectAnimator.OfObject(
            _viewOverlay,
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

    private void AnimationOnAnimationEnd(object sender, EventArgs eventArgs)
    {
        if (IsDisposed)
        {
            return;
        }

        ClearAnimation();
    }

    private void ClearAnimation()
    {
        if (_animator == null)
        {
            return;
        }

        _animator.AnimationEnd -= AnimationOnAnimationEnd;
        _animator.Cancel();
        _animator.Dispose();
        _animator = null;
    }
}