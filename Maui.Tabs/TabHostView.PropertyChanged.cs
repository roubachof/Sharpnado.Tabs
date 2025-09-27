using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Microsoft.Maui.Controls.Shapes;
using Sharpnado.Tasks;

namespace Sharpnado.Tabs;

public partial class TabHostView
{
    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(HeightRequest):
                if (_scrollView != null)
                {
                    _scrollView.HeightRequest = HeightRequest;
                }

                break;

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

        tabHost.InitializeIfNeeded();

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
        if (selectedIndex < 0 || tabHostView._fromTabItemTapped)
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

    private void ResetTabs()
    {
        InternalLogger.Debug(Tag, () => $"ResetTabs()");

        if (_grid is null) return;

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

        foreach (var tabItem in _grid.Children.OfType<TabItem>())
        {
            tabItem.PropertyChanged -= OnTabItemPropertyChanged;
            RemoveTouchEffectIfNeeded(tabItem);
        }

        _grid.Children.Clear();
        _selectableTabs.Clear();
        _lastFillingColumn = null;
        _lastFillingRow = null;

        _grid.BatchCommit();
        BatchCommit();
    }

    private void UpdateTabOrientation()
    {
        InternalLogger.Debug(Tag, () => $"UpdateTabOrientation() => OrientationType: {Orientation}");

        InitializeIfNeeded();

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
            index++;
        }

        BatchCommit();
        _grid.BatchCommit();
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

    private void InitializeItems()
    {
        if (ItemTemplate == null)
        {
            return;
        }

        InternalLogger.Debug(Tag, () => "InitializeItems()");
        Tabs.Clear();
        int index = 0;
        foreach (object model in ItemsSource ?? Array.Empty<object>())
        {
            var tabItem = CreateTabItem(model);
            Tabs.Insert(index++, tabItem);
        }
    }

    private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (ItemTemplate is null)
        {
            return;
        }

        InternalLogger.Debug(Tag, () => $"ItemsSourceCollectionChanged: {e.Action}");
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                int addedIndex = e.NewStartingIndex;
                foreach (object model in e.NewItems!)
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

            case NotifyCollectionChangedAction.Reset:
                InitializeItems();
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
            throw new InvalidOperationException(
                "Your ItemTemplate DataTemplate should contain a view inheriting from TabItem");
        }

        tabItem.BindingContext = item;

        InternalLogger.Debug(Tag, () => $"TabItem binding context changed to {item.GetType().Name}");
        return tabItem;
    }

    private void UpdateSegmentedOutlineColor()
    {
        InitializeIfNeeded();

        Border.Stroke = SegmentedOutlineColor;
        foreach (var separator in _grid.Children.Where(c => c is BoxView))
        {
            ((BoxView)separator).Color = SegmentedOutlineColor;
        }
    }

    private void UpdateBackgroundColor()
    {
        InitializeIfNeeded();

        if (IsSegmented)
        {
            _grid.BackgroundColor = Colors.Transparent;
            Border.BackgroundColor = BackgroundColor;
            return;
        }

        if (_grid == null)
        {
            return;
        }

        _grid.BackgroundColor = BackgroundColor;
        Border.BackgroundColor = Colors.Transparent;
    }

    private void UpdateCornerRadius()
    {
        InitializeIfNeeded();

        Border.StrokeShape = new RoundRectangle
        {
            CornerRadius = CornerRadius,
        };
    }

    private void AddSeparators()
    {
        for (int i = 0; i < _grid.Children.Count - 1; i++)
        {
            var currentItem = _grid.Children[i];
            var nextItem = _grid.Children[i + 1];
            if (currentItem is TabItem && nextItem is TabItem)
            {
                var separator = CreateSeparator();
                separator.SetBinding(IsVisibleProperty, new Binding(nameof(IsVisible), source: currentItem));
                _grid.Children.Insert(i + 1, separator);
            }
        }
    }

    private void RemoveSeparators()
    {
        foreach (var separator in _grid.Children.Where(c => c is BoxView)
                     .ToArray())
        {
            _grid.Children.Remove(separator);
        }
    }

    private BoxView CreateSeparator()
    {
        if (Orientation == OrientationType.Horizontal)
        {
            return new BoxView
            {
                Color = SegmentedOutlineColor,
                WidthRequest = 1,
            };
        }

        return new BoxView
        {
            Color = SegmentedOutlineColor,
            HeightRequest = 1,
        };
    }

    private CancellationTokenSource _scrollAnimationCts = new ();

    private void UpdateSelectedIndex(int selectedIndex)
    {
        if (_selectableTabs.Count == 0)
        {
            selectedIndex = -1;
        }

        if (selectedIndex > _selectableTabs.Count)
        {
            selectedIndex = _selectableTabs.Count - 1;
        }

        for (int index = 0; index < _selectableTabs.Count; index++)
        {
            _selectableTabs[index].IsSelected = selectedIndex == index;
        }

        SelectedIndex = selectedIndex;
        if (TabType == TabType.Scrollable && AutoScrollToSelectedTab)
        {
            var selectedTab = _selectableTabs[selectedIndex];
            TaskMonitor.Create(AnimateScrollToAsync(selectedTab));
        }

        InternalLogger.Debug(Tag, () => $"SelectedIndex: {SelectedIndex}, isFromItemTapped: {_fromTabItemTapped}");
    }

    private async Task AnimateScrollToAsync(TabItem selectedTab)
    {
        if (_scrollView == null)
            return;

        await _scrollAnimationCts.CancelAsync();
        _scrollAnimationCts = new CancellationTokenSource();
        var token = _scrollAnimationCts.Token;

        await Task.Delay(20, token);
        if (token.IsCancellationRequested)
            return;

        // Get the position and size of the selected tab
        double tabStart = selectedTab.X;
        double tabEnd = tabStart + selectedTab.Width;

        // Get the current scroll position and viewport width
        double scrollX = _scrollView.ScrollX;
        double viewportWidth = _scrollView.Width;

        // If the tab is not fully visible, scroll to it
        if (tabStart < scrollX)
        {
            await _scrollView.ScrollToAsync(tabStart, 0, true);
        }
        else if (tabEnd > scrollX + viewportWidth)
        {
            await _scrollView.ScrollToAsync(tabEnd - viewportWidth, 0, true);
        }
    }

    private void UpdateTabType()
    {
        InitializeIfNeeded();

        BatchBegin();

        InternalLogger.Debug(Tag, () => $"UpdateTabType() => TabType: {TabType}, IsSegmented: {IsSegmented}");

        if (IsSegmented)
        {
            Border.Content = _grid;
            Border.BackgroundColor = BackgroundColor;
            _grid.BackgroundColor = Colors.Transparent;
        }
        else
        {
            Border.Content = null;
            Border.BackgroundColor = Colors.Transparent;
            _grid.BackgroundColor = BackgroundColor;
        }

        if (TabType == TabType.Scrollable)
        {
            base.Content = _scrollView ??= new ScrollView
            {
                HeightRequest = HeightRequest,
                Orientation = Orientation == OrientationType.Horizontal
                    ? ScrollOrientation.Horizontal
                    : ScrollOrientation.Vertical,
                HorizontalScrollBarVisibility = ShowScrollbar ? ScrollBarVisibility.Always : ScrollBarVisibility.Never,
            };

            if (IsSegmented)
            {
                _scrollView.Content = Border;
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
                base.Content = Border;
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
}