namespace Sharpnado.Tabs
{
    public static class Initializer
    {
        public static void Initialize(bool loggerEnable, bool debugLogEnable)
        {
            Shades.Initializer.Initialize(loggerEnable: false);

            InternalLogger.EnableDebug = debugLogEnable;
            InternalLogger.EnableLogging = loggerEnable;
        }
    }
}
