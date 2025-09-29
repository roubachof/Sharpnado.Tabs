using MauiSample.Domain.Silly;
using MauiSample.Infrastructure;
using MauiSample.Presentation.Navigables;
using MauiSample.Presentation.Navigables.Impl;
using MauiSample.Presentation.ViewModels;
using Sharpnado.Tabs;
using Sharpnado.TaskLoaderView;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MauiSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseSharpnadoTabs(loggerEnable: true, debugLogEnable: true)
			.ConfigureTaskLoader(true, debugLogEnable: true)
			.UseSkiaSharp()
			.ConfigureFonts(fonts =>
			{
                fonts.AddFont("OpenSans-Bold.ttf", "OpenSansBold");
                fonts.AddFont("OpenSans-ExtraBold.ttf", "OpenSansExtraBold");
                fonts.AddFont("OpenSans-Light.ttf", "OpenSansLight");
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton(new ErrorEmulator());
        builder.Services.AddSingleton<INavigationService, FormsNavigationService>();
        builder.Services.AddSingleton<ISillyDudeService, SillyDudeService>();
        builder.Services.AddSingleton<MainPage>();

        builder.Services.AddSingleton<BottomTabsPageViewModel>();

		return builder.Build();
	}
}
