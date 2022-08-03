namespace Sharpnado.Tabs
{
    public static class Initializer
    {
        public static void Initialize(bool loggerEnable, bool debugLogEnable)
        {
#if !NET6_0_OR_GREATER
            Shades.Initializer.Initialize(loggerEnable: false);
#endif

            InternalLogger.EnableDebug = debugLogEnable;
            InternalLogger.EnableLogging = loggerEnable;
        }
    }
}
