using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeometryIconUnderlinedTabItem : UnderlinedTabItemBase
    {
        public static readonly BindableProperty GeometryIconProperty = BindableProperty.Create(
           nameof(GeometryIcon),
           typeof(Geometry),
           typeof(TabTextItem),
           null);

        public static readonly BindableProperty FillProperty = BindableProperty.Create(
           nameof(Fill),
           typeof(bool),
           typeof(TabTextItem),
           false);

        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(
           nameof(StrokeThickness),
           typeof(double),
           typeof(TabTextItem),
           1.0);

        public GeometryIconUnderlinedTabItem()
        {
            InitializeComponent();

            InnerLabelImpl.PropertyChanged += InnerLabelPropertyChanged;
        }

        [TypeConverter(typeof(PathGeometryConverter))]
        public Geometry GeometryIcon
        {
            get => (Geometry)GetValue(GeometryIconProperty);
            set => SetValue(GeometryIconProperty, value);
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
                    UpdateGeometryIcon();
                    break;
            }
        }

        private void UpdateGeometryIcon()
        {
            var brush = new SolidColorBrush(IsSelected ? SelectedTabColor : UnselectedLabelColor);
            if (Fill)
            {
                IconPath.Fill = brush;
                IconPath.Stroke = null;
            }
            else
            {
                IconPath.Fill = null;
                IconPath.Stroke = brush;
                IconPath.StrokeThickness = StrokeThickness;
            }
        }
    }
}