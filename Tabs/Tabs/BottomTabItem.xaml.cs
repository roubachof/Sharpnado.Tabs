using System.ComponentModel;
using System.Runtime.CompilerServices;

using Sharpnado.Tabs.Effects;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomTabItem : TabTextItem
    {
        public static readonly BindableProperty IconImageSourceProperty = BindableProperty.Create(
            nameof(IconImageSource),
            typeof(ImageSource),
            typeof(BottomTabItem));

        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
            nameof(IconSize),
            typeof(double),
            typeof(BottomTabItem),
            defaultValue: 30D);

        public static readonly BindableProperty UnselectedIconColorProperty = BindableProperty.Create(
            nameof(UnselectedIconColor),
            typeof(Color),
            typeof(BottomTabItem));

        public static readonly BindableProperty IsTextVisibleProperty = BindableProperty.Create(
            nameof(IsTextVisible),
            typeof(bool),
            typeof(BottomTabItem),
            defaultValue: true);

        public static readonly BindableProperty SelectedTabTextColorProperty = BindableProperty.Create(
            propertyName: nameof(SelectedTabTextColor),
            returnType: typeof(Color),
            declaringType: typeof(TabTextItem),
            defaultValue: null);

        private readonly bool _isInitialized = false;

        public BottomTabItem()
        {
            InitializeComponent();

            _isInitialized = true;

            UpdateTextVisibility();
            UpdateIconVisibility();
            UpdateColors();
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource IconImageSource
        {
            get => (ImageSource)GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
        }

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public Color UnselectedIconColor
        {
            get => (Color)GetValue(UnselectedIconColorProperty);
            set => SetValue(UnselectedIconColorProperty, value);
        }

        public bool IsTextVisible
        {
            get => (bool)GetValue(IsTextVisibleProperty);
            set => SetValue(IsTextVisibleProperty, value);
        }

        public Color SelectedTabTextColor
        {
            get => (Color)GetValue(SelectedTabTextColorProperty);
            set => SetValue(SelectedTabTextColorProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (!_isInitialized)
            {
                return;
            }

            switch (propertyName)
            {
                case nameof(IsTextVisible):
                    UpdateTextVisibility();
                    break;

                case nameof(IconImageSource):
                    UpdateIconVisibility();
                    break;

                case nameof(IsSelectable):
                case nameof(UnselectedLabelColor):
                case nameof(UnselectedIconColor):
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
                Grid.Children.Remove(oldBadge);
                return;
            }

            Grid.SetRow((BindableObject)Badge, 0);
            Grid.SetRowSpan((BindableObject)Badge, 2);
            Grid.Children.Add(Badge);
        }

        private void UpdateTextVisibility()
        {
            if (IsTextVisible)
            {
                TextRowDefinition.Height = new GridLength(5, GridUnitType.Star);
                Icon.VerticalOptions = LayoutOptions.End;
            }
            else
            {
                TextRowDefinition.Height = new GridLength(0);
                Icon.VerticalOptions = LayoutOptions.Center;
            }
        }

        private void UpdateIconVisibility()
        {
            if (IconImageSource != null)
            {
                IconRowDefinition.Height = new GridLength(8, GridUnitType.Star);
                IconText.VerticalOptions = LayoutOptions.End;
            }
            else
            {
                IconRowDefinition.Height = new GridLength(0);
                IconText.VerticalOptions = LayoutOptions.Center;
            }
        }

        private void UpdateColors()
        {
            IconText.TextColor = IsSelectable
                ? IsSelected ? SelectedTabTextColor ?? SelectedTabColor : UnselectedLabelColor
                : DisabledLabelColor;
            ImageEffect.SetTintColor(Icon, IsSelected ? SelectedTabColor : UnselectedIconColor);
        }
    }
}
