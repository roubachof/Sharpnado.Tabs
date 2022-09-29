using Sharpnado.Tabs;
using Sharpnado.TaskLoaderView;

namespace MauiSample.Presentation.Views;

public partial class TabA : ContentView
{
    public TabA()
    {
        InitializeComponent();
        TabHostBadge.SelectedIndex = 0;
        TabHostButton.SelectedIndex = 0;

        TabButton.TapCommand = new TaskLoaderCommand(TakeScreenshot);

    }

    private async Task TakeScreenshot()
    {
        var screenshot = await Screenshot.CaptureAsync();
        var stream = await screenshot.OpenReadAsync();

        var image = new Image
            {
                Margin = new Thickness(0, 20, 0, 0),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Aspect=Aspect.AspectFit,
                Rotation = -90,
                Source = ImageSource.FromStream(() => stream),
            };
        
        ScreenshotContainer.Content = image;
    }
}