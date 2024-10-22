using CoreFoundation;

using Foundation;

using UIKit;

namespace Sharpnado.Tabs.Effects.iOS.GestureRecognizers;

public class TouchGestureRecognizer : UIGestureRecognizer
{
    public enum TouchState
    {
        Started,

        Ended,

        Cancelled
    }

    private bool _disposed;

    private bool _startCalled;

    public static bool IsActive { get; private set; }

    public bool Processing => State == UIGestureRecognizerState.Began || State == UIGestureRecognizerState.Changed;

    public event EventHandler<TouchArgs> OnTouch;

    public override async void TouchesBegan(NSSet touches, UIEvent evt)
    {
        base.TouchesBegan(touches, evt);
        if (Processing)
        {
            return;
        }

        State = UIGestureRecognizerState.Began;
        IsActive = true;
        _startCalled = false;

        await Task.Delay(125);
        DispatchQueue.MainQueue.DispatchAsync(
            () =>
            {
                if (!Processing || _disposed)
                {
                    return;
                }

                OnTouch?.Invoke(this, new TouchArgs(TouchState.Started, true));
                _startCalled = true;
            });
    }

    public override void TouchesMoved(NSSet touches, UIEvent evt)
    {
        base.TouchesMoved(touches, evt);

        bool inside = View.PointInside(LocationInView(View), evt);

        if (!inside)
        {
            if (_startCalled)
            {
                OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, false));
            }

            State = UIGestureRecognizerState.Ended;
            IsActive = false;
            return;
        }

        State = UIGestureRecognizerState.Changed;
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
        base.TouchesEnded(touches, evt);

        if (!_startCalled)
        {
            OnTouch?.Invoke(this, new TouchArgs(TouchState.Started, true));
        }

        OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, View.PointInside(LocationInView(View), null)));
        State = UIGestureRecognizerState.Ended;
        IsActive = false;
    }

    public override void TouchesCancelled(NSSet touches, UIEvent evt)
    {
        base.TouchesCancelled(touches, evt);
        OnTouch?.Invoke(this, new TouchArgs(TouchState.Cancelled, false));
        State = UIGestureRecognizerState.Cancelled;
        IsActive = false;
    }

    internal void TryEndOrFail()
    {
        if (_startCalled)
        {
            OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, false));
            State = UIGestureRecognizerState.Ended;
        }

        State = UIGestureRecognizerState.Failed;
        IsActive = false;
    }

    protected override void Dispose(bool disposing)
    {
        _disposed = true;
        IsActive = false;

        base.Dispose(disposing);
    }

    public class TouchArgs : EventArgs
    {
        public TouchArgs(TouchState state, bool inside)
        {
            State = state;
            Inside = inside;
        }

        public TouchState State { get; }

        public bool Inside { get; }
    }
}

public class TouchGestureRecognizerDelegate : UIGestureRecognizerDelegate
{
    private readonly UIView _view;

    public TouchGestureRecognizerDelegate(UIView view)
    {
        _view = view;
    }

    public override bool ShouldRecognizeSimultaneously(
        UIGestureRecognizer gestureRecognizer,
        UIGestureRecognizer otherGestureRecognizer)
    {
        if (gestureRecognizer is TouchGestureRecognizer rec
            && otherGestureRecognizer is UIPanGestureRecognizer
            && otherGestureRecognizer.State == UIGestureRecognizerState.Began)
        {
            rec.TryEndOrFail();
        }

        return true;
    }

    public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
    {
        if (recognizer is TouchGestureRecognizer && TouchGestureRecognizer.IsActive)
        {
            return false;
        }

        return touch.View.IsDescendantOfView(_view);
    }
}