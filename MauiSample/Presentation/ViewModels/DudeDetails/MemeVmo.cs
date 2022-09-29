namespace MauiSample.Presentation.ViewModels
{
    public class MemeVmo
    {
        public MemeVmo(string memeUrl)
        {
            MemeUrl = memeUrl;
        }

        public string MemeUrl { get; }
    }
}