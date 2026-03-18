---
sidebar_position: 1
---

# Getting Started

Welcome to C4G! This guide will help you get started with managing your game configurations through Google Sheets.

## What is C4G?

C4G (Configs for Games) is a production-ready toolset that enables you to manage your Unity game configurations through Google Sheets. It automatically generates C# DTO classes and serializes your configurations to JSON, making configuration management simple and accessible to non-programmers.

## Prerequisites

Before you begin, ensure you have:

- Unity 2019.4 or later
- A Google account for Google Sheets
- Basic understanding of Unity and C#

For detailed requirements, see [Supported Versions](./guides/supported-versions).

## Installation

For complete installation instructions, see the [Installation Guide](./installation).

**Quick Install**:

1. Download the latest `.unitypackage` from [GitHub Releases](https://github.com/vangogih/C4G/releases)
2. In Unity: `Assets → Import Package → Custom Package`
3. Select the downloaded file and click `Import`

## Quick Start

### 1. Set Up Google Sheets API

Configure OAuth 2.0 credentials to access your Google Sheets:

1. Create a Google Cloud project
2. Enable the Google Sheets API
3. Create OAuth 2.0 credentials
4. Download credentials JSON file

**Full guide**: [OAuth 2.0 Setup](./guides/oauth-setup)

### 2. Configure C4G in Unity

1. Open `Window → C4G → Settings`
2. Enter your Google Sheets ID
3. Set path to credentials JSON file
4. Configure output folders for generated code and configs

**Full guide**: [Editor Workflow](./guides/editor-workflow)

### 3. Create Your First Config Sheet

Structure your Google Sheet with this format:

| FieldName | FieldType | Values1   | Values2   | Values3   |
|-----------|-----------|-----------|-----------|-----------|
| id        | int       | 1         | 2         | 3         |
| name      | string    | Sword     | Shield    | Potion    |
| price     | float     | 100.5     | 200.0     | 50.75     |

**Full guide**: [Google Sheets Setup](./guides/google-sheets-setup)

### 4. Generate Code and Configs

1. Open `Window → C4G → Generate`
2. Click `Generate Configs`
3. C4G automatically:
   - Fetches data from Google Sheets
   - Generates C# DTO classes
   - Serializes data to JSON

### 5. Use in Your Game

```csharp
using C4G.Runtime;
using UnityEngine;

public class ConfigExample : MonoBehaviour
{
    void Start()
    {
        // Load your generated config
        var config = Resources.Load<TextAsset>("Configs/YourConfig");
        var data = JsonUtility.FromJson<YourConfig>(config.text);

        Debug.Log($"Loaded {data.items.Length} items");
    }
}
```

## Next Steps

Now that you've completed the quick start, explore these topics:

- **[Installation Guide](./installation)** - Detailed installation and setup
- **[OAuth 2.0 Setup](./guides/oauth-setup)** - Complete OAuth configuration
- **[Google Sheets Setup](./guides/google-sheets-setup)** - Learn sheet structure and best practices
- **[Editor Workflow](./guides/editor-workflow)** - Master C4G in Unity Editor
- **[Supported Versions](./guides/supported-versions)** - Check compatibility
- **[API Reference](./api/introduction)** - Understand C4G's API

## Getting Help

- [GitHub Issues](https://github.com/vangogih/C4G/issues)
- [GitHub Discussions](https://github.com/vangogih/C4G/discussions)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/c4g)
