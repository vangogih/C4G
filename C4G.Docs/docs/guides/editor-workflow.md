---
sidebar_position: 4
---

# Editor Workflow

Learn how to use C4G in the Unity Editor to generate configuration code and data from Google Sheets.

## Overview

The C4G Editor workflow involves:
1. Configuring settings (one-time setup)
2. Generating C# classes and JSON configs
3. Using the generated configs in your game code

## Opening C4G Windows

C4G provides two main Editor windows:

### C4G Settings Window

**Menu**: `Window → C4G → Settings`

Use this window to configure:
- Google Sheets connection
- Output folders
- Custom type parsers
- Sheet definitions

### C4G Generation Window

**Menu**: `Window → C4G → Generate`

Use this window to:
- Generate configs from Google Sheets
- View generation progress
- See errors and warnings

## First-Time Setup

### Step 1: Configure Basic Settings

1. Open `Window → C4G → Settings`

2. Configure the following required fields:

#### Google Sheets ID

- **Label**: "Table ID" or "Spreadsheet ID"
- **Value**: Your spreadsheet ID from the Google Sheets URL
- **Example**: `1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms`

#### Client Secret

- **Label**: "Client Secret"
- **Value**: Path to your OAuth credentials JSON file
- **Example**: `C:/Projects/MyGame/Credentials/c4g_credentials.json`
- **Note**: Use absolute paths

#### Root Config Name

- **Label**: "Root Config Name"
- **Value**: Name for the main config container class
- **Example**: `GameConfig`
- **Default**: `RootConfig`

#### Generated Code Folder

- **Label**: "Generated Code Folder"
- **Value**: Where C# classes will be generated
- **Example**: `Assets/Scripts/Generated/`
- **Best Practice**: Keep generated code in a dedicated folder

#### Serialized Configs Folder

- **Label**: "Serialized Configs Folder"
- **Value**: Where JSON files will be saved
- **Example**: `Assets/Resources/Configs/`
- **Best Practice**: Use `Resources/` for runtime loading

### Step 2: Define Sheet Parsers

Sheet parsers tell C4G which sheets to read from your spreadsheet.

1. In the Settings window, find the **"Sheet Parsers"** section

2. Click **"+"** to add a new sheet

3. Configure each sheet:
   - **Sheet Name**: Must match the exact name in Google Sheets (case-sensitive)
   - **Parser Type**: Usually "Default" (advanced users can create custom parsers)

4. Example configuration:
   ```
   Sheet Name: Items
   Parser Type: Default Sheet Parser

   Sheet Name: Enemies
   Parser Type: Default Sheet Parser

   Sheet Name: LevelConfigs
   Parser Type: Default Sheet Parser
   ```

### Step 3: Configure Custom Type Parsers (Optional)

If you're using enums or custom types:

1. Find the **"Alias Parsers"** section

2. Click **"+"** to add a custom type

3. Configure:
   - **Alias Name**: The type name as it appears in Google Sheets
   - **Parser Type**: Select the appropriate parser (Enum, Custom, etc.)

4. Example for enums:
   ```
   Alias Name: ItemRarity
   Parser Type: Enum Parser
   Enum Type: MyGame.ItemRarity
   ```

## Generating Configs

### Basic Generation

1. Open `Window → C4G → Generate`

2. Click the **"Generate Configs"** button

3. C4G will:
   - Connect to Google Sheets (may open browser for OAuth first time)
   - Fetch sheet data
   - Parse the data
   - Generate C# classes
   - Serialize JSON files
   - Display results in the console

4. Wait for the success message:
   ```
   C4G: Config generation successful!
   ```

### First-Time Authorization

On first generation, you'll need to authorize:

1. Your default browser will open automatically

2. Sign in to Google (if not already)

3. Review and accept the permissions

4. Return to Unity - generation will continue automatically

5. Authorization is saved - you won't need to repeat this

### Generation Progress

During generation, watch the Console for progress:

```
C4G: Starting config generation...
C4G: Connecting to Google Sheets...
C4G: Loading sheet 'Items'...
C4G: Loading sheet 'Enemies'...
C4G: Parsing sheet 'Items'...
C4G: Parsing sheet 'Enemies'...
C4G: Generating class 'Items.cs'...
C4G: Generating class 'Enemies.cs'...
C4G: Generating root config 'GameConfig.cs'...
C4G: Serializing configs to JSON...
C4G: Config generation successful!
```

### Verifying Generated Files

After successful generation, verify the output:

1. **C# Classes** (in Generated Code Folder):
   ```
   Assets/Scripts/Generated/
   ├── Items.cs
   ├── Enemies.cs
   └── GameConfig.cs
   ```

2. **JSON Configs** (in Serialized Configs Folder):
   ```
   Assets/Resources/Configs/
   └── GameConfig.json
   ```

3. **Check Unity Console** for any warnings or errors

## Using Generated Configs in Code

### Loading Configs at Runtime

```csharp
using UnityEngine;
using Newtonsoft.Json;

public class ConfigManager : MonoBehaviour
{
    public GameConfig gameConfig;

    void Start()
    {
        LoadConfigs();
    }

    void LoadConfigs()
    {
        // Load from Resources folder
        TextAsset configFile = Resources.Load<TextAsset>("Configs/GameConfig");

        if (configFile != null)
        {
            gameConfig = JsonConvert.DeserializeObject<GameConfig>(configFile.text);
            Debug.Log($"Loaded {gameConfig.items.Length} items");
        }
        else
        {
            Debug.LogError("Config file not found!");
        }
    }
}
```

### Accessing Config Data

```csharp
// Access items
foreach (var item in gameConfig.items)
{
    Debug.Log($"Item: {item.name}, Price: {item.price}");
}

// Find specific item by ID
var sword = System.Array.Find(gameConfig.items, i => i.id == 1);
if (sword != null)
{
    Debug.Log($"Found: {sword.name}");
}

// Access enemies
foreach (var enemy in gameConfig.enemies)
{
    Debug.Log($"Enemy: {enemy.name}, Health: {enemy.health}");
}
```

### Creating a Config Singleton

```csharp
public class ConfigManager : MonoBehaviour
{
    private static ConfigManager _instance;
    public static ConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ConfigManager>();
            }
            return _instance;
        }
    }

    public GameConfig Config { get; private set; }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        LoadConfigs();
    }

    void LoadConfigs()
    {
        TextAsset configFile = Resources.Load<TextAsset>("Configs/GameConfig");
        Config = JsonConvert.DeserializeObject<GameConfig>(configFile.text);
    }
}

// Usage in other scripts:
var itemConfig = ConfigManager.Instance.Config.items[0];
```

## Development Workflow

### Recommended Workflow

1. **Design configs** in Google Sheets
2. **Test data** with a few sample rows
3. **Generate configs** in Unity (`Window → C4G → Generate`)
4. **Verify generated code** compiles without errors
5. **Test in play mode** with sample data
6. **Add full dataset** to Google Sheets
7. **Regenerate configs**
8. **Commit generated JSON** to version control
9. **Optionally commit C# classes** (team preference)

### When to Regenerate

Regenerate configs when:
- ✅ Adding new fields to a sheet
- ✅ Adding new sheets
- ✅ Changing data values
- ✅ Modifying field types
- ✅ Adding/removing data rows

You don't need to regenerate for:
- ❌ Code-only changes
- ❌ Unity scene changes
- ❌ UI tweaks

### Version Control

**Recommended approach**:

1. **Commit generated JSON files**:
   - These are your runtime configs
   - Team members need them to run the game
   - Small file sizes

2. **`.gitignore` generated C# classes** (optional):
   - They can be regenerated from sheets
   - Reduces merge conflicts
   - Each developer regenerates locally

**Alternative approach**:

1. **Commit both JSON and C# files**:
   - Useful if not all team members have OAuth access
   - Provides full config snapshot
   - More merge conflicts possible

## Keyboard Shortcuts

C4G doesn't have built-in shortcuts, but you can create your own:

1. Create a script in `Assets/Editor/`:

```csharp
using UnityEditor;
using C4G.Editor;

public class C4GShortcuts
{
    [MenuItem("Tools/C4G/Generate Configs _g")]  // Alt+G
    static void GenerateConfigs()
    {
        // Open C4G Generate window
        EditorWindow.GetWindow<C4GWindow>();

        // Trigger generation (you'll need to access the window's method)
    }

    [MenuItem("Tools/C4G/Open Settings _s")]  // Alt+S
    static void OpenSettings()
    {
        EditorWindow.GetWindow<C4GSettingsProvider>();
    }
}
```

## Troubleshooting

### "Generation failed" errors

**Check**:
1. Console for specific error messages
2. Internet connection
3. Google Sheets permissions
4. OAuth credentials validity

### Generated classes have compile errors

**Common causes**:
- Reserved C# keywords used as field names
- Invalid type names
- Circular dependencies

**Solutions**:
- Rename fields in Google Sheets
- Use valid C# type names
- Check field name spelling

### JSON file not found at runtime

**Causes**:
- File not in `Resources/` folder
- Wrong file path in code
- File not committed (if cloning repo)

**Solutions**:
- Verify file is in `Assets/Resources/Configs/`
- Check file name matches code
- Regenerate configs

### OAuth authorization keeps prompting

**Causes**:
- Token expired
- Token file deleted
- Credentials changed

**Solutions**:
- Re-authorize (one-time)
- Check credentials file hasn't moved
- Verify credentials are still valid in Google Cloud Console

## Advanced Tips

### Batch Processing

For multiple config updates:

1. Update all sheets in Google Sheets
2. Generate once - C4G processes all sheets together
3. Review all changes in one commit

### Config Validation

Add validation after loading:

```csharp
void ValidateConfigs()
{
    foreach (var item in gameConfig.items)
    {
        if (item.price < 0)
        {
            Debug.LogWarning($"Item {item.name} has negative price!");
        }
    }
}
```

### Hot Reloading (Advanced)

For rapid iteration during development:

```csharp
#if UNITY_EDITOR
[UnityEditor.MenuItem("Tools/Reload Configs")]
static void ReloadConfigs()
{
    var manager = FindObjectOfType<ConfigManager>();
    manager.LoadConfigs();
    Debug.Log("Configs reloaded!");
}
#endif
```

## Next Steps

- [Supported Versions](./supported-versions) - Check compatibility
- [API Reference](../api/introduction) - Understand C4G's API
- [Custom Type Parsers](./overview) - Extend C4G functionality
