using System.Runtime.CompilerServices;
using Sharpnado.Tabs.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

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
           IconOptions.TopIcon);

        public MaterialUnderlinedTabItem()
        {
            InitializeComponent();

            InnerLabelImpl.PropertyChanged += InnerLabelPropertyChanged;
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource IconImageSource
        {
            get => (ImageSource)GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
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

        protected override Label InnerLabelImpl => InnerLabel;

        protected override Grid GridImpl => Grid;

        protected override BoxView UnderlineImpl => Underline;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                case nameof(IconOptions):
                    UpdateIconAndTextLayout();
                    break;
            }
        }

        private void UpdateIconAndTextLayout()
        {
            if (IconOptions == IconOptions.TextOnly)
            {
                MainLayout.Spacing = 0;
                InnerLabel.IsVisible = true;
                ToggleIconVisibility(false);
            }
            else if (IconOptions == IconOptions.IconOnly)
            {
                MainLayout.Spacing = 0;
                InnerLabel.IsVisible = false;
                ToggleIconVisibility(true);
            }
            else
            {
                MainLayout.Spacing = 9;
                ToggleIconVisibility(true);
                InnerLabel.IsVisible = true;
                MainLayout.Orientation = IconOptions == IconOptions.TopIcon
                    ? StackOrientation.Vertical : StackOrientation.Horizontal;
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
            ImageEffect.SetTintColor(IconImage, IsSelected ? SelectedTabColor : UnselectedLabelColor);
        }
    }
}