using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public abstract class UnderlinedTabItemBase : TabTextItem
    {
        public static readonly BindableProperty UnderlineAllTabProperty = BindableProperty.Create(
            nameof(UnderlineAllTab),
            typeof(bool),
            typeof(TabTextItem),
            true);

        public static readonly BindableProperty UnderlineHeightProperty = BindableProperty.Create(
            nameof(UnderlineHeight),
            typeof(double),
            typeof(TabTextItem),
            3d);

        public UnderlinedTabItemBase()
        {
            LabelSize = 14;
        }

        public bool UnderlineAllTab
        {
            get => (bool)GetValue(UnderlineAllTabProperty);
            set => SetValue(UnderlineAllTabProperty, value);
        }

        public double UnderlineHeight
        {
            get => (double)GetValue(UnderlineHeightProperty);
            set => SetValue(UnderlineHeightProperty, value);
        }

        protected abstract Label InnerLabelImpl { get; }

        protected abstract Grid GridImpl { get; }

        protected abstract BoxView UnderlineImpl { get; }

        protected abstract View ContentImpl { get; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Margin):
                    UpdateMargin();
                    break;

                case nameof(Padding):
                    UpdatePadding();
                    break;

                case nameof(Width):
                case nameof(UnderlineAllTab):
                    UpdateUnderlineAllTab();
                    break;

                case nameof(IsSelectable):
                case nameof(UnselectedLabelColor):
                case nameof(SelectedTabColor):
                case nameof(IsSelected):
                    UpdateColors();
                    break;
            }
        }

        protected override void OnBadgeChanged(BadgeView oldBadge)
        {
            if (oldBadge != null)
            {
                GridImpl.Children.Remove(Badge);
                return;
            }

            GridImpl.Children.Add(Badge);
        }

        protected void ContentImplPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Width) && ContentImpl.Width > 1)
            {
                UpdateUnderlineAllTab();
            }
        }

        protected void UpdateUnderlineAllTab()
        {
            UnderlineImpl.Margin = UnderlineAllTab
                ? new Thickness(-Margin.Left - Padding.Left, 0, -Margin.Right - Padding.Right, 0)
                : new Thickness(0);
            UnderlineImpl.WidthRequest = UnderlineAllTab ? Width : ContentImpl.Width;
        }

        private void UpdatePadding()
        {
            UnderlineImpl.Margin = UnderlineAllTab
                ? new Thickness(UnderlineImpl.Margin.Left - Padding.Left, 0, UnderlineImpl.Margin.Right - Padding.Right, 0)
                : new Thickness(0);
        }

        private void UpdateMargin()
        {
            UnderlineImpl.Margin = UnderlineAllTab
                ? new Thickness(UnderlineImpl.Margin.Left - Margin.Left, 0, UnderlineImpl.Margin.Right - Margin.Right, 0)
                : new Thickness(0);
        }

        private void UpdateColors()
        {
            InnerLabelImpl.TextColor = IsSelectable ? IsSelected ? SelectedTabColor : UnselectedLabelColor : DisabledLabelColor;
            UnderlineImpl.Color = SelectedTabColor;
        }
    }
}
