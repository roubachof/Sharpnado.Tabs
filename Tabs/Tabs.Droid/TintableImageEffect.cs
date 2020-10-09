using System;
using System.Linq;

using Android.Widget;

using Sharpnado.Tabs.Droid;
using Sharpnado.Tabs.Effects;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Sharpnado")]
[assembly: ExportEffect(typeof(AndroidTintableImageEffect), nameof(TintableImageEffect))]

namespace Sharpnado.Tabs.Droid
{
    [Preserve]
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
            var color = effect?.TintColor.ToAndroid();

            if (Control is ImageView imageView && imageView.Handle != IntPtr.Zero && color.HasValue)
            {
                Android.Graphics.Color tint = color.Value;

                imageView.SetColorFilter(tint);
            }
        }
    }
}