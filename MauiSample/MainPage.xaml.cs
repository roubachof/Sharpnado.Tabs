using MauiSample.Presentation.ViewModels;

namespace MauiSample;

public partial class MainPage : ContentPage
{
	public MainPage(BottomTabsPageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }

    private BottomTabsPageViewModel ViewModel => (BottomTabsPageViewModel)BindingContext;
}

