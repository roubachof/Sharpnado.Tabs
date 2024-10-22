using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Maui.Controls.Platform;

using Sharpnado.Tabs.Effects.iOS.GestureCollectors;
using Sharpnado.Tabs.Effects.iOS.GestureRecognizers;

using UIKit;

namespace Sharpnado.Tabs.Effects.iOS {
    public class CommandsPlatform : PlatformEffect {
        public UIView View => Control ?? Container;

        DateTime _tapTime;
        ICommand _tapCommand;
        ICommand _longCommand;
        object _tapParameter;
        object _longParameter;

        protected override void OnAttached() {
            View.UserInteractionEnabled = true;

            UpdateTap();
            UpdateTapParameter();
            UpdateLongTap();
            UpdateLongTapParameter();

            TouchGestureCollector.Add(View, OnTouch, ActionType.Tap);
        }

        protected override void OnDetached() {
            TouchGestureCollector.Delete(View, OnTouch);
        }

        void OnTouch(TouchGestureRecognizer.TouchArgs e) {
            switch (e.State) {
                case TouchGestureRecognizer.TouchState.Started:
                    _tapTime = DateTime.Now;
                    break;

                case TouchGestureRecognizer.TouchState.Ended:
                    if (e.Inside) {
                        var range = (DateTime.Now - _tapTime).TotalMilliseconds;
                        if (range > 800)
                            LongClickHandler();
                        else
                            ClickHandler();
                    }
                    break;

                case TouchGestureRecognizer.TouchState.Cancelled:
                    break;
            }
        }

        void ClickHandler() {
            if (_tapCommand?.CanExecute(_tapParameter) ?? false)
                _tapCommand.Execute(_tapParameter);
        }

        void LongClickHandler() {
            if (_longCommand == null)
                ClickHandler();
            else if (_longCommand.CanExecute(_longParameter))
                _longCommand.Execute(_longParameter);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args) {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Commands.TapProperty.PropertyName)
                UpdateTap();
            else if (args.PropertyName == Commands.TapParameterProperty.PropertyName)
                UpdateTapParameter();
            else if (args.PropertyName == Commands.LongTapProperty.PropertyName)
                UpdateLongTap();
            else if (args.PropertyName == Commands.LongTapParameterProperty.PropertyName)
                UpdateLongTapParameter();
        }

        void UpdateTap() {
            _tapCommand = Commands.GetTap(Element);
        }

        void UpdateTapParameter() {
            _tapParameter = Commands.GetTapParameter(Element);
        }

        void UpdateLongTap() {
            _longCommand = Commands.GetLongTap(Element);
        }

        void UpdateLongTapParameter() {
            _longParameter = Commands.GetLongTapParameter(Element);
        }

        public static void Init() {
        }
    }
}
