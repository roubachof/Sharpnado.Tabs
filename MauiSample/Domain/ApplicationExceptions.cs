using MauiSample.Localization;

namespace MauiSample.Domain
{
    public static class ApplicationExceptions
    {
        public static string ToString(Exception exception)
        {
            switch (exception)
            {
                case ServerException serverException:
                    return SillyResources.Error_Business;
                case NetworkException networkException:
                    return SillyResources.Error_Network;
                default:
                    return SillyResources.Error_Unknown;
            }
        }
    }

    public class ServerException : Exception
    {
    }

    public class NetworkException : Exception
    {
    }
}
