using System.Windows.Input;

namespace MauiSample.Presentation.Views;

public partial class TabU : ContentView
{
    private int _tapCount = 0;

    public TabU()
    {
        InitializeComponent();
    }

    public ICommand TapMeCommand =>
        new Command(() => TapMeLabel.Text = _tapCount++ % 2 == 1 ? "Wait, what?" : "Stop that!");
}