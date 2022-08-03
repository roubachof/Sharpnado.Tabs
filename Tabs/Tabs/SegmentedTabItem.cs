using System;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public class SegmentedTabItem : TabTextItem
    {
        public static readonly BindableProperty SelectedLabelColorProperty = BindableProperty.Create(
            nameof(SelectedLabelColor),
            typeof(Color),
            typeof(SegmentedTabItem),
#if NET6_0_OR_GREATER
            Colors.DodgerBlue);
#else
            Color.Default);
#endif

        private readonly Label _label;

        public SegmentedTabItem()
        {
            _label = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            Content = _label;
            UpdateLabel();
        }

        public Color SelectedLabelColor
        {
            get => (Color)GetValue(SelectedLabelColorProperty);
            set => SetValue(SelectedLabelColorProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsSelected):
                case nameof(Label):
                case nameof(LabelSize):
                case nameof(FontFamily):
                case nameof(SelectedTabColor):
                case nameof(SelectedLabelColor):
                case nameof(UnselectedLabelColor):
                    UpdateLabel();
                    break;
            }
        }

        protected override void OnBadgeChanged(BadgeView oldBadge)
        {
            throw new NotSupportedException("Badge is not currently supported for SegmentedTabItem");
        }

        private void UpdateLabel()
        {
            if (_label == null)
            {
                return;
            }

            if (FontFamily != null)
            {
                _label.FontFamily = FontFamily;
            }

            _label.FontSize = LabelSize;
            _label.Text = Label;

            if (IsSelected)
            {
#if !NET6_0_OR_GREATER
                if (SelectedTabColor != Color.Default)
#endif
                {
                    BackgroundColor = SelectedTabColor;
                }

#if !NET6_0_OR_GREATER
                if (SelectedLabelColor != Color.Default)
#endif
                {
                    _label.TextColor = SelectedLabelColor;
                }
            }
            else
            {

#if NET6_0_OR_GREATER
                BackgroundColor = Colors.Transparent;
#else
                BackgroundColor = Color.Transparent;
#endif

#if !NET6_0_OR_GREATER
                if (UnselectedLabelColor != Color.Default)
#endif
                {
                    _label.TextColor = UnselectedLabelColor;
                }
            }
        }
    }
}