using Sharpnado.Tasks;

namespace Sharpnado.Tabs;

public class DelayedView<TView> : LazyView<TView>
    where TView : View, new()
{
    public int DelayInMilliseconds { get; set; } = 200;

    private View? _currentlyBuiltView;

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (_currentlyBuiltView != null)
        {
            _currentlyBuiltView.BindingContext = BindingContext;
        }
    }

    public override void LoadView()
    {
        TaskMonitor.Create(
            async () =>
                {
                    _currentlyBuiltView = new TView
                    {
                        BindingContext = BindingContext,
                    };

                    await Task.Delay(DelayInMilliseconds);

                    IsLazyLoaded = true;
                    Content = _currentlyBuiltView;
                });
    }
}

public class DelayedView : ALazyView
{
    public static readonly BindableProperty ViewProperty = BindableProperty.Create(
        nameof(View),
        typeof(View),
        typeof(DelayedView),
        default(View));

    public View View
    {
        get => (View)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    public int DelayInMilliseconds { get; set; } = 200;

    public override void LoadView()
    {
        if (IsLazyLoaded)
        {
            return;
        }

        TaskMonitor.Create(
            async () =>
                {
                    await Task.Delay(DelayInMilliseconds);
                    if (IsLazyLoaded)
                    {
                        return;
                    }

                    IsLazyLoaded = true;
                    Content = View;
                });
    }
}