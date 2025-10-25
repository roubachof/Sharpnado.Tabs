# Release Notes v4.0.1

## 📦 Windows Platform Inclusion

**Type:** Packaging Fix

This is a re-release of v4.0.0 to include Windows platform binaries.

## 🐛 Issue

Version 4.0.0 was packaged on macOS, which resulted in the Windows target framework (`net9.0-windows10.0.19041.0`) being excluded from the NuGet package.

## ✅ Fix

Version 4.0.1 includes all platform binaries:
- ✅ `net9.0` - .NET 9 base
- ✅ `net9.0-android` - Android
- ✅ `net9.0-ios` - iOS
- ✅ `net9.0-maccatalyst` - MacCatalyst
- ✅ `net9.0-windows10.0.19041.0` - Windows (now included)

## 📝 Code Changes

**No code changes** - This release is functionally identical to v4.0.0.

All features from v4.0.0 remain unchanged:
- Pure MAUI touch effects (CircularRipple, Standard, PoorsManRipple)
- TouchEffectType and TouchColor properties
- Enhanced platform support (MacCatalyst and Windows)
- Improved SelectedItem handling
- Scroll-to-selected functionality
- SelectedIconSource support

## 📦 Installation

```bash
dotnet add package Sharpnado.Tabs.Maui --version 4.0.1
```

## 🔄 Migration from 4.0.0

Simply update the package version. No code changes required.

Windows developers who were unable to use v4.0.0 can now use v4.0.1.

## 📚 Documentation

For full v4.0.0 feature documentation, see the main README: https://github.com/roubachof/Sharpnado.Tabs

Repository: https://github.com/roubachof/Sharpnado.Tabs
