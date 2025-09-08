namespace Sharpnado.Tabs
{
    public abstract class TabItem : ContentView
    {
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(TabItem),
            false);

        public static readonly BindableProperty SelectedTabColorProperty = BindableProperty.Create(
            nameof(SelectedTabColor),
            typeof(Color),
            typeof(TabItem),
            Colors.Magenta);

        public static readonly BindableProperty BadgeProperty = BindableProperty.Create(
            nameof(Badge),
            typeof(BadgeView),
            typeof(TabItem),
            null,
            propertyChanged: OnBadgeChanged);

        public static readonly BindableProperty IsSelectableProperty = BindableProperty.Create(
            nameof(IsSelectable),
            typeof(bool),
            typeof(TabItem),
            true);

        public static readonly BindableProperty DisabledLabelColorProperty = BindableProperty.Create(
            nameof(DisabledLabelColor),
            typeof(Color),
            typeof(TabItem),
            Colors.Grey);

        public Color DisabledLabelColor
        {
            get => (Color)GetValue(DisabledLabelColorProperty);
            set => SetValue(DisabledLabelColorProperty, value);
        }

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

        public bool IsSelectable
        {
            get => (bool)GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }

        protected abstract void OnBadgeChanged(BadgeView oldBadge);

        private static void OnBadgeChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabItem = (TabItem)bindable;
            tabItem.OnBadgeChanged((BadgeView)oldvalue);
        }
    }
}
