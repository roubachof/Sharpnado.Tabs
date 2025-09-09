using Microsoft.Maui.Animations;
using Microsoft.Maui.Controls.Shapes;

namespace Sharpnado.Tabs;

public class TouchOverlay : BoxView;

public partial class TabHostView
{
    private TapGestureRecognizer _touchGestureRecognizer = new ();

    public static readonly BindableProperty TouchColorProperty = BindableProperty.Create(
        nameof(TouchColor),
        typeof(Color),
        typeof(TabHostView),
        Colors.Transparent,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is not TabHostView tabHostView) return;

            if (newValue is Color color
                && Equals(color, Colors.Transparent))
            {
                tabHostView.ClearTouchEffect();
            }
            else if (!Equals(oldValue, newValue))
            {
                tabHostView.CreateTouchEffect();
            }
        });

    public Color TouchColor
    {
        get => (Color)GetValue(TouchColorProperty);
        set => SetValue(TouchColorProperty, value);
    }

    private void CreateTouchEffect()
    {
        if (Equals(TouchColor, Colors.Transparent)) return;

        foreach (var selectableTab in _selectableTabs)
        {
            AddTouchEffectIfNeeded(selectableTab);
        }
    }

    private void ClearTouchEffect()
    {
        foreach (var selectableTab in _selectableTabs)
        {
            RemoveTouchEffect(selectableTab);
        }
    }

    private void RemoveTouchEffect(TabItem tabItem)
    {
        if (tabItem.Content is Grid grid && grid.Children[0] is TouchOverlay touchOverlay)
        {
            grid.Children.Remove(touchOverlay);
            tabItem.GestureRecognizers.Remove(_touchGestureRecognizer);
        }
    }

    private void AddTouchEffectIfNeeded(TabItem tabItem)
    {
        if (Equals(TouchColor, Colors.Transparent)) return;

        if (tabItem.Content is Grid grid && grid.Children[0] is not TouchOverlay)
        {
            var touchOverlay = new TouchOverlay()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Color = TouchColor,
                Opacity = 0,
            };

            Grid.SetRowSpan(touchOverlay, grid.RowDefinitions.Count);
            Grid.SetColumnSpan(touchOverlay, grid.ColumnDefinitions.Count);

            grid.Children.Insert(0, touchOverlay);

            tabItem.GestureRecognizers.Add(_touchGestureRecognizer);
        }
    }

    private async void OnTouchStarted(object? sender, TappedEventArgs e)
    {
        if (sender is TabItem { Content: Grid grid } && grid.Children[0] is TouchOverlay touchOverlay)
        {
            touchOverlay.Color = TouchColor;
            await touchOverlay.FadeTo(1, 300);
            await touchOverlay.FadeTo(0, 100);
        }
    }

    public void AnimateColor(Color startColor, Color endColor, double durationMs)
    {
        var animation = new Microsoft.Maui.Controls.Animation(
            callback: progress => { TouchColor = startColor.Lerp(endColor, progress); },
            start: 0,
            end: 1);

        animation.Commit(
            owner: this,
            name: "ColorAnimation",
            length: (uint)durationMs,
            easing: Easing.Linear);
    }
}

