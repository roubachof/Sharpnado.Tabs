using Sharpnado.Shades.iOS;

namespace Sharpnado.Tabs.iOS
{
    public static class Preserver
    {
        public static void Preserve()
        {
            iOSShadowsRenderer.Initialize();
            iOSTapCommandEffect.Initialize();
            iOSTintableImageEffect.Initialize();
            iOSViewStyleEffect.Initialize();
        }
    }
}