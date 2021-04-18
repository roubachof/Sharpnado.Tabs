using System.Linq;
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
                case nameof(GeometryIcon):
                    UpdateGeometryIcon();
                    break;
            }
        }

        private void UpdateGeometryIcon()
        {
            if (IconPath != null)
            {
                IconPath.Data = GeometryIcon;
                IconPath.Fill = new SolidColorBrush(IsSelected ? SelectedTabColor : UnselectedLabelColor);
            }
        }
    }
}