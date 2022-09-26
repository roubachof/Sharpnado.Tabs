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

#if __IOS__
        XamEffects.iOS.CommandsPlatform.Init();
        XamEffects.iOS.TouchEffectPlatform.Init();
        Sharpnado.Tabs.iOS.iOSTintableImageEffect.Init();
#endif

        return builder;
    }
}
