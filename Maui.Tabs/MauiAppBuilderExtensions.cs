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
            x.Add<CommandsRoutingEffect, Sharpnado.Tabs.Effects.Droid.CommandsPlatform>();
            x.Add<TouchRoutingEffect, Sharpnado.Tabs.Effects.Droid.TouchEffectPlatform>();
            x.Add<TintableImageEffect, Sharpnado.Tabs.Droid.AndroidTintableImageEffect>();
#elif IOS
            x.Add<CommandsRoutingEffect, Sharpnado.Tabs.Effects.iOS.CommandsPlatform>();
            x.Add<TouchRoutingEffect, Sharpnado.Tabs.Effects.iOS.TouchEffectPlatform>();
            x.Add<TintableImageEffect, Sharpnado.Tabs.iOS.iOSTintableImageEffect>();
#endif
        });

#if IOS
        Sharpnado.Tabs.Effects.iOS.CommandsPlatform.Init();
        Sharpnado.Tabs.Effects.iOS.TouchEffectPlatform.Init();
        Sharpnado.Tabs.iOS.iOSTintableImageEffect.Init();
#endif

        return builder;
    }
}
