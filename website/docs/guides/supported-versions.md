---
sidebar_position: 5
---

# Supported Versions

This page documents C4G version compatibility with Unity, .NET, and related dependencies.

## Unity Compatibility

### Supported Unity Versions

C4G officially supports:

| Unity Version | Support Status | Notes                              |
|---------------|----------------|------------------------------------|
| **2019.4 LTS** | ✅ Full Support | Minimum required version          |
| **2020.3 LTS** | ✅ Full Support | Recommended for stability         |
| **2021.3 LTS** | ✅ Full Support | Recommended for new projects      |
| **2022.3 LTS** | ✅ Full Support | Latest LTS, fully tested          |
| **2023.1+**    | ✅ Full Support | Latest versions supported         |
| **6000.0+**    | ⚠️ Experimental | Unity 6 - testing in progress    |

### Unity Version Selection

**Recommended**:
- **New Projects**: Unity 2022.3 LTS or later
- **Existing Projects**: Stay on your current LTS
- **Production**: Use LTS versions for stability

**Not Supported**:
- Unity 2018.x and earlier
- Unity 2019.3 and earlier (non-LTS)

## .NET Compatibility

### Target Frameworks

C4G is built on .NET Standard 2.0, providing broad compatibility:

| Framework            | Support Status | C4G Package |
|----------------------|----------------|-------------|
| **.NET Standard 2.0** | ✅ Full Support | Unity & Standalone |
| **.NET Standard 2.1** | ✅ Full Support | Unity & Standalone |
| **.NET 4.x**          | ✅ Full Support | Unity only |
| **.NET 5+**           | ✅ Full Support | Standalone only |
| **.NET Core 3.1**     | ✅ Full Support | Standalone only |
| **.NET Framework 4.7+**| ✅ Full Support | Standalone only |

### Unity Scripting Backend

| Backend           | Support Status | Notes                       |
|-------------------|----------------|-----------------------------|
| **Mono**          | ✅ Full Support | Default for Unity Editor   |
| **IL2CPP**        | ⚠️ Limited     | Editor-only tool           |

**Note**: C4G is an **Editor-only** tool. Generated configs work on all backends.

## Platform Compatibility

### Unity Editor Platforms

C4G runs in the Unity Editor on:

| Platform        | Support Status | Notes                          |
|-----------------|----------------|--------------------------------|
| **Windows**     | ✅ Full Support | Tested on Windows 10/11       |
| **macOS**       | ✅ Full Support | Tested on macOS 12+           |
| **Linux**       | ✅ Full Support | Tested on Ubuntu 20.04+       |

### Runtime Platforms (Generated Configs)

Generated configs can be used on **all Unity platforms**:

- ✅ Windows (Standalone)
- ✅ macOS (Standalone)
- ✅ Linux (Standalone)
- ✅ iOS
- ✅ Android
- ✅ WebGL
- ✅ Console platforms (PlayStation, Xbox, Switch)

**C4G itself doesn't run at runtime** - only the generated JSON configs are loaded.

## Dependency Versions

### Google Sheets API

C4G bundles specific versions of Google APIs:

| Dependency                 | Version   | Location                      |
|----------------------------|-----------|-------------------------------|
| Google.Apis.dll            | Latest    | Assets/C4G/Plugins/           |
| Google.Apis.Core.dll       | Latest    | Assets/C4G/Plugins/           |
| Google.Apis.Auth.dll       | Latest    | Assets/C4G/Plugins/           |
| Google.Apis.Sheets.v4.dll  | v4        | Assets/C4G/Plugins/           |

**Updating Google APIs**:
- Not recommended unless necessary
- May cause compatibility issues
- Test thoroughly if updating

### Newtonsoft.Json

| Version           | Support Status | Notes                              |
|-------------------|----------------|------------------------------------|
| **12.0.3+**       | ✅ Recommended | Bundled with C4G                  |
| **11.0.0 - 12.x** | ✅ Compatible  | Works but may have minor issues   |
| **13.0.0+**       | ✅ Compatible  | Latest versions work              |

**Conflicts**:
- If your project uses a different Newtonsoft.Json version, you may see warnings
- Usually safe to ignore or use the latest version
- Remove duplicate DLLs if issues occur

## C4G Version History

### Current Version: 0.0.4

**Release Date**: Check [GitHub Releases](https://github.com/vangogih/C4G/releases)

**Features**:
- Google Sheets integration
- Code generation for C# DTOs
- JSON serialization
- Custom type parsers
- Enum support
- List/array support

### Previous Versions

| Version | Release Date | Status      | Notes                     |
|---------|--------------|-------------|---------------------------|
| 0.0.4   | Latest       | ✅ Current  | Stable release           |
| 0.0.3   | Previous     | ⚠️ Legacy   | Upgrade recommended      |
| 0.0.2   | Older        | ❌ Deprecated| Not supported           |
| 0.0.1   | Initial      | ❌ Deprecated| Not supported           |

**Upgrade Path**:
1. Backup your current project
2. Delete `Assets/C4G` folder
3. Import latest `.unitypackage`
4. Reconfigure settings
5. Test generation

## Breaking Changes

### Version 0.0.4

No breaking changes from 0.0.3.

### Version 0.0.3

- Introduced modular architecture
- May require reconfiguration of custom type parsers

### Upgrading from 0.0.2 or earlier

**Breaking changes**:
- Settings structure changed
- Custom parser registration changed
- File output structure modified

**Migration steps**:
1. Note current settings (spreadsheet ID, folders)
2. Remove old C4G package
3. Install new version
4. Reconfigure settings from scratch
5. Regenerate all configs

## Unity Package Versions

C4G may interact with these common Unity packages:

| Package                    | Compatibility | Notes                        |
|----------------------------|---------------|------------------------------|
| **TextMesh Pro**           | ✅ Compatible | No conflicts                |
| **Unity UI (uGUI)**        | ✅ Compatible | No conflicts                |
| **Addressables**           | ✅ Compatible | Use for config loading      |
| **JSON .NET for Unity**    | ⚠️ Conflict  | Remove if using C4G's version|

## Checking Your Version

### In Unity

1. Select any C4G file in Project window
2. Check Inspector for package version
3. Or check `Assets/C4G/package.json`:

```json
{
  "version": "0.0.4",
  "displayName": "C4G"
}
```

### Via Package Manager

1. `Window → Package Manager`
2. Select `C4G` from list
3. Version shown in details panel

### Via Code

```csharp
using C4G.Core;

Debug.Log($"C4G Version: {AssemblyReference.Version}");
```

## Version Support Policy

### Long-Term Support (LTS)

- **Current version (0.0.4+)**: Full support, active development
- **Previous minor version**: Bug fixes only
- **Older versions**: No support, upgrade recommended

### Security Updates

- Critical security issues: Fixed in all supported versions
- Non-critical issues: Fixed in current version only

### Bug Reports

When reporting bugs, please include:
- C4G version
- Unity version
- Operating system
- Google Sheets API version
- Newtonsoft.Json version

## Compatibility Testing

C4G is tested on:

| Environment                    | Status          |
|--------------------------------|-----------------|
| Windows 11 + Unity 2022.3 LTS  | ✅ Primary test |
| macOS 14 + Unity 2022.3 LTS    | ✅ Regular test |
| Ubuntu 22.04 + Unity 2022.3    | ✅ Regular test |
| Windows 10 + Unity 2021.3 LTS  | ✅ Regular test |
| Windows 11 + Unity 2020.3 LTS  | ✅ Regular test |
| Windows 11 + Unity 2019.4 LTS  | ✅ Minimum test |

**Not tested**:
- Unity versions below 2019.4
- Deprecated OS versions
- Mobile OS for editor (not applicable)

## Future Compatibility

### Planned Support

- Unity 6 LTS (when released)
- .NET 8+ for standalone package
- Google Sheets API v5 (when available)

### Deprecation Timeline

No major features planned for deprecation. Any deprecations will be:
- Announced in release notes
- Supported for 2+ major versions
- Provided with migration guides

## Getting Help

### Version-Specific Issues

- Check [GitHub Issues](https://github.com/vangogih/C4G/issues) for known problems
- Include version numbers in bug reports
- Search for version-specific workarounds

### Upgrade Assistance

- [Installation Guide](../installation) - Fresh install instructions
- [Migration Guides](https://github.com/vangogih/C4G/releases) - Version-specific upgrades
- [GitHub Discussions](https://github.com/vangogih/C4G/discussions) - Community help

## Next Steps

- [Installation](../installation) - Install or upgrade C4G
- [Getting Started](../getting-started) - Quick start guide
- [Editor Workflow](./editor-workflow) - Using C4G in Unity
