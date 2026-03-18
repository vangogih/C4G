---
sidebar_position: 2
---

# Installation

This guide covers installing C4G for both Unity and standalone .NET projects.

## System Requirements

### Unity Package

- **Unity Version**: 2019.4 or later
- **Target Framework**: .NET Standard 2.0 or .NET 4.x
- **Operating System**: Windows, macOS, or Linux
- **Supported Platforms**: Editor only (runtime loading of configs works on all platforms)

### Standalone .NET Package

- **.NET Version**: .NET Standard 2.0 compatible
- **Operating System**: Cross-platform (Windows, macOS, Linux)

## Unity Package Installation

### Method 1: Unity Package Manager (Recommended)

1. Download the latest `.unitypackage` from [GitHub Releases](https://github.com/vangogih/C4G/releases)

2. In Unity, open the Package Manager:
   - `Window → Package Manager`

3. Import the custom package:
   - `Assets → Import Package → Custom Package`
   - Select the downloaded `C4G-v*.unitypackage` file
   - Click `Import` to import all files

### Method 2: Manual Installation

1. Download and extract the latest release from GitHub

2. Copy the `C4G` folder to your Unity project:
   ```
   YourProject/
   └── Assets/
       └── C4G/
           ├── Core/
           ├── Editor/
           ├── Plugins/
           └── package.json
   ```

3. Unity will automatically detect and import the package

### Verify Installation

After installation, verify C4G is properly installed:

1. Open Unity Editor
2. Check the menu bar for `Window → C4G`
3. You should see two menu items:
   - `Window → C4G → Settings`
   - `Window → C4G → Generate`

If these menu items appear, the installation was successful!

## .NET Standard Package Installation

For standalone .NET projects, install C4G via NuGet:

### Using .NET CLI

```bash
dotnet add package C4G
```

### Using Package Manager Console (Visual Studio)

```powershell
Install-Package C4G
```

### Using PackageReference

Add to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="C4G" Version="0.0.4" />
</ItemGroup>
```

## Dependencies

C4G includes the following dependencies (automatically included):

### Google Sheets API

- `Google.Apis.dll`
- `Google.Apis.Core.dll`
- `Google.Apis.Auth.dll`
- `Google.Apis.Sheets.v4.dll`

### JSON Serialization

- `Newtonsoft.Json.dll`

These dependencies are located in `Assets/C4G/Plugins/` for Unity installations.

## Post-Installation Setup

After installing C4G, you'll need to:

1. **Configure Google Sheets API** - [OAuth 2.0 Setup Guide](./guides/oauth-setup)
2. **Set up your first Google Sheet** - [Google Sheets Setup Guide](./guides/google-sheets-setup)
3. **Configure C4G Settings** - Open `Window → C4G → Settings` in Unity

## Updating C4G

### Unity Package

1. Download the latest version from [GitHub Releases](https://github.com/vangogih/C4G/releases)
2. Delete the existing `Assets/C4G` folder (backup your generated configs first!)
3. Import the new `.unitypackage` following the installation steps above
4. Reconfigure your C4G settings if needed

### .NET Package

```bash
dotnet add package C4G --version [new-version]
```

Or update via Package Manager in Visual Studio.

## Troubleshooting

### "C4G menu items not appearing"

**Solution**:
- Restart Unity Editor
- Check Console for import errors
- Ensure Unity version is 2019.4 or later

### "Google APIs DLL errors"

**Solution**:
- Verify all DLLs in `Assets/C4G/Plugins/` are imported
- Check the Inspector for each DLL - ensure they're set for Editor platform
- Reimport the package if needed

### "Namespace 'C4G' not found"

**Solution**:
- Check your assembly definition references C4G.Core
- Ensure the C4G.Core.asmdef file exists
- Restart Unity to refresh assembly references

### "Version conflicts with existing packages"

**Solution**:
- C4G uses `Newtonsoft.Json.dll` - if you have another version, you may need to remove one
- For Google APIs conflicts, ensure you're not importing another Google Sheets integration

## Uninstallation

### Unity Package

1. Close Unity Editor
2. Delete the `Assets/C4G` folder from your project
3. Delete any generated config files (optional)
4. Restart Unity

### .NET Package

```bash
dotnet remove package C4G
```

## Next Steps

- [OAuth 2.0 Setup](./guides/oauth-setup) - Configure Google Sheets API access
- [Google Sheets Setup](./guides/google-sheets-setup) - Create your first config sheet
- [Editor Workflow](./guides/editor-workflow) - Learn how to use C4G in Unity Editor
