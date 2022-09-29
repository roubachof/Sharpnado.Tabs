using System.Runtime.CompilerServices;
using Sharpnado.Tabs;
using Sharpnado.Tasks;

namespace MauiSample.Presentation.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpamTab : TabItem
    {
        public static readonly BindableProperty SpamImageProperty = BindableProperty.Create(
            nameof(SpamImage),
            typeof(string),
            typeof(SpamTab),
            string.Empty);

        private double _height = 0;

        private bool _isAttached;

        public SpamTab()
        {
            InitializeComponent();
        }

        public string SpamImage
        {
            get => (string)GetValue(SpamImageProperty);
            set => SetValue(SpamImageProperty, value);
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            _isAttached = true;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (height > 0 && height != _height)
            {
                _height = height;
                if (!IsSelected)
                {
                    Foot.TranslationY = -_height;
                    Foot.Opacity = 0;
                    Spam.HeightRequest = _height;
                }
                else
                {
                    Spam.HeightRequest = 0;
                }
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsSelected))
            {
                Animate(IsSelected);
            }
        }

        protected override void OnBadgeChanged(BadgeView oldBadge)
        {
            throw new System.NotImplementedException();
        }

        private void Animate(bool isSelected)
        {
            double targetFootOpacity = isSelected ? 1 : 0;
            double targetFootTranslationY = isSelected ? 0 : -_height;

            double targetHeightSpam = isSelected ? 0 : _height;

            TaskMonitor.Create(
                async () =>
                    {
                        if (!_isAttached)
                        {
                            await Task.Delay(2000);
                        }

                        Task fadeFootTask = Foot.FadeTo(targetFootOpacity, 500);
                        Task translateFootTask = Foot.TranslateTo(0, targetFootTranslationY, 250, Easing.CubicOut);
                        Task heightSpamTask = Spam.HeightRequestTo(targetHeightSpam, 250, Easing.CubicOut);

                        await Task.WhenAll(fadeFootTask, translateFootTask);

                        Spam.HeightRequest = targetHeightSpam;
                        Foot.TranslationY = targetFootTranslationY;
                        Foot.Opacity = targetFootOpacity;
                    });
        }
    }

    public static class ViewExtensions
    {
        public static Task<bool> AnimateTo(
            this VisualElement view,
            double start,
            double end,
            string name,
            Action<VisualElement, double> updateAction,
            uint length = 250,
            Easing easing = null)
        {
            if (easing == null)
            {
                easing = Easing.Linear;
            }

            var tcs = new TaskCompletionSource<bool>();
            var weakView = new WeakReference<VisualElement>(view);
            new Animation(UpdateProperty, start, end, easing, null).Commit(
                view,
                name,
                16U,
                length,
                null,
                (f, a) => tcs.SetResult(a),
                null);
            return tcs.Task;

            void UpdateProperty(double f)
            {
                if (!weakView.TryGetTarget(out var target))
                {
                    return;
                }

                updateAction(target, f);
            }
        }

        public static Task<bool> HeightRequestTo(
            this VisualElement view,
            double height,
            uint length = 250,
            Easing easing = null)
        {
            return view.AnimateTo(
                view.HeightRequest,
                height,
                nameof(HeightRequestTo),
                (v, value) => v.HeightRequest = value,
                length,
                easing);
        }
    }
}