using System.Diagnostics;

namespace MauiSample.Infrastructure
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:Element should begin with upper-case letter",
        Justification = "iOS name is an exception")]
    public enum OS
    {
        Android = 1,
        iOS = 2,
    }

    public enum ScreenSize
    {
        Regular = 0,
        Small = 1,
        Big = 2,
    }

    public static class PlatformService
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();

        private static string stopWatchMessage;

        private static Func<Thickness> safeAreaGetter;

        public static double DisplayScaleFactor { get; private set; }

        public static Size MainSize { get; private set; }

        public static bool IsFoldingScreen { get; private set; }

        public static bool IsEmulator { get; private set; }

        public static ScreenSize ScreenSize
        {
            get
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (MainSize.Width <= 320)
                    {
                        return ScreenSize.Small;
                    }

                    if (MainSize.Width <= 375)
                    {
                        return ScreenSize.Regular;
                    }

                    return ScreenSize.Big;
                }

                // Android
                if (MainSize.Width <= 384)
                {
                    return ScreenSize.Small;
                }

                if (MainSize.Width <= 540)
                {
                    return ScreenSize.Regular;
                }

                return ScreenSize.Big;
            }
        }

        public static void Initialize(bool isEmulator, double scaleFactor, double width, double height, Func<Thickness> safeAreaGetter = null)
        {
            IsEmulator = isEmulator;
            DisplayScaleFactor = scaleFactor;

            if (width > height)
            {
                var temp = width;
                width = height;
                height = temp;
            }

            MainSize = new Size(width, height);
            PlatformService.safeAreaGetter = safeAreaGetter;
        }

        public static void InitializeFoldingScreen(bool isFoldingScreen)
        {
            IsFoldingScreen = isFoldingScreen;
        }

        /// <summary>
        /// Call this when you are sure the UI is Idled to clean up resources.
        /// </summary>
        public static void ForceGarbageCollector()
        {
            // This code come is from Shane Neuville of Xamarin.
            // Yeah I know this seems like an absurd numbers of calls...
            // I tested it, and in fact, in the most cases, you DO need all
            // those calls for the GC to REALLY reclaim resources (especially
            // with Android RecyclerView).
            Device.BeginInvokeOnMainThread(async () =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                await Task.Delay(500);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            });
        }

        public static Thickness GetSafeArea() => safeAreaGetter?.Invoke() ?? new Thickness(0);

        public static void StartWatch(string message)
        {
            stopWatchMessage = message;
            Stopwatch.Start();
        }

        public static TimeSpan StopWatch()
        {
            Stopwatch.Stop();
            var result = Stopwatch.Elapsed;
            Stopwatch.Reset();
            return result;
        }

        public static void StopWatchAndDisplay()
        {
            var result = StopWatch();
            Trace.WriteLine($"{stopWatchMessage} completed in {result.TotalMilliseconds:000} ms");
        }

        public static int DpToPixels(int dp) => (int)(DisplayScaleFactor * dp);

        public static int DpToPixels(double dp) => (int)(DisplayScaleFactor * dp);
    }
}