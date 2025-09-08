using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Maui.Controls.Shapes;

using Sharpnado.Tabs.Effects;

namespace Sharpnado.Tabs;

public enum TabType
{
    Fixed = 0,
    Scrollable,
}

public enum OrientationType
{
    Horizontal = 0,
    Vertical,
}

[ContentProperty("Tabs")]
public partial class TabHostView : ContentView
{
    private const string Tag = nameof(TabHostView);

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
        false,
        BindingMode.OneWayToSource);

    public static readonly BindableProperty SegmentedOutlineColorProperty = BindableProperty.Create(
        nameof(SegmentedOutlineColor),
        typeof(Color),
        typeof(TabHostView),
        Colors.Magenta);

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(float),
        typeof(TabHostView),
        10f);

    public static readonly BindableProperty SegmentedHasSeparatorProperty = BindableProperty.Create(
        nameof(SegmentedHasSeparator),
        typeof(bool),
        typeof(TabHostView),
        false,
        propertyChanged: OnSegmentedHasSeparatorChanged);

    public static readonly BindableProperty TabTypeProperty = BindableProperty.Create(
        nameof(TabType),
        typeof(TabType),
        typeof(TabHostView),
        TabType.Fixed,
        BindingMode.OneWayToSource);

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(TabHostView),
        -1,
        BindingMode.TwoWay,
        propertyChanged: SelectedIndexPropertyChanged);

    public static readonly BindableProperty OrientationProperty = BindableProperty.Create(
        nameof(Orientation),
        typeof(OrientationType),
        typeof(TabHostView),
        OrientationType.Horizontal,
        propertyChanged: OrientationPropertyChanged);

    public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
        nameof(BackgroundColor),
        typeof(Color),
        typeof(TabHostView),
        Colors.Transparent);

    private Grid? _grid;

    private INotifyCollectionChanged? _currentNotifyCollection;

    private bool _fromTabItemTapped;

    private bool _hasBeenUnloaded;

    private ColumnDefinition? _lastFillingColumn;

    private RowDefinition? _lastFillingRow;

    private ScrollView? _scrollView;

    private List<TabItem> _selectableTabs = new();

    public TabHostView()
    {
        UpdateTabType();

        TabItemTappedCommand = new TapCommand(OnTabItemTapped);

        Tabs.CollectionChanged += OnTabsCollectionChanged;

        HandlerChanged += TabHostView_HandlerChanged;

        base.BackgroundColor = Colors.Transparent;
    }

    public event EventHandler<SelectedPositionChangedEventArgs>? SelectedTabIndexChanged;

    private void InitializeIfNeeded()
    {
        if (_grid is not null)
        {
            return;
        }

        Border = new Border
        {
            Padding = 0,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = CornerRadius,
            },

            Stroke = SegmentedOutlineColor,
        };

        _grid = new Grid
        {
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = BackgroundColor,
        };
    }

    public Border Border { get; private set; }

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
    ///     Only available if IsSegmented is true.
    /// </summary>
    public Color SegmentedOutlineColor
    {
        get => (Color)GetValue(SegmentedOutlineColorProperty);
        set => SetValue(SegmentedOutlineColorProperty, value);
    }

    /// <summary>
    ///     Only available if IsSegmented is true.
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
        set => throw new NotSupportedException("You can only add TabItem to the TabHostView through the Tabs property");
    }

    public View TabHostContent
    {
        set =>
            throw new NotSupportedException(
                "Starting from version 1.3, you can only add TabItem to the TabHostView through the Tabs property");
    }

    public bool ShowScrollbar { get; set; }

    private ICommand TabItemTappedCommand { get; }

    private void TabHostView_HandlerChanged(object? sender, EventArgs e)
    {
        InternalLogger.Debug(Tag, () => "HandlerChanged");
    }

    private void OnTabItemTapped(object tappedItem)
    {
        int selectedIndex = _selectableTabs.IndexOf((TabItem)tappedItem);

        if (selectedIndex == -1 || !_selectableTabs[selectedIndex].IsSelectable)
        {
            return;
        }

        _fromTabItemTapped = true;
        UpdateSelectedIndex(selectedIndex);
        RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        _fromTabItemTapped = false;
    }

    private void OnTabsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                int index = e.NewStartingIndex;
                foreach (var tab in e.NewItems!)
                {
                    OnChildAdded((TabItem)tab, index++);
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var tab in e.OldItems!)
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
        if (tabItem.GestureRecognizers.Any(gesture =>
                gesture is TapGestureRecognizer tapGestureRecognizer &&
                Equals(tapGestureRecognizer.Command, TabItemTappedCommand)))
        {
            return;
        }

        tabItem.GestureRecognizers.Add(
            new TapGestureRecognizer
            {
                Command = TabItemTappedCommand,
                CommandParameter = tabItem,
            });
    }

    private void OnChildAdded(TabItem tabItem, int index)
    {
        InternalLogger.Debug(Tag, () => $"OnChildAdded( tabItem: {tabItem.GetType().Name}, index: {index} )");

        ArgumentNullException.ThrowIfNull(_grid, nameof(_grid));

        _grid.BatchBegin();
        BatchBegin();

        int tabIndexInGrid = GetTabIndexInGrid(index);

        _grid.Insert(tabIndexInGrid, tabItem);
        if (Orientation == OrientationType.Horizontal)
        {
            _grid.ColumnDefinitions.Insert(
                tabIndexInGrid,
                new ColumnDefinition
                {
                    Width = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto,
                });

            if (TabType == TabType.Scrollable)
            {
                if (Tabs.Count == 1)
                {
                    // Add a last empty slot to fill remaining space
                    _lastFillingColumn = new ColumnDefinition
                    {
                        Width = GridLength.Star,
                    };
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
            _grid.RowDefinitions.Insert(
                tabIndexInGrid,
                new RowDefinition
                {
                    Height = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto,
                });

            if (TabType == TabType.Scrollable)
            {
                if (Tabs.Count == 1)
                {
                    // Add a last empty slot to fill remaining space
                    _lastFillingRow = new RowDefinition
                    {
                        Height = GridLength.Star,
                    };
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

            var previousElementAt = _grid.Children.Where(v => v is TabItem)
                .ElementAtOrDefault(index - 1);
            int indexInGrid = _grid.Children.IndexOf(previousElementAt) + 1;

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

        int tabItemIndex = _grid.Children.IndexOf(tabItem);

        InternalLogger.Debug(Tag, () => $"OnChildRemoved( tabItem: {tabItem.GetType().Name}, index: {tabItemIndex} )");

        if (tabItemIndex > 1 && _grid.Children[tabItemIndex - 1] is BoxView)
        {
            _grid.RemoveAt(tabItemIndex - 1);
            _grid.ColumnDefinitions.RemoveAt(tabItemIndex - 1);
            tabItemIndex--;
        }

        _grid.RemoveAt(tabItemIndex);
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
        _selectableTabs = Tabs.Where(t => t.IsSelectable)
            .ToList();
    }

    private void ConsolidateColumnIndexes()
    {
        int index = 0;
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

        int index = 0;
        while (index < _grid.Children.Count)
        {
            var currentItem = _grid.Children[index];

            bool previousItemIsTab = index > 0 && _grid.Children[index - 1] is TabItem;
            bool currentItemIsTab = currentItem is TabItem;

            if (previousItemIsTab && currentItemIsTab)
            {
                var separator = CreateSeparator();
                separator.SetBinding(IsVisibleProperty, new Binding(nameof(IsVisible), source: currentItem));

                if (Orientation == OrientationType.Horizontal)
                {
                    _grid.ColumnDefinitions.Insert(
                        index,
                        new ColumnDefinition
                        {
                            Width = separator.WidthRequest,
                        });
                }
                else
                {
                    _grid.RowDefinitions.Insert(
                        index,
                        new RowDefinition
                        {
                            Height = separator.HeightRequest,
                        });
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

            bool previousItemIsSeparator = index > 0 && _grid.Children[index - 1] is BoxView;
            bool currentItemIsSeparator = currentItem is BoxView;

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

    private void OnTabItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var tabItem = (TabItem)sender!;
        if (e.PropertyName == nameof(IsVisible))
        {
            UpdateTabVisibility(tabItem);
        }
    }

    private void UpdateTabVisibility(TabItem tabItem)
    {
        ArgumentNullException.ThrowIfNull(_grid, nameof(_grid));

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
            tabButton.ZIndex = 1000;
        }
    }
}