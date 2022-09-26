using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

#if !NET6_0_OR_GREATER
using Sharpnado.Shades;
#endif

using Sharpnado.Tabs.Effects;

using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public enum TabType
    {
        Fixed = 0,
        Scrollable
    }

    public enum OrientationType
    {
        Horizontal = 0,
        Vertical
    }

    [ContentProperty("Tabs")]

#if NET6_0_OR_GREATER
    public class TabHostView : ContentView
#else
    public class TabHostView : Shadows
#endif
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(TabHostView),
            defaultValueCreator: _ => Array.Empty<TabItem>());

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(TabHostView));

        public static readonly BindableProperty TabsProperty = BindableProperty.Create(
            nameof(Tabs),
            typeof(ObservableCollection<TabItem>),
            typeof(TabHostView),
            defaultValueCreator: _ => new ObservableCollection<TabItem>());

        public static readonly BindableProperty IsSegmentedProperty = BindableProperty.Create(
            nameof(IsSegmented),
            typeof(bool),
            typeof(TabHostView),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty SegmentedOutlineColorProperty = BindableProperty.Create(
            nameof(SegmentedOutlineColor),
            typeof(Color),
            typeof(TabHostView),
#if NET6_0_OR_GREATER
            Colors.DodgerBlue);
#else
            Color.Default);
#endif

#if NET6_0_OR_GREATER
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius),
            typeof(float),
            typeof(TabHostView),
            defaultValue: 10);
#endif

        public static readonly BindableProperty SegmentedHasSeparatorProperty = BindableProperty.Create(
            nameof(SegmentedHasSeparator),
            typeof(bool),
            typeof(TabHostView),
            defaultValue: false,
            propertyChanged: OnSegmentedHasSeparatorChanged);

        public static readonly BindableProperty TabTypeProperty = BindableProperty.Create(
            nameof(TabType),
            typeof(TabType),
            typeof(TabHostView),
            defaultValue: TabType.Fixed,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(TabHostView),
            defaultValue: -1,
            propertyChanged: SelectedIndexPropertyChanged);

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(
            nameof(Orientation),
            typeof(OrientationType),
            typeof(TabHostView),
            defaultValue: OrientationType.Horizontal,
            propertyChanged: OrientationPropertyChanged);

        public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(TabHostView),
#if NET6_0_OR_GREATER
            Colors.Transparent);
#else
            Color.Transparent);
#endif

        private const string Tag = nameof(TabHostView);

        private readonly Grid _grid;
        private readonly Frame _frame;
        private List<TabItem> _selectableTabs = new();

        private INotifyCollectionChanged _currentNotifyCollection;

        private ScrollView _scrollView;

        private ColumnDefinition _lastFillingColumn;

        private RowDefinition _lastFillingRow;

        public TabHostView()
        {
            TabItemTappedCommand = new TapCommand(OnTabItemTapped);

            Tabs.CollectionChanged += OnTabsCollectionChanged;

#if NET6_0_OR_GREATER
            base.BackgroundColor = Colors.Transparent;
#else
            base.BackgroundColor = Color.Transparent;
#endif

            _grid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = BackgroundColor,
            };

            _frame = new Frame
            {
                Padding = 0,
                HasShadow = false,
                IsClippedToBounds = true,
                CornerRadius = CornerRadius,
#if NET6_0_OR_GREATER
                BackgroundColor = Colors.Transparent,
#else
                BackgroundColor = Color.Transparent,
#endif
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                BorderColor = SegmentedOutlineColor,
            };

            UpdateTabType();

#if !NET6_0_OR_GREATER
            Shades = new List<Shade>();
#endif
        }

        public event EventHandler<SelectedPositionChangedEventArgs> SelectedTabIndexChanged;

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public ObservableCollection<TabItem> Tabs
        {
            get => (ObservableCollection<TabItem>)GetValue(TabsProperty);
            set => SetValue(TabsProperty, value);
        }

        public bool IsSegmented
        {
            get => (bool)GetValue(IsSegmentedProperty);
            set => SetValue(IsSegmentedProperty, value);
        }

        /// <summary>
        /// Only available if IsSegmented is true.
        /// </summary>
        public Color SegmentedOutlineColor
        {
            get => (Color)GetValue(SegmentedOutlineColorProperty);
            set => SetValue(SegmentedOutlineColorProperty, value);
        }

        /// <summary>
        /// Only available if IsSegmented is true.
        /// </summary>
        public bool SegmentedHasSeparator
        {
            get => (bool)GetValue(SegmentedHasSeparatorProperty);
            set => SetValue(SegmentedHasSeparatorProperty, value);
        }

        public TabType TabType
        {
            get => (TabType)GetValue(TabTypeProperty);
            set => SetValue(TabTypeProperty, value);
        }

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public OrientationType Orientation
        {
            get => (OrientationType)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public new View Content
        {
            get => base.Content;
            set =>
                throw new NotSupportedException(
                    "You can only add TabItem to the TabHostView through the Tabs property");
        }

        public View TabHostContent
        {
            set =>
                throw new NotSupportedException(
                    "Starting from version 1.3, you can only add TabItem to the TabHostView through the Tabs property");
        }

        public bool ShowScrollbar { get; set; }

        private ICommand TabItemTappedCommand { get; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(ItemTemplate):
                    UpdateItemTemplate();
                    break;
                case nameof(BackgroundColor):
                    UpdateBackgroundColor();
                    break;
                case nameof(SegmentedOutlineColor):
                    UpdateSegmentedOutlineColor();
                    break;
                case nameof(CornerRadius):
                    UpdateCornerRadius();
                    break;
                case nameof(IsSegmented):
                case nameof(TabType):
                    UpdateTabType();
                    break;
                case nameof(Tabs):
                    throw new NotSupportedException("Updating Tabs collection reference is not supported");
            }
        }

        private static void OnSegmentedHasSeparatorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabHost = (TabHostView)bindable;
            if (!tabHost.IsSegmented)
            {
                return;
            }

            if (!(bool)oldvalue && (bool)newvalue)
            {
                tabHost.AddSeparators();
            }

            if ((bool)oldvalue && !(bool)newvalue)
            {
                tabHost.RemoveSeparators();
            }
        }

        private static void SelectedIndexPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabHostView = (TabHostView)bindable;

            int selectedIndex = (int)newvalue;
            if (selectedIndex < 0)
            {
                return;
            }

            tabHostView.UpdateSelectedIndex(selectedIndex);
            tabHostView.RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        }

        private static void OrientationPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (oldvalue != newvalue)
            {
                var tabHostView = (TabHostView)bindable;
                tabHostView.UpdateTabOrientation();
            }
        }

        private void InitializeItems()
        {
            if (ItemTemplate == null)
            {
                return;
            }

            int index = 0;
            foreach (var model in ItemsSource ?? new object[0])
            {
                var tabItem = CreateTabItem(model);
                Tabs.Insert(index++, tabItem);
            }
        }

        private void UpdateItemsSource()
        {
            if (_currentNotifyCollection != null)
            {
                _currentNotifyCollection.CollectionChanged -= ItemsSourceCollectionChanged;
                _currentNotifyCollection = null;
            }

            if (ItemsSource is INotifyCollectionChanged notifyCollectionChanged)
            {
                _currentNotifyCollection = notifyCollectionChanged;
                _currentNotifyCollection.CollectionChanged += ItemsSourceCollectionChanged;
            }

            InitializeItems();
        }

        private void UpdateItemTemplate()
        {
            InitializeItems();
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemTemplate is null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var addedIndex = e.NewStartingIndex;
                    foreach (var model in e.NewItems)
                    {
                        var tabItem = CreateTabItem(model);
                        Tabs.Insert(addedIndex++, tabItem);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int removedIndex = e.OldStartingIndex + (e.OldItems.Count - 1);
                        removedIndex >= e.OldStartingIndex;
                        removedIndex--)
                    {
                        Tabs.RemoveAt(removedIndex);
                    }

                    break;

                default:
                    Console.WriteLine("Warning: TabHostView ItemsSource only support Add, Remove and Reset actions");
                    break;
            }
        }

        private TabItem CreateTabItem(object item)
        {
            View result;
            if (ItemTemplate is DataTemplateSelector selector)
            {
                var template = selector.SelectTemplate(item, this);
                result = (View)template.CreateContent();
            }
            else
            {
                result = (View)ItemTemplate.CreateContent();
            }

            if (result is not TabItem tabItem)
            {
                throw new InvalidOperationException("Your ItemTemplate DataTemplate should contain a view inheriting from TabItem");
            }

            tabItem.BindingContext = item;
            return tabItem;
        }

        private void UpdateSegmentedOutlineColor()
        {
            if (_frame == null)
            {
                return;
            }

            _frame.BorderColor = SegmentedOutlineColor;
            foreach (var separator in _grid.Children.Where(c => c is BoxView))
            {

#if NET6_0_OR_GREATER
                ((View)separator).BackgroundColor = Colors.Transparent;
#else
                separator.BackgroundColor = Color.Transparent;
#endif
            }
        }

        private void UpdateBackgroundColor()
        {
            if (IsSegmented)
            {
                if (_frame == null)
                {
                    return;
                }

#if NET6_0_OR_GREATER
                _grid.BackgroundColor = Colors.Transparent;
#else
                _grid.BackgroundColor = Color.Transparent;
#endif
                _frame.BackgroundColor = BackgroundColor;
                return;
            }

            if (_grid == null)
            {
                return;
            }

#if NET6_0_OR_GREATER
            _frame.BackgroundColor = Colors.Transparent;
#else
            _frame.BackgroundColor = Color.Transparent;
#endif
            _grid.BackgroundColor = BackgroundColor;
        }

        private void UpdateCornerRadius()
        {
            if (_frame == null)
            {
                return;
            }

            _frame.CornerRadius = CornerRadius;
        }

        private void AddSeparators()
        {
            for (int i = 0; i < _grid.Children.Count - 1; i++)
            {
                var currentItem = _grid.Children[i];
                var nextItem = _grid.Children[i + 1];
                if (currentItem is TabItem && nextItem is TabItem)
                {
                    _grid.Children.Insert(i + 1, CreateSeparator());
                }
            }
        }

        private void RemoveSeparators()
        {
            foreach (var separator in _grid.Children.Where(c => c is BoxView).ToArray())
            {
                _grid.Children.Remove(separator);
            }
        }

        private BoxView CreateSeparator()
        {
            if (Orientation == OrientationType.Horizontal)
            {
                return new BoxView { BackgroundColor = SegmentedOutlineColor, WidthRequest = 1 };
            }
            else
            {
                return new BoxView { BackgroundColor = SegmentedOutlineColor, HeightRequest = 1 };
            }
        }

        private void UpdateSelectedIndex(int selectedIndex)
        {
            if (_selectableTabs.Count == 0)
            {
                selectedIndex = 0;
            }

            if (selectedIndex > _selectableTabs.Count)
            {
                selectedIndex = _selectableTabs.Count - 1;
            }

            for (var index = 0; index < _selectableTabs.Count; index++)
            {
                _selectableTabs[index].IsSelected = selectedIndex == index;
            }

            SelectedIndex = selectedIndex;
            InternalLogger.Debug(Tag, () => $"SelectedIndex: {SelectedIndex}");
        }

        private void OnTabItemTapped(object tappedItem)
        {
            var selectedIndex = _selectableTabs.IndexOf((TabItem)tappedItem);

            if (!_selectableTabs[selectedIndex].IsSelectable)
            {
                return;
            }

            UpdateSelectedIndex(selectedIndex);
            RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        }

        private void UpdateTabType()
        {
            BatchBegin();

            InternalLogger.Debug(Tag, () => $"UpdateTabType() => TabType: {TabType}, IsSegmented: {IsSegmented}");

            if (IsSegmented)
            {
                _frame.Content = _grid;
                _frame.BackgroundColor = BackgroundColor;

#if NET6_0_OR_GREATER
                _grid.BackgroundColor = Colors.Transparent;
#else
                _grid.BackgroundColor = Color.Transparent;
#endif
            }
            else
            {
                _frame.Content = null;

#if NET6_0_OR_GREATER
                _frame.BackgroundColor = Colors.Transparent;
#else
                _frame.BackgroundColor = Color.Transparent;
#endif
                _grid.BackgroundColor = BackgroundColor;
            }

            if (TabType == TabType.Scrollable)
            {
                base.Content = _scrollView ??= new ScrollView
                {
                    Orientation = this.Orientation == OrientationType.Horizontal ? ScrollOrientation.Horizontal : ScrollOrientation.Vertical,
                    HorizontalScrollBarVisibility =
                        ShowScrollbar ? ScrollBarVisibility.Always : ScrollBarVisibility.Never,
                };

                if (IsSegmented)
                {
                    _scrollView.Content = _frame;
                }
                else
                {
                    _scrollView.Content = _grid;
                }

                if (Orientation == OrientationType.Horizontal)
                {
                    foreach (var definition in _grid.ColumnDefinitions)
                    {
                        definition.Width = GridLength.Auto;
                    }
                }
                else
                {
                    foreach (var definition in _grid.RowDefinitions)
                    {
                        definition.Height = GridLength.Star;
                    }
                }
            }
            else
            {
                if (IsSegmented)
                {
                    base.Content = _frame;
                }
                else
                {
                    base.Content = _grid;
                }

                if (Orientation == OrientationType.Horizontal)
                {
                    foreach (var definition in _grid.ColumnDefinitions)
                    {
                        definition.Width = GridLength.Star;
                    }
                }
                else
                {
                    foreach (var definition in _grid.RowDefinitions)
                    {
                        definition.Height = GridLength.Star;
                    }
                }
            }

            BatchCommit();
        }

        private void OnTabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var index = e.NewStartingIndex;
                    foreach (var tab in e.NewItems)
                    {
                        OnChildAdded((TabItem)tab, index++);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var tab in e.OldItems)
                    {
                        OnChildRemoved((TabItem)tab);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                default:
                    throw new NotSupportedException();
            }
        }

        private void AddTapCommand(TabItem tabItem)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                tabItem.GestureRecognizers.Add(
                    new TapGestureRecognizer { Command = TabItemTappedCommand, CommandParameter = tabItem }
                    );
            }
            else
            {
#if NET6_0_OR_GREATER
                XamEffects.TouchEffect.SetColor(tabItem, tabItem.SelectedTabColor);
                XamEffects.Commands.SetTap(tabItem, TabItemTappedCommand);
                XamEffects.Commands.SetTapParameter(tabItem, tabItem);

                tabItem.Effects.Add(new XamEffects.TouchRoutingEffect());
                tabItem.Effects.Add(new XamEffects.CommandsRoutingEffect());
#else
                ViewEffect.SetTouchFeedbackColor(tabItem, tabItem.SelectedTabColor);
                TapCommandEffect.SetTap(tabItem, TabItemTappedCommand);
                TapCommandEffect.SetTapParameter(tabItem, tabItem);

                tabItem.Effects.Add(new ViewStyleEffect());
                tabItem.Effects.Add(new TapCommandRoutingEffect());
#endif
            }
        }

        private void OnChildAdded(TabItem tabItem, int index)
        {
            InternalLogger.Debug(Tag, () => $"OnChildAdded( tabItem: {tabItem.GetType().Name}, index: {index} )");

            _grid.BatchBegin();
            BatchBegin();

            var tabIndexInGrid = GetTabIndexInGrid(index);

            _grid.Children.Insert(tabIndexInGrid, tabItem);
            if (Orientation == OrientationType.Horizontal)
            {
                _grid.ColumnDefinitions.Insert(tabIndexInGrid, new ColumnDefinition { Width = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto });

                if (TabType == TabType.Scrollable)
                {
                    if (Tabs.Count == 1)
                    {
                        // Add a last empty slot to fill remaining space
                        _lastFillingColumn = new ColumnDefinition { Width = GridLength.Star };
                        _grid.ColumnDefinitions.Add(_lastFillingColumn);
                    }
                    else
                    {
                        _grid.ColumnDefinitions.Remove(_lastFillingColumn);
                        _grid.ColumnDefinitions.Add(_lastFillingColumn);
                    }
                }

                Grid.SetRow(tabItem, 0);
            }
            else
            {
                _grid.RowDefinitions.Insert(tabIndexInGrid, new RowDefinition { Height = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto });

                if (TabType == TabType.Scrollable)
                {
                    if (Tabs.Count == 1)
                    {
                        // Add a last empty slot to fill remaining space
                        _lastFillingRow = new RowDefinition { Height = GridLength.Star };
                        _grid.RowDefinitions.Add(_lastFillingRow);
                    }
                    else
                    {
                        _grid.RowDefinitions.Remove(_lastFillingRow);
                        _grid.RowDefinitions.Add(_lastFillingRow);
                    }
                }

                Grid.SetColumn(tabItem, 0);
            }

            RaiseTabButtons();
            AddTapCommand(tabItem);

            if (TabType == TabType.Fixed)
            {
                tabItem.PropertyChanged += OnTabItemPropertyChanged;
                UpdateTabVisibility(tabItem);
            }

            UpdateSelectableTabs();

            if (IsSegmented && SegmentedHasSeparator)
            {
                ConsolidateSeparatedColumnIndexes();
            }
            else
            {
                ConsolidateColumnIndexes();
            }

            ConsolidateSelectedIndex();

            BatchCommit();
            _grid.BatchCommit();
        }

        private int GetTabIndexInGrid(int index)
        {
            if (IsSegmented && SegmentedHasSeparator)
            {
                if (_grid.Children.Count == 0 || index == 0)
                {
                    return 0;
                }

                var previousElementAt = _grid.Children.Where(v => v is TabItem).ElementAtOrDefault(index - 1);
                var indexInGrid = _grid.Children.IndexOf(previousElementAt) + 1;

                InternalLogger.Debug(Tag, () => $"GetTabIndexInGrid() => indexInGrid: {indexInGrid}");
                return indexInGrid;
            }

            return index;
        }

        private void OnChildRemoved(TabItem tabItem)
        {
            if (_grid.ColumnDefinitions.Count == 0)
            {
                return;
            }

            _grid.BatchBegin();
            BatchBegin();

            if (TabType == TabType.Scrollable)
            {
                if (Tabs.Count == 0)
                {
                    _grid.ColumnDefinitions.Remove(_lastFillingColumn);
                }
            }

            var tabItemIndex = _grid.Children.IndexOf(tabItem);

            InternalLogger.Debug(Tag, () => $"OnChildRemoved( tabItem: {tabItem.GetType().Name}, index: {tabItemIndex} )");

            if (tabItemIndex > 1 && _grid.Children[tabItemIndex - 1] is BoxView)
            {
                _grid.Children.RemoveAt(tabItemIndex - 1);
                _grid.ColumnDefinitions.RemoveAt(tabItemIndex - 1);
                tabItemIndex--;
            }

            _grid.Children.RemoveAt(tabItemIndex);
            _grid.ColumnDefinitions.RemoveAt(tabItemIndex);

            tabItem.PropertyChanged -= OnTabItemPropertyChanged;

            if (IsSegmented && SegmentedHasSeparator)
            {
                ConsolidateSeparatedColumnIndexes();
            }
            else
            {
                ConsolidateColumnIndexes();
            }

            UpdateSelectableTabs();
            ConsolidateSelectedIndex();

            BatchCommit();
            _grid.BatchCommit();
        }

        private void UpdateSelectableTabs()
        {
            _selectableTabs = Tabs.Where(t => t.IsSelectable).ToList();
        }

        private void ConsolidateColumnIndexes()
        {
            var index = 0;
            foreach (var tabItem in Tabs)
            {
                if (Orientation == OrientationType.Horizontal)
                {
                    Grid.SetColumn(tabItem, index++);
                }
                else
                {
                    Grid.SetRow(tabItem, index++);
                }
            }
        }

        private void ConsolidateSeparatedColumnIndexes()
        {
            if (_grid.Children.FirstOrDefault() is BoxView)
            {
                _grid.Children.RemoveAt(0);
                if (Orientation == OrientationType.Horizontal)
                {
                    _grid.ColumnDefinitions.RemoveAt(0);
                }
                else
                {
                    _grid.RowDefinitions.RemoveAt(0);
                }
            }

            if (_grid.Children.LastOrDefault() is BoxView)
            {
                _grid.Children.RemoveAt(_grid.Children.Count - 1);
                if (Orientation == OrientationType.Horizontal)
                {
                    _grid.ColumnDefinitions.RemoveAt(_grid.Children.Count - 1);
                }
                else
                {
                    _grid.RowDefinitions.RemoveAt(_grid.Children.Count - 1);
                }
            }

            var index = 0;
            while (index < _grid.Children.Count)
            {
                var currentItem = _grid.Children[index];

                var previousItemIsTab = index > 0 && _grid.Children[index - 1] is TabItem;
                var currentItemIsTab = currentItem is TabItem;

                if (previousItemIsTab && currentItemIsTab)
                {
                    var separator = CreateSeparator();
                    if (Orientation == OrientationType.Horizontal)
                    {
                        _grid.ColumnDefinitions.Insert(index, new ColumnDefinition { Width = separator.WidthRequest });
                    }
                    else
                    {
                        _grid.RowDefinitions.Insert(index, new RowDefinition { Height = separator.HeightRequest });
                    }

                    _grid.Children.Insert(index, separator);

                    if (Orientation == OrientationType.Horizontal)
                    {
                        Grid.SetColumn(separator, index);
                        Grid.SetRow(separator, 0);
                    }
                    else
                    {
                        Grid.SetColumn(separator, 0);
                        Grid.SetRow(separator, index);
                    }

                    index++;

                    continue;
                }

                var previousItemIsSeparator = index > 0 && _grid.Children[index - 1] is BoxView;
                var currentItemIsSeparator = currentItem is BoxView;

                if (previousItemIsSeparator && currentItemIsSeparator)
                {
                    _grid.Children.Remove(currentItem);
                    if (Orientation == OrientationType.Horizontal)
                    {
                        _grid.ColumnDefinitions.RemoveAt(index);
                    }
                    else
                    {
                        _grid.RowDefinitions.RemoveAt(index);
                    }

                    continue;
                }

                if (Orientation == OrientationType.Horizontal)
                {
                    Grid.SetColumn((BindableObject)currentItem, index);
                }
                else
                {
                    Grid.SetRow((BindableObject)currentItem, index);
                }

                index++;
            }
        }

        private void ConsolidateSelectedIndex()
        {
            if (_selectableTabs.Count == 0)
            {
                SelectedIndex = 0;
                return;
            }

            bool found = false;
            for (int index = 0; index < _selectableTabs.Count; index++)
            {
                var tabItem = _selectableTabs[index];
                if (tabItem.IsSelected)
                {
                    if (found)
                    {
                        tabItem.IsSelected = false;
                    }
                    else
                    {
                        SelectedIndex = index;
                        found = true;
                    }
                }
            }

            if (found || SelectedIndex < 0)
            {
                return;
            }

            if (SelectedIndex >= _selectableTabs.Count)
            {
                SelectedIndex = _selectableTabs.Count - 1;
            }

            _selectableTabs[SelectedIndex].IsSelected = true;
        }

        private void OnTabItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tabItem = (TabItem)sender;
            if (e.PropertyName == nameof(TabItem.IsVisible))
            {
                UpdateTabVisibility(tabItem);
            }
        }

        private void UpdateTabVisibility(TabItem tabItem)
        {
            if (Orientation == OrientationType.Horizontal)
            {
                int columnIndex = Grid.GetColumn(tabItem);
                var columnDefinition = _grid.ColumnDefinitions[columnIndex];
                columnDefinition.Width = tabItem.IsVisible ? GridLength.Star : 0;
            }
            else
            {
                int rowIndex = Grid.GetRow(tabItem);
                var rowDefinition = _grid.RowDefinitions[rowIndex];
                rowDefinition.Height = tabItem.IsVisible ? GridLength.Star : 0;
            }
        }

        private void RaiseSelectedTabIndexChanged(SelectedPositionChangedEventArgs e)
        {
            SelectedTabIndexChanged?.Invoke(this, e);
        }

        private void RaiseTabButtons()
        {
            foreach (var tabButton in Tabs.Where(t => t is TabButton))
            {
                // We always want our TabButton with the highest Z-index
#if NET6_0_OR_GREATER
                tabButton.ZIndex = 100;
#else
                _grid.RaiseChild(tabButton);
#endif
            }
        }

        private void UpdateTabOrientation()
        {
            InternalLogger.Debug(Tag, () => $"UpdateTabOrientation() => OrientationType: {Orientation}");

            _grid.BatchBegin();
            BatchBegin();

            if (_grid.RowDefinitions.Count != 0)
            {
                _grid.RowDefinitions.Clear();
            }

            if (_grid.ColumnDefinitions.Count != 0)
            {
                _grid.ColumnDefinitions.Clear();
            }

            int index = 0;
            foreach (var tabItem in Tabs)
            {
                int tabIndexInGrid = GetTabIndexInGrid(index);

                if (Orientation == OrientationType.Horizontal)
                {
                    _grid.ColumnDefinitions.Insert(tabIndexInGrid, new ColumnDefinition { Width = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto });

                    if (TabType == TabType.Scrollable)
                    {
                        if (Tabs.Count == 1)
                        {
                            // Add a last empty slot to fill remaining space
                            _lastFillingColumn = new ColumnDefinition { Width = GridLength.Star };
                            _grid.ColumnDefinitions.Add(_lastFillingColumn);
                        }
                        else
                        {
                            _grid.ColumnDefinitions.Remove(_lastFillingColumn);
                            _grid.ColumnDefinitions.Add(_lastFillingColumn);
                        }
                    }

                    Grid.SetRow(tabItem, 0);
                }
                else
                {
                    _grid.RowDefinitions.Insert(tabIndexInGrid, new RowDefinition { Height = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto });

                    if (TabType == TabType.Scrollable)
                    {
                        if (Tabs.Count == 1)
                        {
                            // Add a last empty slot to fill remaining space
                            _lastFillingRow = new RowDefinition { Height = GridLength.Star };
                            _grid.RowDefinitions.Add(_lastFillingRow);
                        }
                        else
                        {
                            _grid.RowDefinitions.Remove(_lastFillingRow);
                            _grid.RowDefinitions.Add(_lastFillingRow);
                        }
                    }

                    Grid.SetColumn(tabItem, 0);
                }

                RaiseTabButtons();
                AddTapCommand(tabItem);

                if (TabType == TabType.Fixed)
                {
                    tabItem.PropertyChanged += OnTabItemPropertyChanged;
                    UpdateTabVisibility(tabItem);
                }

                UpdateSelectableTabs();

                if (IsSegmented && SegmentedHasSeparator)
                {
                    ConsolidateSeparatedColumnIndexes();
                }
                else
                {
                    ConsolidateColumnIndexes();
                }

                ConsolidateSelectedIndex();
                index++;
            }

            BatchCommit();
            _grid.BatchCommit();
        }
    }
}