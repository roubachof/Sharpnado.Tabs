using Sharpnado.Tabs;
using System.Runtime.CompilerServices;

namespace MauiSample
{
    public partial class VerticalTabItem : TabTextItem
    {
        public static readonly BindableProperty SelectorColorProperty = BindableProperty.Create(
            nameof(SelectorColor),
            typeof(Color),
            typeof(TabTextItem),
            Colors.DodgerBlue);

        public Color SelectorColor
        {
            get => (Color)GetValue(SelectorColorProperty);
            set => SetValue(SelectorColorProperty, value);
        }

        public VerticalTabItem()
        {
            InitializeComponent();

        }

        protected override void OnBadgeChanged(BadgeView oldBadge)
        {
            if (oldBadge != null)
            {
                Grid.Children.Remove(Badge);
                return;
            }

            Grid.Children.Add(Badge);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsSelectable):
                case nameof(UnselectedLabelColor):
                case nameof(SelectedTabColor):
                case nameof(IsSelected):
                    UpdateColors();
                    break;
            }
        }

        private void UpdateColors()
        {
            InnerLabel.TextColor = IsSelectable ? IsSelected ? SelectedTabColor : UnselectedLabelColor : DisabledLabelColor;
            Selector.BackgroundColor = SelectorColor;
        }
    }
}