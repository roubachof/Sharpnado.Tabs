using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Color = Windows.UI.Color;
using Image = Microsoft.UI.Xaml.Controls.Image;

public static class ImageTintHelper
{
    public static async Task<WriteableBitmap> TintImageAsync(Image image, Color tint)
    {
        if (image.Source is not BitmapImage bitmapImage)
            throw new InvalidOperationException("Image.Source must be a BitmapImage.");

        // Get the URI of the image (only works if Source was set with a Uri!)
        if (bitmapImage.UriSource is null)
            throw new InvalidOperationException("BitmapImage.UriSource is null. Cannot reload.");

        var streamRef = RandomAccessStreamReference.CreateFromUri(bitmapImage.UriSource);
        using var stream = await streamRef.OpenReadAsync();
        var writeable = new WriteableBitmap(1, 1);
        await writeable.SetSourceAsync(stream);

        await using (var buffer = writeable.PixelBuffer.AsStream())
        {
            var pixels = new byte[buffer.Length];
            await buffer.ReadExactlyAsync(pixels, 0, pixels.Length);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                // BGRA
                pixels[i + 0] = (byte)(pixels[i + 0] * tint.B / 255);
                pixels[i + 1] = (byte)(pixels[i + 1] * tint.G / 255);
                pixels[i + 2] = (byte)(pixels[i + 2] * tint.R / 255);
                // alpha channel unchanged
            }

            buffer.Seek(0, SeekOrigin.Begin);
            await buffer.WriteAsync(pixels);
        }

        // Replace the Image.Source with tinted bitmap
        return writeable;
    }
}