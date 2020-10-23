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
            Color.Default);

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
                if (SelectedTabColor != Color.Default)
                {
                    BackgroundColor = SelectedTabColor;
                }

                if (SelectedLabelColor != Color.Default)
                {
                    _label.TextColor = SelectedLabelColor;
                }
            }
            else
            {
                BackgroundColor = Color.Transparent;

                if (UnselectedLabelColor != Color.Default)
                {
                    _label.TextColor = UnselectedLabelColor;
                }
            }
        }
    }
}