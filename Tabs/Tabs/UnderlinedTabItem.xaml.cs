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
                    if (UnderlineAllTab)
                    {
                        Underline.Margin = new Thickness(Underline.Margin.Left - Margin.Left, 0, Underline.Margin.Right - Margin.Right, 0);
                    }
                    else
                    {
                        Underline.Margin = new Thickness(0);
                    }

                    break;

                case nameof(Padding):
                    if (UnderlineAllTab)
                    {
                        Underline.Margin = new Thickness(Underline.Margin.Left - Padding.Left, 0, Underline.Margin.Right - Padding.Right, 0);
                    }
                    else
                    {
                        Underline.Margin = new Thickness(0);
                    }

                    break;

                case nameof(UnderlineAllTab):
                    if (UnderlineAllTab)
                    {
                        Underline.Margin = new Thickness(-Margin.Left - Padding.Left, 0, -Margin.Right - Padding.Right, 0);
                    }
                    else
                    {
                        Underline.Margin = new Thickness(0);
                    }

                    break;
            }
        }
    }
}