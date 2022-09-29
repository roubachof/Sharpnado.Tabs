using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public class ViewSwitcher : Grid, IDisposable
    {
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(ViewSwitcher),
            defaultValue: -1,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: SelectedIndexPropertyChanged);

        public static readonly BindableProperty OnShowCommandProperty = BindableProperty.Create(
            nameof(OnShowCommand),
            typeof(ICommand),
            typeof(ViewSwitcher),
            null);

        public static readonly BindableProperty OnHideCommandProperty = BindableProperty.Create(
            nameof(OnHideCommand),
            typeof(ICommand),
            typeof(ViewSwitcher),
            null);

        private View _activeView;

        private bool _isAttached;

        public ViewSwitcher()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;

#if !NET6_0_OR_GREATER
            _isAttached = true;
#endif
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public ICommand OnShowCommand
        {
            get { return (ICommand)GetValue(OnShowCommandProperty); }
            set { SetValue(OnShowCommandProperty, value); }
        }

        public ICommand OnHideCommand
        {
            get { return (ICommand)GetValue(OnHideCommandProperty); }
            set { SetValue(OnHideCommandProperty, value); }
        }

        public bool Animate { get; set; } = true;

        public void Dispose()
        {
            foreach (var child in Children)
            {
                if (child is IDisposable disposableView)
                {
                    disposableView.Dispose();
                }
            }
        }

        public void OnAppearing()
        {
        }

        public void OnDisappearing()
        {
        }

#if NET6_0_OR_GREATER
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            _isAttached = true;
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);

            if (child is View view)
            {
                HideView(view, Children.Count - 1);
            }
        }
#else
        protected override void OnAdded(View view)
        {
            base.OnAdded(view);

            HideView(view, Children.Count - 1);
        }
#endif

        private static void SelectedIndexPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var viewSwitcher = (ViewSwitcher)bindable;
            viewSwitcher.UpdateSelectedView((int)newvalue);
        }

        private void UpdateSelectedView(int selectedIndex)
        {
            if (selectedIndex < 0)
            {
                return;
            }

            View previousVisibleView = null;
            int previousVisibleViewIndex = -1;

            View newVisibleView = null;

            for (int index = 0; index < Children.Count; index++)
            {
                var view = (View)Children[index];
                if (view.IsVisible)
                {
                    previousVisibleView = view;
                    previousVisibleViewIndex = index;
                }

                if (index == selectedIndex)
                {
                    newVisibleView = view;
                }
            }

            if (previousVisibleView != newVisibleView)
            {
                if (previousVisibleView != null && previousVisibleView.IsVisible)
                {
                    HideView(previousVisibleView, previousVisibleViewIndex);
                }

                if (newVisibleView != null && !newVisibleView.IsVisible)
                {
                    ShowView(newVisibleView, selectedIndex);
                }
            }
        }

        private void HideView(View view, int viewIndex)
        {
            view.IsVisible = false;
            if (Animate && view is IAnimatableReveal animatable && animatable.Animate && _isAttached)
            {
                view.TranslationY = -200;
                view.Opacity = 0;
            }

            if (view is ILazyView lazyView)
            {
                view = lazyView.Content;
            }

            if (_activeView == view)
            {
                _activeView = null;
            }

            OnHideCommand?.Execute(viewIndex);
        }

        private void ShowView(View view, int viewIndex)
        {
            var lazyView = view as ILazyView;
            if (lazyView != null)
            {
                if (!lazyView.IsLoaded)
                {
                    lazyView.LoadView();
                }
            }

            view.IsVisible = true;

            if (Animate && view is IAnimatableReveal animatable && animatable.Animate && view.Opacity == 0 && _isAttached)
            {
                var localView = view;
                TaskMonitor.Create(
                    async () =>
                    {
                        Task fadeTask = localView.FadeTo(1, 500);
                        Task translateTask = localView.TranslateTo(0, 0, 250, Easing.CubicOut);

                        await Task.WhenAll(fadeTask, translateTask);
                        localView.TranslationY = 0;
                        localView.Opacity = 1;
                    });
            }

            if (lazyView != null)
            {
                view = lazyView.Content;
            }

            _activeView = view;

            OnShowCommand?.Execute(viewIndex);
        }
    }
}
