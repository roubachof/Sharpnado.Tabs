using ABI.Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Controls.Platform;
using Sharpnado.Tabs.Effects;
using Image = Microsoft.UI.Xaml.Controls.Image;

namespace Sharpnado.Tabs.Dos;

public class WindowsTintableImageEffect : PlatformEffect
{
    protected override void OnAttached()
    {
        UpdateColor();
    }

    protected override void OnDetached()
    {
        if (Control is Image image)
        {
            //TODO: restore original image
        }
    }

    protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if ((Element is Microsoft.Maui.Controls.Image) && args.PropertyName == Microsoft.Maui.Controls.Image.SourceProperty.PropertyName)
        {
            UpdateColor();
        }
    }

    private Microsoft.UI.Xaml.Media.ImageSource? _untintedSource;

    private async void UpdateColor()
    {
        var effect = (TintableImageEffect)Element.Effects.FirstOrDefault(x => x is TintableImageEffect);
        var color = effect?.TintColor?.ToWindowsColor();

        if (Control is Image image && image.Source != null && color != null)
        {
            //TODO: implement bitmap tinting
        }
    }
}

public static class ColorExtensions
{
    public static Windows.UI.Color? ToWindowsColor(this Color color)
    {
        if (color == null)
            return null;

        return Windows.UI.Color.FromArgb(
            (byte)(color.Alpha * 255),
            (byte)(color.Red * 255),
            (byte)(color.Green * 255),
            (byte)(color.Blue * 255));
    }
}