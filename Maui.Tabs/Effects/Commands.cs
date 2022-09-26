using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamEffects {
    public static class Commands {
        [Obsolete("Not need with usual Linking")]
        public static void Init() {
        }

        public static readonly BindableProperty TapProperty =
            BindableProperty.CreateAttached(
                "Tap",
                typeof(ICommand),
                typeof(Commands),
                default(ICommand),
                propertyChanged: PropertyChanged
            );

        public static void SetTap(BindableObject view, ICommand value) {
            view.SetValue(TapProperty, value);
        }

        public static ICommand GetTap(BindableObject view) {
            return (ICommand)view.GetValue(TapProperty);
        }

        public static readonly BindableProperty TapParameterProperty =
            BindableProperty.CreateAttached(
                "TapParameter",
                typeof(object),
                typeof(Commands),
                default(object),
                propertyChanged: PropertyChanged
            );

        public static void SetTapParameter(BindableObject view, object value) {
            view.SetValue(TapParameterProperty, value);
        }

        public static object GetTapParameter(BindableObject view) {
            return view.GetValue(TapParameterProperty);
        }

        public static readonly BindableProperty LongTapProperty =
            BindableProperty.CreateAttached(
                "LongTap",
                typeof(ICommand),
                typeof(Commands),
                default(ICommand),
                propertyChanged: PropertyChanged
            );

        public static void SetLongTap(BindableObject view, ICommand value) {
            view.SetValue(LongTapProperty, value);
        }

        public static ICommand GetLongTap(BindableObject view) {
            return (ICommand)view.GetValue(LongTapProperty);
        }

        public static readonly BindableProperty LongTapParameterProperty =
            BindableProperty.CreateAttached(
                "LongTapParameter",
                typeof(object),
                typeof(Commands),
                default(object)
            );

        public static void SetLongTapParameter(BindableObject view, object value) {
            view.SetValue(LongTapParameterProperty, value);
        }

        public static object GetLongTapParameter(BindableObject view) {
            return view.GetValue(LongTapParameterProperty);
        }

        static void PropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            if (!(bindable is View view))
                return;

            var eff = view.Effects.FirstOrDefault(e => e is CommandsRoutingEffect);

            if (GetTap(bindable) != null || GetLongTap(bindable) != null) {
                view.InputTransparent = false;

                if (eff != null) return;
                view.Effects.Add(new CommandsRoutingEffect());
                if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                    !EffectsConfig.GetChildrenInputTransparent(view)) {
                    EffectsConfig.SetChildrenInputTransparent(view, true);
                }
            }
            else {
                if (eff == null || view.BindingContext == null) return;
                view.Effects.Remove(eff);
                if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                    EffectsConfig.GetChildrenInputTransparent(view)) {
                    EffectsConfig.SetChildrenInputTransparent(view, false);
                }
            }
        }
    }

    public class CommandsRoutingEffect : RoutingEffect {
        public CommandsRoutingEffect() : base("XamEffects." + nameof(Commands)) {
        }
    }
}