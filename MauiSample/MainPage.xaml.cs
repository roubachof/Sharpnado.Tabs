using Android.Widget;
using MauiSample.Presentation.ViewModels;

namespace MauiSample;

public partial class MainPage : ContentPage
{
	public MainPage(BottomTabsPageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
        // Switcher.SelectedIndex = 3;
    }

    private BottomTabsPageViewModel ViewModel => (BottomTabsPageViewModel)BindingContext;
}

