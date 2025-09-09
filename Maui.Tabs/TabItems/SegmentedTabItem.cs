using System.Runtime.CompilerServices;

namespace Sharpnado.Tabs
{
    public class SegmentedTabItem : TabTextItem
    {
        public static readonly BindableProperty SelectedLabelColorProperty = BindableProperty.Create(
            nameof(SelectedLabelColor),
            typeof(Color),
            typeof(SegmentedTabItem),
            Colors.White);

        public Color SelectedLabelColor
        {
            get => (Color)GetValue(SelectedLabelColorProperty);
            set => SetValue(SelectedLabelColorProperty, value);
        }

        protected override async void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            Background = Colors.Transparent;
            await Task.Delay(100);
            UpdateLabel();
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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
            var label = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = LabelSize,
                    Text = Label,
                };

            if (FontFamily != null)
            {
                label.FontFamily = FontFamily;
            }

            label.TextColor = IsSelected ? SelectedLabelColor : UnselectedLabelColor;

            Background = IsSelected ? SelectedTabColor : Colors.Transparent;

            AddContent(label);
        }
    }
}