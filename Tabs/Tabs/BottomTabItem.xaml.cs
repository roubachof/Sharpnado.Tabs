using System.Runtime.CompilerServices;

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

        private readonly bool _isInitialized = false;

        public BottomTabItem()
        {
            InitializeComponent();

            _isInitialized = true;

            LabelSize = 12;

            UpdateTextVisibility();
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
            }
        }

        protected override void OnBadgeChanged(BadgeView oldBadge)
        {
            if (oldBadge != null)
            {
                Grid.Children.Remove(oldBadge);
                return;
            }

            Grid.SetRow(Badge, 0);
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
    }
}