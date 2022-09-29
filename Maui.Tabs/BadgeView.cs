using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.Maui.Controls.Shapes;

namespace Sharpnado.Tabs
{
    public class BadgeView : Border
    {
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            nameof(FontFamily),
            typeof(string),
            typeof(TabTextItem),
            null,
            BindingMode.OneWay);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(TabTextItem),
            string.Empty);

        public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(
            nameof(TextSize),
            typeof(double),
            typeof(TabTextItem),
            10d);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(TabTextItem),
            Colors.White);

        public static readonly BindableProperty BadgePaddingProperty = BindableProperty.Create(
            nameof(BadgePadding),
            typeof(Thickness),
            typeof(BadgeView),
            defaultValueCreator: (b) => new Thickness(6, 2));

        public static readonly BindableProperty ShowIndicatorProperty = BindableProperty.Create(
            nameof(ShowIndicator),
            typeof(bool),
            typeof(BadgeView),
            default(bool));

        private bool _paddingInternalChange;

        public BadgeView()
        {
            _paddingInternalChange = true;
            Padding = new Thickness(6, 2);
            Margin = new Thickness(0, 4, 0, 0);

            BackgroundColor = Colors.Red;
            HorizontalOptions = LayoutOptions.End;
            VerticalOptions = LayoutOptions.Start;

            var textLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, -1.5, 0, 0),
                BindingContext = this,
            };

            Content = textLabel;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public Thickness BadgePadding
        {
            get => (Thickness)GetValue(BadgePaddingProperty);
            set => SetValue(BadgePaddingProperty, value);
        }

        public bool ShowIndicator
        {
            get => (bool)GetValue(ShowIndicatorProperty);
            set => SetValue(ShowIndicatorProperty, value);
        }

        private Label BadgeLabel => (Label)Content;

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            Update();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Padding):
                    if (!_paddingInternalChange && Padding != new Thickness(0))
                    {
                        throw new NotSupportedException("Use the BadgePadding property instead of Padding to set the text inner margin");
                    }

                    _paddingInternalChange = false;
                    break;

                case nameof(BadgePadding):
                case nameof(ShowIndicator):
                case nameof(FontFamily):
                case nameof(TextColor):
                case nameof(TextSize):
                case nameof(Text):
                    Update();
                    break;

                case nameof(Height):
                    UpdateCornerRadius();
                    break;
            }
        }

        private void UpdateCornerRadius()
        {
            if (Height < 1)
            {
                return;
            }

            double cornerRadius = Height / 2;
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(cornerRadius) };
        }

        private void Update()
        {
            InternalLogger.Debug($"Update text: {BadgeLabel.Text}, showIndicator: {ShowIndicator}");

            bool isInt = int.TryParse(Text, out var count);
            bool isEmpty = string.IsNullOrEmpty(Text);
            IsVisible = ShowIndicator || (isInt && count > 0) || (!isInt && !isEmpty);

            BadgeLabel.FontFamily = FontFamily;
            BadgeLabel.Text = Text;
            BadgeLabel.TextColor = TextColor;
            BadgeLabel.FontSize = TextSize;

            BadgeLabel.IsVisible = !ShowIndicator;

            if (ShowIndicator)
            {
                double margin = (TextSize + BadgePadding.VerticalThickness) / 2;

                TranslationX = HorizontalOptions.Alignment switch
                    {
                        LayoutAlignment.Start => margin,
                        LayoutAlignment.End => -margin,
                        _ => 0,
                    };

                TranslationY = VerticalOptions.Alignment switch
                    {
                        LayoutAlignment.Start => margin,
                        LayoutAlignment.End => -margin,
                        _ => 0,
                    };

                HeightRequest = 10;
                WidthRequest = 10;
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) };
                return;
            }
            else
            {
                TranslationX = 0;
                TranslationY = 0;
                WidthRequest = -1;
                HeightRequest = -1;
                UpdateCornerRadius();
            }

            _paddingInternalChange = true;
            Padding = BadgePadding;
        }
    }
}
