// https://github.com/shrutinambiar/xamarin-forms-tinted-image/blob/master/src/Plugin.CrossPlatformTintedImage.UWP/TintedImageRenderer.cs
// by https://github.com/shrutinambiar
// MIT License
//
// Copyright (c) 2016 shrutinambiar
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

using CompositionProToolkit;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

using Sharpnado.Tabs.Effects;
using Sharpnado.Tabs.Uwp;
using Sharpnado.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

using Color = System.Drawing.Color;
using Image = Windows.UI.Xaml.Controls.Image;
using Size = Windows.Foundation.Size;

[assembly: ResolutionGroupName("Sharpnado")]
[assembly: ExportEffect(typeof(UwpTintableImageEffect), nameof(TintableImageEffect))]

namespace Sharpnado.Tabs.Uwp
{
    [Preserve]
    public class UwpTintableImageEffect : PlatformEffect
    {
        private CompositionEffectBrush _effectBrush;
        private SpriteVisual _spriteVisual;
        private IImageSurface _imageSurface;
        private Compositor _compositor;
        private ICompositionGenerator _generator;

        public new Xamarin.Forms.Image Element => (Xamarin.Forms.Image)base.Element;

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

            if ((Element != null) && args.PropertyName == Xamarin.Forms.Image.SourceProperty.PropertyName)
            {
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (!(Control is Image nativeImage))
            {
                return;
            }

            TaskMonitor.Create(CreateTintEffectBrushAsync(nativeImage));
        }

        private async Task CreateTintEffectBrushAsync(Image nativeImage)
        {
            if (Control == null || Element == null || Element.Width <= 0 || Element.Height <= 0)
            {
                return;
            }

            var uri = new Uri($"ms-appx:///{((FileImageSource)Element.Source).File}");

            var effect = (TintableImageEffect)Element.Effects.FirstOrDefault(x => x is TintableImageEffect);

            var color = effect?.TintColor;
            if (color == null)
            {
                return;
            }

            var nativeColor = GetNativeColor(color.Value);

            SetupCompositor();

            _spriteVisual = _compositor.CreateSpriteVisual();
            _spriteVisual.Size = new Vector2((float)Element.Width, (float)Element.Height);

            _imageSurface = await _generator.CreateImageSurfaceAsync(
                uri,
                new Size(Element.Width, Element.Height),
                ImageSurfaceOptions.DefaultOptimized);

            CompositionSurfaceBrush surfaceBrush = _compositor.CreateSurfaceBrush(_imageSurface.Surface);
            CompositionBrush targetBrush = surfaceBrush;

            if (color == Color.Transparent)
            {
                // Don't apply tint effect
                _effectBrush = null;
            }
            else
            {
                // Set target brush to tint effect brush
                IGraphicsEffect graphicsEffect = new CompositeEffect
                {
                    Mode = CanvasComposite.DestinationIn,
                    Sources =
                    {
                        new ColorSourceEffect
                        {
                            Name = "colorSource",
                            Color = nativeColor,
                        },
                        new CompositionEffectSourceParameter("mask"),
                    },
                };

                CompositionEffectFactory effectFactory = _compositor.CreateEffectFactory(
                    graphicsEffect,
                    new[] { "colorSource.Color" });

                _effectBrush = effectFactory.CreateBrush();
                _effectBrush.SetSourceParameter("mask", surfaceBrush);

                SetTint(nativeColor);

                targetBrush = _effectBrush;
            }

            _spriteVisual.Brush = targetBrush;
            ElementCompositionPreview.SetElementChildVisual(Control, _spriteVisual);
        }

        private static Windows.UI.Color GetNativeColor(Color color)
        {
            return Windows.UI.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
        }

        private void SetTint(Windows.UI.Color color)
        {
            _effectBrush?.Properties.InsertColor("colorSource.Color", color);
        }

        private void SetupCompositor()
        {
            if (_compositor != null && _generator != null)
            {
                return;
            }

            _compositor = ElementCompositionPreview.GetElementVisual(Control).Compositor;
            _generator = _compositor.CreateCompositionGenerator();
        }
    }
}