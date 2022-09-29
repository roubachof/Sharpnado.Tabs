using MauiSample.Presentation.Navigables;

namespace SillyCompany.Mobile.Practices.Presentation.Views
{
    public class SillyContentPage : ContentPage, IBindablePage
    {
        public SillyContentPage()
        {
            Padding = 0;
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
