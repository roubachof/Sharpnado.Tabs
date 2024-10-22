using Android.Widget;

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;

using Sharpnado.Tabs.Effects;

namespace Sharpnado.Tabs.Droid
{
    public class AndroidTintableImageEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateColor();
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if ((Element is Image) && args.PropertyName == Image.SourceProperty.PropertyName)
            {
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            var effect =
                (TintableImageEffect)Element.Effects.FirstOrDefault(x => x is TintableImageEffect);
            var color = effect?.TintColor?.ToPlatform();

            if (Control is ImageView imageView && imageView.Handle != IntPtr.Zero && color.HasValue)
            {
                Android.Graphics.Color tint = color.Value;

                imageView.SetColorFilter(tint);
            }
        }
    }
}
