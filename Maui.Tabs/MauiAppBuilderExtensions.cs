using Sharpnado.Tabs.Effects;


namespace Sharpnado.Tabs;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseSharpnadoTabs(
        this MauiAppBuilder builder,
        bool loggerEnable,
        bool debugLogEnable = false)
    {
        InternalLogger.EnableDebug = debugLogEnable;
        InternalLogger.EnableLogging = loggerEnable;

        builder.ConfigureEffects(x =>
        {
#if ANDROID
            x.Add<TintableImageEffect, Sharpnado.Tabs.Droid.AndroidTintableImageEffect>();
#elif IOS
            x.Add<TintableImageEffect, Sharpnado.Tabs.iOS.iOSTintableImageEffect>();
#elif WINDOWS
            // TODO: implement Windows effect
            // x.Add<TintableImageEffect, Sharpnado.Tabs.Dos.WindowsTintableImageEffect>();
#endif
        });

#if IOS
        Sharpnado.Tabs.iOS.iOSTintableImageEffect.Init();
#endif

        return builder;
    }
}
