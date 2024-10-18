using System.ComponentModel;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using Sharpnado.Tabs.Effects.iOS.GestureCollectors;
using Sharpnado.Tabs.Effects.iOS.GestureRecognizers;
using UIKit;

namespace Sharpnado.Tabs.Effects.iOS {
    public class TouchEffectPlatform : PlatformEffect
    {
        public bool IsDisposed => Container == null || Container.Handle == NativeHandle.Zero;
        public UIView View => Control ?? Container;

        UIView _layer;
        float _alpha;

        protected override void OnAttached() {
            View.UserInteractionEnabled = true;
            _layer = new UIView {
                UserInteractionEnabled = false,
                Opaque = false,
                Alpha = 0,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            UpdateEffectColor();
            TouchGestureCollector.Add(View, OnTouch);

            View.AddSubview(_layer);
            View.BringSubviewToFront(_layer);
            _layer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            _layer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            _layer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _layer.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
        }

        protected override void OnDetached() {
            TouchGestureCollector.Delete(View, OnTouch);
            _layer?.RemoveFromSuperview();
            _layer?.Dispose();
        }

        void OnTouch(TouchGestureRecognizer.TouchArgs e) {
            switch (e.State) {
                case TouchGestureRecognizer.TouchState.Started:
                    InternalLogger.Debug($"OnTouch Started");
                    BringLayer();
                    break;

                case TouchGestureRecognizer.TouchState.Ended:
                    InternalLogger.Debug($"OnTouch Ended");
                    EndAnimation();
                    break;

                case TouchGestureRecognizer.TouchState.Cancelled:
                    InternalLogger.Debug($"OnTouch Cancelled");
                    if (!IsDisposed && _layer != null) {
                        _layer.Layer.RemoveAllAnimations();
                        _layer.Alpha = 0;
                    }

                    break;
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == TouchEffect.ColorProperty.PropertyName) {
                UpdateEffectColor();
            }
        }

        void UpdateEffectColor() {
            var color = TouchEffect.GetColor(Element);
            if (color == Colors.Transparent) {
                return;
            }

            InternalLogger.Debug($"UpdateEffectColor");
            _alpha = color.Alpha < 1.0 ? 1 : (float)0.3;
            _layer.BackgroundColor = color.ToPlatform();
        }

        void BringLayer() {
            InternalLogger.Debug($"BringLayer");
            _layer.Layer.RemoveAllAnimations();
            _layer.Alpha = _alpha;
            View.BringSubviewToFront(_layer);
        }

        void EndAnimation() {
            if (!IsDisposed && _layer != null) {
                InternalLogger.Debug($"EndAnimation");
                _layer.Layer.RemoveAllAnimations();
                UIView.Animate(0.225,
                () => {
                    _layer.Alpha = 0;
                });
            }
        }

        public static void Init() {
        }
    }
}
