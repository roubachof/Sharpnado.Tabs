using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public abstract class TabItem : ContentView
    {
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(TabTextItem),
            false);

        public static readonly BindableProperty SelectedTabColorProperty = BindableProperty.Create(
            nameof(SelectedTabColor),
            typeof(Color),
            typeof(TabTextItem),
            Color.Default);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Color SelectedTabColor
        {
            get => (Color)GetValue(SelectedTabColorProperty);
            set => SetValue(SelectedTabColorProperty, value);
        }

        public bool IsSelectable { get; set; } = true;
    }
}
