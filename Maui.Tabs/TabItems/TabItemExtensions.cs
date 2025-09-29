namespace Sharpnado.Tabs;

public static class TabItemExtensions
{
    public static bool TryGetTouchOverlay(this TabItem tabItem, out TouchOverlay? touchOverlay)
    {
        touchOverlay = null;
        if (tabItem is { Content: Grid grid } && grid.Children[0] is TouchOverlay localTouchOverlay)
        {
            touchOverlay = localTouchOverlay;
            return true;
        }

        return false;
    }
}