using Microsoft.Maui.Animations;
using Microsoft.Maui.Controls.Shapes;

namespace Sharpnado.Tabs;

public class TouchOverlay : BoxView;

public enum TouchEffectType
{
    None = 0,
    PoorsManRipple = 1,
    Standard = 2,
}

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

            if (newValue is Color
                && !tabHostView.HasTouchEffect)
            {
                tabHostView.ClearTouchEffect();
            }
            else if (!Equals(oldValue, newValue) && tabHostView.HasTouchEffect)
            {
                tabHostView.CreateTouchEffect();
            }
        });

    public static readonly BindableProperty TouchEffectTypeProperty = BindableProperty.Create(
        nameof(TouchColor),
        typeof(TouchEffectType),
        typeof(TabHostView),
        TouchEffectType.None,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is not TabHostView tabHostView) return;

            if (newValue is TouchEffectType && tabHostView.HasTouchEffect)
            {
                tabHostView.ClearTouchEffect();
            }
            else if (!Equals(oldValue, newValue) && tabHostView.HasTouchEffect)
            {
                tabHostView.CreateTouchEffect();
            }
        });

    public Color TouchColor
    {
        get => (Color)GetValue(TouchColorProperty);
        set => SetValue(TouchColorProperty, value);
    }

    public TouchEffectType TouchEffectType
    {
        get => (TouchEffectType)GetValue(TouchEffectTypeProperty);
        set => SetValue(TouchEffectTypeProperty, value);
    }

    public bool HasTouchEffect => TouchEffectType != TouchEffectType.None && !Equals(TouchColor, Colors.Transparent);

    private void CreateTouchEffect()
    {
        if (!HasTouchEffect) return;

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
        if (!HasTouchEffect) return;

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
        if (sender is TabItem { Content: Grid grid } tabItem && grid.Children[0] is TouchOverlay touchOverlay)
        {
            switch (TouchEffectType)
            {
                case TouchEffectType.PoorsManRipple:
                    await PoorsManRipple(tabItem, touchOverlay);
                    break;
                case TouchEffectType.Standard:
                    await Standard(tabItem, touchOverlay);
                    break;
            }
        }
    }

    private async Task PoorsManRipple(TabItem tabItem, TouchOverlay touchOverlay)
    {
        tabItem.Clip = new RectangleGeometry(Rect.FromLTRB(0, 0, tabItem.Width, tabItem.Height));
        touchOverlay.Color = TouchColor.WithAlpha(0.5f);
        touchOverlay.Scale = 0.2;
        touchOverlay.CornerRadius = new CornerRadius(30);

        await Task.WhenAll(
            Blink(touchOverlay, 200),
            touchOverlay.ScaleTo(1.5, 200, Easing.CubicIn));
    }

    private async Task Standard(TabItem tabItem, TouchOverlay touchOverlay)
    {
        tabItem.Clip = null;
        touchOverlay.Color = TouchColor;

        await Blink(touchOverlay, 200);
    }

    private static async Task Blink(TouchOverlay touchOverlay, uint delay)
    {
        await touchOverlay.FadeTo(1, delay);
        await touchOverlay.FadeTo(0, delay / 2);
    }
}

