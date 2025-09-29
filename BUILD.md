# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

Sharpnado.Tabs is a .NET MAUI library that provides highly customizable tab controls with no native handlers or renderers. It's a "Pure MAUI" implementation that supports multiple tab types, animations, and touch effects across Android, iOS, Windows, and MacCatalyst.

## Build Commands

### Library Build
```bash
# Build the main library
dotnet build Maui.Tabs/Maui.Tabs.csproj -c Release

# Build library in debug mode
dotnet build Maui.Tabs/Maui.Tabs.csproj -c Debug

# Create NuGet package (automatically done with Release builds due to PackOnBuild=true)
dotnet pack Maui.Tabs/Maui.Tabs.csproj -c Release
```

### Sample Applications
```bash
# Build the main sample app
dotnet build MauiSample/MauiSample.csproj -c Debug

# Build for specific platforms
dotnet build MauiSample/MauiSample.csproj -f net9.0-android -c Debug
dotnet build MauiSample/MauiSample.csproj -f net9.0-ios -c Debug
dotnet build MauiSample/MauiSample.csproj -f net9.0-maccatalyst -c Debug

# Restore dependencies for sample
dotnet restore MauiSample/MauiSample.sln

# Build Shell sample
dotnet build MauiShellSample/MauiShellSample.csproj -c Debug
```

### Solution Level
```bash
# Using solution files
dotnet build MauiSample/MauiSample.sln
dotnet build Maui.Tabs/Maui.Tabs.sln
```

## Development Environment

### Target Frameworks
- **Library**: net9.0, net9.0-android, net9.0-ios, net9.0-maccatalyst, net9.0-windows10.0.19041.0
- **Samples**: net9.0-android, net9.0-ios, net9.0-maccatalyst, net9.0-windows10.0.19041.0

### Platform Requirements
- **iOS**: 14.2+
- **MacCatalyst**: 15.0+
- **Android**: API 21+
- **Windows**: 10.0.17763.0+

## Architecture Overview

### Core Components

**TabHostView** (`TabHostView.cs`): The main container that hosts tabs. Supports multiple tab types (Fixed, Scrollable), orientations (Horizontal, Vertical), and segmented controls. Key properties:
- `SelectedIndex`: Bindable property for two-way binding with ViewSwitcher
- `TabType`: Fixed or Scrollable layout
- `Orientation`: Horizontal or Vertical tab arrangement
- `IsSegmented`: Enables segmented control appearance

**ViewSwitcher** (`ViewSwitcher.cs`): Independent view container that switches between content based on SelectedIndex. Features:
- Animation support through `IAnimatableReveal` interface
- Lazy loading support through `ILazyView` interface
- Two-way binding with TabHostView via SelectedIndex

**Tab Item Types**:
- `UnderlinedTabItem`: Material Design underlined tabs
- `BottomTabItem`: iOS-style bottom navigation tabs
- `MaterialUnderlinedTabItem`: Full Material Design spec implementation with icon options
- `SegmentedTabItem`: iOS segmented control style
- `TabButton`: Action buttons within tab bars
- `TabItem` (abstract): Base class for custom tab implementations

### Specialized Components

**LazyView** (`LazyView.cs`): Performance optimization component that delays view creation until needed. Reduces app startup time by deferring UI construction.

**DelayedView** (`DelayedView.cs`): Evolution of LazyView with configurable delay timing for fine-tuned UI building control. Includes activity indicator support.

**BadgeView** (`BadgeView.cs`): Chip/badge overlay component for tabs with text, numbers, or indicator dots.

### Touch and Animation System

The library implements pure MAUI touch effects without custom handlers:
- Circular ripple effects
- Standard touch feedback
- Custom ripple implementations
- Animation support through `IAnimatableReveal` interface

### Key Design Patterns

**Component Separation**: TabHostView and ViewSwitcher are independent components connected only through SelectedIndex binding, allowing flexible layouts.

**Extensible Tab System**: All tab items inherit from abstract `TabItem` base class, enabling unlimited customization without modifying core library.

**Property-Driven Configuration**: Extensive use of bindable properties for styling and behavior modification without subclassing.

## Library Usage Initialization

In MAUI applications using this library, initialize in `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseSharpnadoTabs(loggerEnable: false);

    return builder.Build();
}
```

## Code Style and Analysis

- Uses StyleCop analyzers with custom ruleset (`Configuration/StyleCopRules.ruleset`)
- C# nullable reference types enabled
- Latest C# language version
- Code analysis enforced during build

## NuGet Package Information

- **Package ID**: `Sharpnado.Tabs.Maui`
- **Current Version**: 4.0.0
- **License**: MIT
- **Auto-packaging**: Enabled for Release builds