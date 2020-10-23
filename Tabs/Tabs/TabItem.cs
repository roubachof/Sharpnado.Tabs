using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public abstract class TabItem : ContentView
    {
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(TabTextItem),
            false);

        public static readonly BindableProperty SelectedTabColorProperty = BindableProperty.Create(
            nameof(SelectedTabColor),
            typeof(Color),
            typeof(TabTextItem),
            Color.Default);

        public static readonly BindableProperty BadgeProperty = BindableProperty.Create(
            nameof(Badge),
            typeof(BadgeView),
            typeof(TabItem),
            default(BadgeView),
            propertyChanged: OnBadgeChanged);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Color SelectedTabColor
        {
            get => (Color)GetValue(SelectedTabColorProperty);
            set => SetValue(SelectedTabColorProperty, value);
        }

        public BadgeView Badge
        {
            get => (BadgeView)GetValue(BadgeProperty);
            set => SetValue(BadgeProperty, value);
        }

        public bool IsSelectable { get; set; } = true;

        protected abstract void OnBadgeChanged(BadgeView oldBadge);

        private static void OnBadgeChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabItem = (TabItem)bindable;
            tabItem.OnBadgeChanged((BadgeView)oldvalue);
        }
    }
}
