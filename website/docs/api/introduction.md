---
sidebar_position: 1
---

# API Reference

Welcome to the C4G API reference documentation. This section provides detailed information about C4G's public API.

## Core Components

### C4GFacade

The main entry point for C4G operations. Orchestrates the entire workflow from fetching Google Sheets data to generating code and serializing configurations.

```csharp
public sealed class C4GFacade
{
    public C4GFacade(IC4GSettingsProvider settingsProvider);
    public async Task<Result<string>> RunAsync(CancellationToken ct);
}
```

**Usage:**

```csharp
var settingsProvider = new YourSettingsProvider();
var facade = new C4GFacade(settingsProvider);
var result = await facade.RunAsync(cancellationToken);

if (result.IsOk)
{
    Debug.Log("Config generation successful!");
}
else
{
    Debug.LogError($"Error: {result.Error}");
}
```

## Type Parsers

### IC4GTypeParser Interface

Interface for implementing custom type parsers.

```csharp
public interface IC4GTypeParser
{
    string TypeName { get; }
    object Parse(string value);
}
```

### Built-in Parsers

- **IntParser** - Parses integer values
- **FloatParser** - Parses float values
- **DoubleParser** - Parses double values
- **BoolParser** - Parses boolean values
- **StringParser** - Parses string values
- **EnumParser** - Parses enum values

## Settings

### C4GSettings

Configuration container for C4G operations.

```csharp
public class C4GSettings
{
    public string TableId { get; set; }
    public string ClientSecret { get; set; }
    public string RootConfigName { get; set; }
    public string GeneratedCodeFolderFullPath { get; set; }
    public string SerializedConfigsFolderFullPath { get; set; }
    public Dictionary<string, SheetParserBase> SheetParsersByName { get; set; }
    public Dictionary<string, IC4GTypeParser> AliasParsersByName { get; set; }
}
```

## Sheet Parsing

### SheetParserBase

Base class for sheet parsers.

```csharp
public abstract class SheetParserBase
{
    public abstract ParsedSheet Parse(string sheetName, IList<IList<object>> sheet);
}
```

## Result Types

### Result\<T>

Generic result type for operations that can succeed or fail.

```csharp
public class Result<T>
{
    public bool IsOk { get; }
    public T Value { get; }
    public string Error { get; }

    public static Result<T> FromValue(T value);
    public static Result<T> FromError(string error);
}
```

## Next Steps

- [Getting Started Guide](../getting-started)
- [Explore Guides](../guides/overview)
- [Get Help](../help)
