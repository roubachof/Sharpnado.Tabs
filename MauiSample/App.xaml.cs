using MauiSample.Presentation.ViewModels;

namespace MauiSample;

public partial class App : Application
{
	public App(BottomTabsPageViewModel mainViewModel)
	{
		InitializeComponent();

        Current!.UserAppTheme = AppTheme.Dark;

		MainPage = new MainPage(mainViewModel);
	}
}
