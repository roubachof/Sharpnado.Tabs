namespace Sharpnado.Tabs;

public interface ILazyView
{
    View Content { get; set; }

    Color AccentColor { get; }

    bool IsLoaded { get; }

    void LoadView();
}

public abstract class ALazyView : ContentView, ILazyView, IDisposable, IAnimatableReveal
{
    public static readonly BindableProperty LoadingViewProperty = BindableProperty.Create(
        nameof(LoadingView),
        typeof(View),
        typeof(ALazyView),
        propertyChanged: LoadingViewChanged);

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(ILazyView),
        Colors.Magenta,
        propertyChanged: AccentColorChanged);

    public static readonly BindableProperty UseActivityIndicatorProperty = BindableProperty.Create(
        nameof(UseActivityIndicator),
        typeof(bool),
        typeof(ILazyView),
        false,
        propertyChanged: UseActivityIndicatorChanged);

    public static readonly BindableProperty AnimateProperty = BindableProperty.Create(
        nameof(Animate),
        typeof(bool),
        typeof(ILazyView),
        false);

    public View? LoadingView
    {
        get => (View?)GetValue(LoadingViewProperty);
        set => SetValue(LoadingViewProperty, value);
    }

    public bool UseActivityIndicator
    {
        get => (bool)GetValue(UseActivityIndicatorProperty);
        set => SetValue(UseActivityIndicatorProperty, value);
    }

    public bool Animate
    {
        get => (bool)GetValue(AnimateProperty);
        set => SetValue(AnimateProperty, value);
    }

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public bool IsLoaded { get; protected set; }

    public void Dispose()
    {
        if (Content is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public abstract void LoadView();

    protected override void OnBindingContextChanged()
    {
        if (Content != null && Content is not ActivityIndicator)
        {
            Content.BindingContext = BindingContext;
        }
    }

    private static void LoadingViewChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var lazyView = (ALazyView)bindable;
        var loadingView = (View)newvalue;

        if (loadingView is not null)
        {
            lazyView.Content = loadingView;
        }
    }

    private static void AccentColorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var lazyView = (ILazyView)bindable;
        if (lazyView.Content is ActivityIndicator activityIndicator)
        {
            activityIndicator.Color = (Color)newvalue;
        }
    }

    private static void UseActivityIndicatorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var lazyView = (ILazyView)bindable;
        bool useActivityIndicator = (bool)newvalue;

        if (useActivityIndicator)
        {
            lazyView.Content = new ActivityIndicator
            {
                Color = lazyView.AccentColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsRunning = true,
            };
        }
    }
}

public class LazyView<TView> : ALazyView
    where TView : View, new()
{
    public override void LoadView()
    {
        IsLoaded = true;

        View view = new TView
        {
            BindingContext = BindingContext,
        };

        Content = view;
    }
}