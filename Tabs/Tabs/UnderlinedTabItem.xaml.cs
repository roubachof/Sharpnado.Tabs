using System.Linq;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderlinedTabItem : TabTextItem
    {
        public static readonly BindableProperty UnderlineAllTabProperty = BindableProperty.Create(
            nameof(UnderlineAllTab),
            typeof(bool),
            typeof(TabTextItem),
            true);

        public UnderlinedTabItem()
        {
            InitializeComponent();

            LabelSize = 14;
        }

        public bool UnderlineAllTab
        {
            get => (bool)GetValue(UnderlineAllTabProperty);
            set => SetValue(UnderlineAllTabProperty, value);
        }

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

                case nameof(UnderlineAllTab):
                    UpdateUnderlineAllTab();
                    break;
            }
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

        private void UpdateUnderlineAllTab()
        {
            Underline.Margin = UnderlineAllTab
                ? new Thickness(-Margin.Left - Padding.Left, 0, -Margin.Right - Padding.Right, 0)
                : new Thickness(0);
        }

        private void UpdatePadding()
        {
            Underline.Margin = UnderlineAllTab
                ? new Thickness(Underline.Margin.Left - Padding.Left, 0, Underline.Margin.Right - Padding.Right, 0)
                : new Thickness(0);
        }

        private void UpdateMargin()
        {
            Underline.Margin = UnderlineAllTab
                ? new Thickness(Underline.Margin.Left - Margin.Left, 0, Underline.Margin.Right - Margin.Right, 0)
                : new Thickness(0);
        }
    }
}