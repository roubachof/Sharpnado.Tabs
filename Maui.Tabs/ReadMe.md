# Pure MAUI Tabs
* Fixed tabs (android tabs style)
* Scrollable tabs
* Vertical tabs
* Material design tabs (top and leading icon)
* Lazy and Delayed views
* Support for SVG images
* Segmented tabs
* Custom shadows (neumorphism ready)
* Badges on tabs
* Circle button in tab bar
* Bottom bar tabs (ios tabs style)
* Custom tabs (be creative just implement TabItem)
* Independent ViewSwitcher
* Bindable with ItemsSource
* Pure MAUI touch effects

## Installation

* In Core project, in `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp()
        .UseSharpnadoTabs(loggerEnabled: false);
}
```

## Usage

First create a `TabHostView` which will contains all your tabs:

```xml
<tabs:TabHostView WidthRequest="250"
                  HeightRequest="60"
                  Padding="20,0"
                  HorizontalOptions="Center"
                  BackgroundColor="{StaticResource Gray900}"
                  CornerRadius="30"
                  IsSegmented="True"
                  Orientation="Horizontal"
                  SegmentedOutlineColor="{StaticResource Gray950}"
                  SelectedIndex="{Binding Source={x:Reference Switcher}, Path=SelectedIndex, Mode=TwoWay}"
                  TabType="Fixed">
    <tabs:TabHostView.Shadow>
        <Shadow Brush="{StaticResource Primary}"
                Opacity="0.7"
                Radius="30"
                Offset="0,10" />
    </tabs:TabHostView.Shadow>
    <tabs:BottomTabItem Style="{StaticResource BottomTab}" Label="M" />
    <tabs:BottomTabItem Style="{StaticResource BottomTab}" Label="A">
        <tabs:BottomTabItem.Badge>
            <tabs:BadgeView BackgroundColor="{StaticResource Tertiary}" Text="new" />
        </tabs:BottomTabItem.Badge>
    </tabs:BottomTabItem>
    <tabs:UnderlinedTabItem FontFamily="OpenSansExtraBold"
                            Label="U"
                            LabelSize="36"
                            SelectedTabColor="{StaticResource Primary}"
                            UnselectedLabelColor="{StaticResource White}" />
    <tabs:BottomTabItem Style="{StaticResource BottomTab}"
                        Padding="0,0,10,0"
                        Label="I">
        <tabs:BottomTabItem.Badge>
            <tabs:BadgeView BackgroundColor="{StaticResource Tertiary}" Text="2" />
        </tabs:BottomTabItem.Badge>
    </tabs:BottomTabItem>
</tabs:TabHostView>
```


Then bind the `SelectedIndex` with a `ViewSwitcher` that will accordingly select your views.

```xml
<tabs:ViewSwitcher x:Name="Switcher"
                           Grid.RowSpan="3"
                           Margin="0"
                           Animate="True"
                           SelectedIndex="{Binding SelectedViewModelIndex, Mode=TwoWay}">
    <tabs:DelayedView x:TypeArguments="views:TabM"
                        AccentColor="{StaticResource Primary}"
                        Animate="True"
                        BindingContext="{Binding HomePageViewModel}"
                        UseActivityIndicator="True" />
    <tabs:DelayedView x:TypeArguments="views:TabA"
                        AccentColor="{StaticResource Primary}"
                        Animate="True"
                        UseActivityIndicator="True" />
    <tabs:DelayedView x:TypeArguments="views:TabU"
                        AccentColor="{StaticResource Primary}"
                        Animate="True"
                        UseActivityIndicator="True" />
    <tabs:LazyView x:TypeArguments="views:TabI" Animate="True" />
</tabs:ViewSwitcher>
```