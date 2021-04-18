using System.Linq;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderlinedTabItem : UnderlinedTabItemBase
    {

        public UnderlinedTabItem()
        {
            InitializeComponent();
        }

        protected override Label InnerLabelImpl => InnerLabel;

        protected override Grid GridImpl => Grid;

        protected override BoxView UnderlineImpl => Underline;
    }
}