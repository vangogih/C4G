---
sidebar_position: 1
---

# Справочник API

Добро пожаловать в справочную документацию API C4G. Этот раздел предоставляет подробную информацию о публичном API C4G.

## Основные компоненты

### C4GFacade

Главная точка входа для операций C4G. Управляет всем рабочим процессом от получения данных Google Sheets до генерации кода и сериализации конфигураций.

```csharp
public sealed class C4GFacade
{
    public C4GFacade(IC4GSettingsProvider settingsProvider);
    public async Task<Result<string>> RunAsync(CancellationToken ct);
}
```

**Использование:**

```csharp
var settingsProvider = new YourSettingsProvider();
var facade = new C4GFacade(settingsProvider);
var result = await facade.RunAsync(cancellationToken);

if (result.IsOk)
{
    Debug.Log("Генерация конфигурации успешна!");
}
else
{
    Debug.LogError($"Ошибка: {result.Error}");
}
```

## Парсеры типов

### Интерфейс IC4GTypeParser

Интерфейс для реализации пользовательских парсеров типов.

```csharp
public interface IC4GTypeParser
{
    string TypeName { get; }
    object Parse(string value);
}
```

### Встроенные парсеры

- **IntParser** - Парсит целочисленные значения
- **FloatParser** - Парсит значения float
- **DoubleParser** - Парсит значения double
- **BoolParser** - Парсит булевы значения
- **StringParser** - Парсит строковые значения
- **EnumParser** - Парсит значения enum

## Настройки

### C4GSettings

Контейнер конфигурации для операций C4G.

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

## Парсинг таблиц

### SheetParserBase

Базовый класс для парсеров таблиц.

```csharp
public abstract class SheetParserBase
{
    public abstract ParsedSheet Parse(string sheetName, IList<IList<object>> sheet);
}
```

## Типы результатов

### Result\<T>

Общий тип результата для операций, которые могут завершиться успехом или неудачей.

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

## Следующие шаги

- [Руководство по началу работы](../getting-started)
- [Изучить руководства](../guides/overview)
- [Получить помощь](../help)
