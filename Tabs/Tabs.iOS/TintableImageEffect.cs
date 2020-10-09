using System;
using System.Linq;
using System.Threading.Tasks;

using Foundation;

using Sharpnado.Tabs.Effects;
using Sharpnado.Tabs.iOS;
using Sharpnado.Tasks;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Sharpnado")]
[assembly: ExportEffect(typeof(iOSTintableImageEffect), nameof(TintableImageEffect))]

namespace Sharpnado.Tabs.iOS
{
    [Preserve]
    public class iOSTintableImageEffect : PlatformEffect
    {
        private int _tintAttempts = 0;
        private bool _isAttached = false;

        public static void Initialize()
        {
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if ((Element is Image) && args.PropertyName == Image.SourceProperty.PropertyName)
            {
                _tintAttempts = 0;
                UpdateColor();
            }
        }

        protected override void OnAttached()
        {
            _tintAttempts = 0;
            _isAttached = true;
            UpdateColor();
        }

        protected override void OnDetached()
        {
            _isAttached = false;
            _tintAttempts = 0;
            if (Control is UIImageView imageView && imageView.Image != null)
            {
                imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.Automatic);
            }
        }

        private void UpdateColor()
        {
            if (!_isAttached || Control == null || Element == null)
            {
                return;
            }

            var imageView = (UIImageView)Control;
            var effect = (TintableImageEffect)Element.Effects.FirstOrDefault(x => x is TintableImageEffect);

            var color = effect?.TintColor.ToUIColor();
            if (color == null)
            {
                return;
            }

            Control.TintColor = color;

            if (imageView?.Image == null)
            {
                if (_tintAttempts < 5)
                {
                    TaskMonitor.Create(() => DelayedPost(500, UpdateColor));
                }

                return;
            }

            _tintAttempts = 0;
            imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }

        private async Task DelayedPost(int milliseconds, Action action)
        {
            await Task.Delay(milliseconds);
            Device.BeginInvokeOnMainThread(action);
        }
    }
}
