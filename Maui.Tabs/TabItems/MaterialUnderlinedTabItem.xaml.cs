using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.Maui.Controls.Shapes;
using Sharpnado.Tabs.Effects;

namespace Sharpnado.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MaterialUnderlinedTabItem : UnderlinedTabItemBase
    {
        public static readonly BindableProperty IconImageSourceProperty = BindableProperty.Create(
            nameof(IconImageSource),
            typeof(ImageSource),
            typeof(MaterialUnderlinedTabItem),
            null);

        public static readonly BindableProperty SelectedIconImageSourceProperty = BindableProperty.Create(
            nameof(SelectedIconImageSource),
            typeof(ImageSource),
            typeof(BottomTabItem));

        public static readonly BindableProperty GeometryIconProperty = BindableProperty.Create(
           nameof(GeometryIcon),
           typeof(Geometry),
           typeof(MaterialUnderlinedTabItem),
           null);

        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
           nameof(IconSize),
           typeof(double),
           typeof(MaterialUnderlinedTabItem),
           defaultValue: 30D);

        public static readonly BindableProperty FillProperty = BindableProperty.Create(
           nameof(Fill),
           typeof(bool),
           typeof(MaterialUnderlinedTabItem),
           true);

        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(
           nameof(StrokeThickness),
           typeof(double),
           typeof(MaterialUnderlinedTabItem),
           0.1);

        public static readonly BindableProperty IconOptionsProperty = BindableProperty.Create(
           nameof(IconOptions),
           typeof(IconOptions),
           typeof(MaterialUnderlinedTabItem),
           IconOptions.TextOnly);

        public static readonly BindableProperty IconTextSpacingProperty = BindableProperty.Create(
            nameof(IconTextSpacing),
            typeof(double),
            typeof(MaterialUnderlinedTabItem),
            2d);

        public MaterialUnderlinedTabItem()
        {
            InitializeComponent();

            MainLayout.PropertyChanged += ContentImplPropertyChanged;
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource IconImageSource
        {
            get => (ImageSource)GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource? SelectedIconImageSource
        {
            get => (ImageSource)GetValue(SelectedIconImageSourceProperty);
            set => SetValue(SelectedIconImageSourceProperty, value);
        }

        [TypeConverter(typeof(PathGeometryConverter))]
        public Geometry GeometryIcon
        {
            get => (Geometry)GetValue(GeometryIconProperty);
            set => SetValue(GeometryIconProperty, value);
        }

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public IconOptions IconOptions
        {
            get => (IconOptions)GetValue(IconOptionsProperty);
            set => SetValue(IconOptionsProperty, value);
        }

        public bool Fill
        {
            get => (bool)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public double IconTextSpacing
        {
            get => (double)GetValue(IconTextSpacingProperty);
            set => SetValue(IconTextSpacingProperty, value);
        }

        protected override Label InnerLabelImpl => InnerLabel;

        protected override Grid GridImpl => Grid;

        protected override BoxView UnderlineImpl => Underline;

        protected override View ContentImpl => MainLayout;

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(UnselectedLabelColor):
                case nameof(SelectedTabColor):
                case nameof(IsSelected):
                case nameof(StrokeThickness):
                case nameof(Fill):
                case nameof(GeometryIcon):
                case nameof(IconImageSource):
                case nameof(SelectedIconImageSource):
                case nameof(IconOptions):
                case nameof(IconTextSpacing):
                    UpdateIconAndTextLayout();
                    break;
            }
        }

        private void UpdateIconAndTextLayout()
        {
            switch (IconOptions)
            {
                case IconOptions.TextOnly:
                    MainLayout.Spacing = 0;
                    InnerLabel.IsVisible = true;
                    ToggleIconVisibility(false);
                    break;

                case IconOptions.IconOnly:
                    MainLayout.Spacing = 0;
                    InnerLabel.IsVisible = false;
                    ToggleIconVisibility(true);
                    break;

                default:
                    ToggleIconVisibility(true);
                    InnerLabel.IsVisible = true;
                    MainLayout.Spacing = IconTextSpacing;
                    MainLayout.Orientation = IconOptions == IconOptions.TopIcon
                        ? StackOrientation.Vertical
                        : StackOrientation.Horizontal;
                    break;
            }
        }

        private bool IsGeometryPreferred() => IconPath != null && IconPath.Data != null;

        private void ToggleIconVisibility(bool visible)
        {
            IconPath.IsVisible = IsGeometryPreferred() && visible;
            IconImage.IsVisible = !IsGeometryPreferred() && visible;

            if (IconPath.IsVisible)
            {
                UpdateGeometryIcon();
            }
            else if (IconImage.IsVisible)
            {
                UpdateImageIcon();
            }
        }

        private void UpdateGeometryIcon()
        {
            var brush = new SolidColorBrush(IsSelected ? SelectedTabColor : UnselectedLabelColor);
            if (Fill)
            {
                IconPath.Fill = brush;
            }

            IconPath.Stroke = brush;
            IconPath.StrokeThickness = StrokeThickness;
        }

        private void UpdateImageIcon()
        {
            IconImage.Source = IconImageSource;

            if (!IsSelectable) return;

            if (SelectedIconImageSource != null)
            {
                IconImage.Source = IsSelected ? SelectedIconImageSource : IconImageSource;
            }
            else
            {
                IconImage.Source = IconImageSource;
                ImageEffect.SetTintColor(IconImage, IsSelected ? SelectedTabColor : UnselectedLabelColor);
            }
        }
    }
}