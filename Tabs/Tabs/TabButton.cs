using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Sharpnado.Tabs
{
    public class TabButton : TabItem
    {
        public static readonly BindableProperty IconImageSourceProperty = BindableProperty.Create(
            nameof(IconImageSource),
            typeof(ImageSource),
            typeof(TabButton));

        public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(TabButton));

        public static readonly BindableProperty ButtonBackgroundColorProperty = BindableProperty.Create(
            nameof(ButtonBackgroundColor),
            typeof(Color),
            typeof(TabButton),
            defaultValue: Colors.Transparent);

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius),
            typeof(int),
            typeof(TabButton));

        public static readonly BindableProperty ButtonPaddingProperty = BindableProperty.Create(
            nameof(ButtonPadding),
            typeof(Thickness),
            typeof(TabButton));

        public static readonly BindableProperty ButtonWidthRequestProperty = BindableProperty.Create(
            nameof(ButtonWidthRequest),
            typeof(double),
            typeof(TabButton));

        public static readonly BindableProperty ButtonHeightRequestProperty = BindableProperty.Create(
            nameof(ButtonHeightRequest),
            typeof(double),
            typeof(TabButton));

        public static readonly BindableProperty ButtonCircleSizeProperty = BindableProperty.Create(
            nameof(ButtonCircleSize),
            typeof(double),
            typeof(TabButton));

        private ImageButton _imageButton;

        public TabButton()
        {
            Initialize();
        }

        public event EventHandler Clicked
        {
            add => _imageButton.Clicked += value;
            remove => _imageButton.Clicked -= value;
        }

        public ImageSource IconImageSource
        {
            get => (ImageSource)GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
        }

        public ICommand TapCommand
        {
            get => (ICommand)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
        }

        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Color ButtonBackgroundColor
        {
            get => (Color)GetValue(ButtonBackgroundColorProperty);
            set => SetValue(ButtonBackgroundColorProperty, value);
        }

        public Thickness ButtonPadding
        {
            get => (Thickness)GetValue(ButtonPaddingProperty);
            set => SetValue(ButtonPaddingProperty, value);
        }

        public double ButtonWidthRequest
        {
            get => (double)GetValue(ButtonWidthRequestProperty);
            set => SetValue(ButtonWidthRequestProperty, value);
        }

        public double ButtonHeightRequest
        {
            get => (double)GetValue(ButtonHeightRequestProperty);
            set => SetValue(ButtonHeightRequestProperty, value);
        }

        public double ButtonCircleSize
        {
            get => (double)GetValue(ButtonCircleSizeProperty);
            set => SetValue(ButtonCircleSizeProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(CornerRadius):
                    UpdateCornerRadius();
                    break;

                case nameof(ButtonBackgroundColor):
                    _imageButton.BackgroundColor = ButtonBackgroundColor;
                    break;

                case nameof(IconImageSource):
                    _imageButton.Source = IconImageSource;
                    break;

                case nameof(TapCommand):
                    _imageButton.Command = TapCommand;
                    break;

                case nameof(ButtonPadding):
                    _imageButton.Padding = ButtonPadding;
                    break;

                case nameof(ButtonWidthRequest):
                    UpdateButtonWidthRequest();
                    break;

                case nameof(ButtonHeightRequest):
                    UpdateButtonHeightRequest();
                    break;

                case nameof(ButtonCircleSize):
                    UpdateButtonCircleSize();
                    break;
            }
        }

        protected override void OnBadgeChanged(BadgeView oldBadge)
        {
            throw new NotSupportedException("Badge is not currently supported for TabButton");
        }

        private void UpdateCornerRadius()
        {
            _imageButton.CornerRadius = CornerRadius;
        }

        private void UpdateButtonHeightRequest()
        {
            if (ButtonHeightRequest > 0)
            {
                _imageButton.HeightRequest = ButtonHeightRequest;
                _imageButton.VerticalOptions = LayoutOptions.Center;
            }
            else
            {
                _imageButton.VerticalOptions = LayoutOptions.Fill;
            }
        }

        private void UpdateButtonWidthRequest()
        {
            if (ButtonWidthRequest > 0)
            {
                _imageButton.WidthRequest = ButtonWidthRequest;
                _imageButton.HorizontalOptions = LayoutOptions.Center;
            }
            else
            {
                _imageButton.HorizontalOptions = LayoutOptions.Fill;
            }
        }

        private void UpdateButtonCircleSize()
        {
            if (ButtonCircleSize > 0)
            {
                _imageButton.HeightRequest = ButtonCircleSize;
                _imageButton.WidthRequest = ButtonCircleSize;
                _imageButton.CornerRadius = (int)ButtonCircleSize / 2;

                _imageButton.VerticalOptions = LayoutOptions.Center;
                _imageButton.HorizontalOptions = LayoutOptions.Center;
            }
            else
            {
                _imageButton.VerticalOptions = LayoutOptions.Fill;
                _imageButton.HorizontalOptions = LayoutOptions.Fill;
            }
        }

        private void Initialize()
        {
            _imageButton = new ImageButton
            {
                Padding = 10,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Source = IconImageSource,
                Aspect = Aspect.AspectFit,
                BackgroundColor = Colors.Transparent,
            };

            Content = _imageButton;

            IsSelectable = false;

            _imageButton.BackgroundColor = ButtonBackgroundColor;
            _imageButton.Source = IconImageSource;
            _imageButton.Command = TapCommand;
            _imageButton.Padding = ButtonPadding;

            UpdateCornerRadius();
            UpdateButtonHeightRequest();
            UpdateButtonWidthRequest();
            UpdateButtonCircleSize();
        }
    }
}
