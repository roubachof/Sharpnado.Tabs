using Sharpnado.Tabs;
using System.Windows.Input;

namespace MauiSample.Presentation.Views;

public partial class TabI : ContentView
{
    public TabI()
    {
        InitializeComponent();

        string tabsVersion = typeof(TabHostView).Assembly.GetName().Version.ToString();
        Version.Markdown += tabsVersion;
    }
}