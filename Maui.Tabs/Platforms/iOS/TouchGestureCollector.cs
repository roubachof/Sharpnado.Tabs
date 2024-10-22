using Sharpnado.Tabs.Effects.iOS.GestureRecognizers;

using UIKit;

namespace Sharpnado.Tabs.Effects.iOS.GestureCollectors;

internal enum ActionType
{
    Color = 0,
    Tap = 1,
}

internal static class TouchGestureCollector
{
    private static Dictionary<UIView, GestureActionsContainer> Collection { get; } = [];

    public static void Add(UIView view, Action<TouchGestureRecognizer.TouchArgs> action, ActionType actionType)
    {
        if (!Collection.TryGetValue(view, out var value))
        {
            var gest = new TouchGestureRecognizer
            {
                CancelsTouchesInView = false,
                Delegate = new TouchGestureRecognizerDelegate(view),
            };

            gest.OnTouch += ActionActivator;
            value = new GestureActionsContainer
            {
                Recognizer = gest,
            };

            Collection.Add(view, value);
            view.AddGestureRecognizer(gest);
        }

        switch (actionType)
        {
            case ActionType.Color:
                value.ColorActions.Add(action);
                break;
            case ActionType.Tap:
                value.TapActions.Add(action);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
        }
    }

    public static void Delete(UIView view, Action<TouchGestureRecognizer.TouchArgs> action)
    {
        if (!Collection.ContainsKey(view))
        {
            return;
        }

        var ci = Collection[view];
        ci.RemoveAction(action);

        if (ci.ActionCount != 0)
        {
            return;
        }

        view.RemoveGestureRecognizer(ci.Recognizer);
        Collection.Remove(view);
    }

    private static void ActionActivator(object sender, TouchGestureRecognizer.TouchArgs e)
    {
        var gest = (TouchGestureRecognizer)sender;
        if (!Collection.ContainsKey(gest.View))
        {
            return;
        }

        var actions = Collection[gest.View]
            .Actions.ToArray();
        foreach (var valueAction in actions)
        {
            valueAction?.Invoke(e);
        }
    }

    private class GestureActionsContainer
    {
        public TouchGestureRecognizer Recognizer { get; init; }

        public IEnumerable<Action<TouchGestureRecognizer.TouchArgs>> Actions => ColorActions.Concat(TapActions);

        public List<Action<TouchGestureRecognizer.TouchArgs>> TapActions { get; } = [];

        public List<Action<TouchGestureRecognizer.TouchArgs>> ColorActions { get; } = [];

        public int ActionCount => TapActions.Count + ColorActions.Count;

        public void RemoveAction(Action<TouchGestureRecognizer.TouchArgs> action)
        {
            if (TapActions.Contains(action))
            {
                TapActions.Remove(action);
            }
            else if (ColorActions.Contains(action))
            {
                ColorActions.Remove(action);
            }
        }
    }
}