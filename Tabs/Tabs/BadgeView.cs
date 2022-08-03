using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public class BadgeView : Frame
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
#if NET6_0_OR_GREATER
            Colors.White);
#else
            Color.White);
#endif

        public static readonly BindableProperty BadgePaddingProperty = BindableProperty.Create(
            nameof(BadgePadding),
            typeof(Thickness),
            typeof(BadgeView),
            defaultValueCreator: (b) => new Thickness(4, 2));

        public static readonly BindableProperty ShowIndicatorProperty = BindableProperty.Create(
            nameof(ShowIndicator),
            typeof(bool),
            typeof(BadgeView),
            default(bool));

        public BadgeView()
        {
            BatchBegin();

            HasShadow = false;
            Padding = 0;
            Margin = 10;
            CornerRadius = (float)(Padding.VerticalThickness + TextSize) / 2;
#if NET6_0_OR_GREATER
            BackgroundColor = Colors.Red;
#else
            BackgroundColor = Color.Red;
#endif
            HorizontalOptions = LayoutOptions.End;
            VerticalOptions = LayoutOptions.Start;

            var textLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BindingContext = this,
            };

            textLabel.PropertyChanged += OnTextLabelPropertyChanged;
            textLabel.BatchBegin();

            Content = textLabel;

            Update();
            UpdateSize();

            textLabel.BatchCommit();
            BatchCommit();
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

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Padding):
                    if (Padding != new Thickness(0))
                    {
                        throw new NotSupportedException("Use the BadgePadding property instead of Padding to set the text inner margin");
                    }

                    break;

                case nameof(FontFamily):
                case nameof(TextColor):
                case nameof(TextSize):
                case nameof(Text):
                    Update();
                    break;

                case nameof(BadgePadding):
                    UpdateSize();
                    break;

                case nameof(ShowIndicator):
                    UpdateShowIndicator();
                    break;
            }
        }

        private void OnTextLabelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Height):
                case nameof(Width):
                    UpdateSize();
                    break;
            }
        }

        private void Update()
        {
            bool isInt = int.TryParse(Text, out var count);
            bool isEmpty = string.IsNullOrEmpty(Text);
            IsVisible = ShowIndicator || (isInt && count > 0) || (!isInt && !isEmpty);

            BadgeLabel.FontFamily = FontFamily;
            BadgeLabel.Text = Text;
            BadgeLabel.TextColor = TextColor;
            BadgeLabel.FontSize = TextSize;

            BadgeLabel.IsVisible = !ShowIndicator;
        }

        private void UpdateShowIndicator()
        {
            Update();

            if (!ShowIndicator)
            {
                UpdateSize();
                return;
            }

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

            HeightRequest = 6;
            WidthRequest = 6;
            CornerRadius = 3;
        }

        private void UpdateSize()
        {
            if (BadgeLabel.Width <= 0 || BadgeLabel.Height <= 0)
            {
                return;
            }

            if (ShowIndicator)
            {
                UpdateShowIndicator();
                return;
            }

            TranslationX = 0;
            TranslationY = 0;

            double height = BadgeLabel.Height + BadgePadding.VerticalThickness;
            double maxWidth = Math.Max(BadgeLabel.Width + BadgePadding.HorizontalThickness, height);
            HeightRequest = height;
            WidthRequest = maxWidth;

            BadgeLabel.Margin = new Thickness(
                BadgePadding.Left - BadgePadding.Right,
                BadgePadding.Top - BadgePadding.Bottom,
                0,
                0);

            CornerRadius = (int)Math.Round(height / 2);
        }
    }
}
