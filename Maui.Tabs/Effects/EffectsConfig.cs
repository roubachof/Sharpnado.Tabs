using System;
using Xamarin.Forms;

namespace XamEffects {
    public static class EffectsConfig {
        [Obsolete("Not need with usual Linking")]
        public static void Init() {
        }

        public static bool AutoChildrenInputTransparent { get; set; } = true;

        public static readonly BindableProperty ChildrenInputTransparentProperty =
            BindableProperty.CreateAttached(
                "ChildrenInputTransparent",
                typeof(bool),
                typeof(EffectsConfig),
                false,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ConfigureChildrenInputTransparent(bindable);
                }
            );

        public static void SetChildrenInputTransparent(BindableObject view, bool value) {
            view.SetValue(ChildrenInputTransparentProperty, value);
        }

        public static bool GetChildrenInputTransparent(BindableObject view) {
            return (bool)view.GetValue(ChildrenInputTransparentProperty);
        }

        static void ConfigureChildrenInputTransparent(BindableObject bindable) {
            if (!(bindable is Layout layout))
                return;

            if (GetChildrenInputTransparent(bindable)) {
                foreach (View layoutChild in layout.Children)
                    AddInputTransparentToElement(layoutChild);
                layout.ChildAdded += Layout_ChildAdded;
            }
            else {
                layout.ChildAdded -= Layout_ChildAdded;
            }
        }

        static void Layout_ChildAdded(object sender, ElementEventArgs e) {
            AddInputTransparentToElement(e.Element);
        }

        static void AddInputTransparentToElement(BindableObject obj) {
            if (obj is View view && TouchEffect.GetColor(view) == Colors.Transparent && Commands.GetTap(view) == null && Commands.GetLongTap(view) == null) {
                view.InputTransparent = true;
            }
        }
    }
}