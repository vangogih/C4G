---
sidebar_position: 3
---

# Google Sheets Setup

Learn how to structure your Google Sheets for C4G configuration generation.

## Overview

C4G reads data from Google Sheets and generates:
- C# DTO classes (Data Transfer Objects)
- JSON configuration files

Each sheet in your spreadsheet becomes a separate config class.

## Creating Your First Config Sheet

### Step 1: Create a Google Spreadsheet

1. Go to [Google Sheets](https://sheets.google.com/)

2. Click **"+ Blank"** to create a new spreadsheet

3. Name your spreadsheet:
   - Example: `GameConfigs`
   - Example: `ProductionConfigs`

4. Copy the spreadsheet ID from the URL:
   ```
   https://docs.google.com/spreadsheets/d/SPREADSHEET_ID_HERE/edit
   ```

   The ID is the long string between `/d/` and `/edit`

### Step 2: Structure Your Sheet

C4G expects a specific sheet structure:

#### Required Format

| FieldName | FieldType | Values1    | Values2    | Values3    |
|-----------|-----------|------------|------------|------------|
| id        | int       | 1          | 2          | 3          |
| name      | string    | Sword      | Shield     | Potion     |
| price     | float     | 100.5      | 200.0      | 50.75      |
| damage    | int       | 25         | 0          | 0          |

**Column Structure**:
- **Column A (FieldName)**: Names of your data fields
- **Column B (FieldType)**: C# type for each field
- **Columns C onwards**: Data values (each column is one item/row of data)

### Step 3: Supported Data Types

C4G supports the following types out of the box:

#### Primitive Types

| Type      | Example Values              | Description                    |
|-----------|-----------------------------|--------------------------------|
| `int`     | `1`, `42`, `-5`            | Integer numbers                |
| `float`   | `1.5`, `3.14`, `-2.7`      | Floating-point numbers         |
| `double`  | `1.5`, `3.14159265`        | Double-precision numbers       |
| `bool`    | `true`, `false`            | Boolean values                 |
| `string`  | `Hello`, `Item Name`       | Text strings                   |

#### Lists

To create a list field, append `[]` to the type:

| FieldName  | FieldType    | Values1      | Values2        |
|------------|--------------|--------------|----------------|
| tags       | string[]     | fire,sword   | defense,metal  |
| stats      | int[]        | 10,20,30     | 5,15,25        |

Lists use comma-separated values.

#### Enums

First, define your enum in C#:

```csharp
public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
```

Then use it in your sheet:

| FieldName | FieldType    | Values1   | Values2  |
|-----------|--------------|-----------|----------|
| rarity    | ItemRarity   | Common    | Epic     |

**Important**: Register custom enums in C4G Settings (covered in Editor Workflow).

## Example: Complete Item Config

### Google Sheet Structure

**Sheet Name**: `Items`

| FieldName     | FieldType      | Values1          | Values2          | Values3         |
|---------------|----------------|------------------|------------------|-----------------|
| id            | int            | 1                | 2                | 3               |
| name          | string         | Iron Sword       | Oak Shield       | Health Potion   |
| description   | string         | A sturdy blade   | Wooden defense   | Restores HP     |
| price         | float          | 150.0            | 100.0            | 25.5            |
| damage        | int            | 25               | 0                | 0               |
| defense       | int            | 0                | 15               | 0               |
| isConsumable  | bool           | false            | false            | true            |
| tags          | string[]       | weapon,melee     | armor,shield     | consumable,heal |

### Generated C# Class

C4G will generate:

```csharp
[System.Serializable]
public class Items
{
    public int id;
    public string name;
    public string description;
    public float price;
    public int damage;
    public int defense;
    public bool isConsumable;
    public string[] tags;
}
```

### Generated JSON

```json
{
  "items": [
    {
      "id": 1,
      "name": "Iron Sword",
      "description": "A sturdy blade",
      "price": 150.0,
      "damage": 25,
      "defense": 0,
      "isConsumable": false,
      "tags": ["weapon", "melee"]
    },
    {
      "id": 2,
      "name": "Oak Shield",
      "description": "Wooden defense",
      "price": 100.0,
      "damage": 0,
      "defense": 15,
      "isConsumable": false,
      "tags": ["armor", "shield"]
    }
  ]
}
```

## Multiple Sheets in One Spreadsheet

You can create multiple config types in one spreadsheet:

**Sheet: Items**
| FieldName | FieldType | ... |
|-----------|-----------|-----|
| id        | int       | ... |
| name      | string    | ... |

**Sheet: Enemies**
| FieldName | FieldType | ... |
|-----------|-----------|-----|
| id        | int       | ... |
| name      | string    | ... |
| health    | int       | ... |

C4G will generate:
- `Items.cs` and `Items` JSON
- `Enemies.cs` and `Enemies` JSON
- `RootConfig.cs` (contains both configs)

## Best Practices

### Naming Conventions

✅ **DO**:
- Use PascalCase for sheet names: `Items`, `EnemyStats`
- Use camelCase for field names: `id`, `itemName`, `maxHealth`
- Use descriptive names: `attackDamage` not `dmg`

❌ **DON'T**:
- Avoid spaces in sheet names: `Enemy Stats` → `EnemyStats`
- Avoid special characters: `item-name` → `itemName`
- Avoid reserved C# keywords: `class`, `int`, `string` as field names

### Data Organization

✅ **DO**:
- Keep related data in one sheet
- Use consistent column ordering
- Add a unique `id` field to each config
- Group similar items together

❌ **DON'T**:
- Mix unrelated data types in one sheet
- Leave empty rows between data
- Use merged cells
- Add comments or notes in data cells

### Performance Tips

- **Limit columns**: Google Sheets API is fastest with < 100 columns per sheet
- **Cache generated files**: Commit generated JSON to version control for runtime use
- **Separate sheets**: Use multiple sheets for different config types
- **Avoid complex formulas**: C4G reads raw values, not formula results

## Sheet Permissions

### For Personal Projects

1. Keep the sheet **private** (not shared)
2. Access via your personal Google account OAuth

### For Team Projects

1. Share the spreadsheet with team members:
   - Click **"Share"** in Google Sheets
   - Add team members with **"Editor"** access

2. Each team member needs their own OAuth credentials

3. Consider using **Google Workspace** for better team management

## Common Sheet Structures

### RPG Items

| FieldName    | FieldType  | Description               |
|--------------|------------|---------------------------|
| id           | int        | Unique identifier         |
| name         | string     | Display name              |
| description  | string     | Tooltip text              |
| iconId       | string     | Icon sprite reference     |
| rarity       | ItemRarity | Enum: Common, Rare, etc.  |
| stackSize    | int        | Max stack in inventory    |
| sellPrice    | int        | Vendor sell value         |

### Character Stats

| FieldName       | FieldType | Description                |
|-----------------|-----------|----------------------------|
| characterId     | int       | Unique ID                  |
| displayName     | string    | Character name             |
| baseHealth      | int       | Starting HP                |
| baseAttack      | int       | Starting attack power      |
| baseDefense     | int       | Starting defense           |
| movementSpeed   | float     | Movement units per second  |
| specialAbilities| string[]  | List of ability IDs        |

### Level Design

| FieldName       | FieldType | Description                 |
|-----------------|-----------|----------------------------|
| levelId         | int       | Unique identifier          |
| sceneName       | string    | Unity scene to load        |
| displayName     | string    | Level name in UI           |
| difficulty      | int       | 1-10 difficulty rating     |
| requiredLevel   | int       | Player level requirement   |
| enemySpawns     | string[]  | Enemy IDs to spawn         |
| rewardCoins     | int       | Completion reward          |

## Troubleshooting

### "Sheet not found" error

**Cause**: Sheet name doesn't match what's configured in C4G

**Solution**:
- Check exact sheet name (case-sensitive)
- Remove any spaces or special characters
- Verify sheet exists in the spreadsheet

### "Invalid spreadsheet ID"

**Cause**: Wrong ID copied from URL

**Solution**:
- Copy the ID between `/d/` and `/edit` in the URL
- Ensure no extra spaces or characters
- Test by opening the URL in a browser

### "Type not supported"

**Cause**: Using a type C4G doesn't recognize

**Solution**:
- Check the list of supported types above
- Register custom enums in C4G Settings
- Create a custom type parser (advanced)

### "Data not parsing correctly"

**Cause**: Data format doesn't match expected type

**Solution**:
- Verify data format (e.g., `true`/`false` for bool, not `yes`/`no`)
- Check for extra spaces in cells
- Ensure lists use comma separators with no spaces

### Empty or null values

**Cause**: Missing data in sheet

**Solution**:
- C4G treats empty cells as default values:
  - `int`: 0
  - `float`/`double`: 0.0
  - `bool`: false
  - `string`: empty string ""
  - Lists: empty array []

## Next Steps

- [Editor Workflow](./editor-workflow) - Generate configs in Unity
- [API Reference](../api/introduction) - Learn about C4G's API
- [Custom Type Parsers](./overview) - Extend C4G with custom types
