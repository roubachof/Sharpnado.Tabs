using System.Runtime.CompilerServices;
using Sharpnado.Tabs.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageIconUnderlinedTabItem : UnderlinedTabItemBase
    {
        public static readonly BindableProperty IconImageSourceProperty = BindableProperty.Create(
            nameof(IconImageSource),
            typeof(ImageSource),
            typeof(ImageIconUnderlinedTabItem));

        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
            nameof(IconSize),
            typeof(double),
            typeof(ImageIconUnderlinedTabItem),
            defaultValue: 30D);

        public static readonly BindableProperty UnselectedIconColorProperty = BindableProperty.Create(
            nameof(UnselectedIconColor),
            typeof(Color),
            typeof(ImageIconUnderlinedTabItem));

        public static readonly BindableProperty IsTextVisibleProperty = BindableProperty.Create(
            nameof(IsTextVisible),
            typeof(bool),
            typeof(ImageIconUnderlinedTabItem),
            defaultValue: true);

        private readonly bool _isInitialized = false;

        public ImageIconUnderlinedTabItem()
        {
            InitializeComponent();

            InnerLabelImpl.PropertyChanged += InnerLabelPropertyChanged;

            _isInitialized = true;

            LabelSize = 12;

            UpdateTextVisibility();
            UpdateImageColor();
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

        protected override Label InnerLabelImpl => InnerLabel;

        protected override Grid GridImpl => Grid;

        protected override BoxView UnderlineImpl => Underline;

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

                case nameof(UnselectedLabelColor):
                case nameof(UnselectedIconColor):
                case nameof(SelectedTabColor):
                case nameof(IsSelected):
                    UpdateImageColor();
                    break;

                case nameof(IconImageSource):
                    UpdateImageIcon();
                    break;
            }
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
                TextRowDefinition.Height = new GridLength(UnderlineHeight);
                Icon.VerticalOptions = LayoutOptions.Center;
            }
        }

        private void UpdateImageColor()
        {
            ImageEffect.SetTintColor(Icon, IsSelected ? SelectedTabColor : UnselectedIconColor);
        }

        private void UpdateImageIcon()
        {
            Icon.Source = IconImageSource;
        }
    }
}