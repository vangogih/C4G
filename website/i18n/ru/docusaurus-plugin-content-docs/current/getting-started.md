---
sidebar_position: 1
---

# Начало работы

Добро пожаловать в C4G! Это руководство поможет вам начать управлять конфигурациями вашей игры через Google Sheets.

## Что такое C4G?

C4G (Configs for Games) — это готовый к использованию в производстве набор инструментов, который позволяет управлять конфигурациями вашей Unity-игры через Google Sheets. Он автоматически генерирует C# DTO-классы и сериализует ваши конфигурации в JSON, делая управление конфигурацией простым и доступным для непрограммистов.

## Требования

Перед началом убедитесь, что у вас есть:

- Unity 2019.4 или новее
- Учетная запись Google для Google Sheets
- Базовое понимание Unity и C#

## Установка

### Установка Unity-пакета

1. Загрузите последний Unity-пакет C4G из [GitHub Releases](https://github.com/vangogih/C4G/releases)
2. Импортируйте пакет в ваш Unity-проект:
   - `Assets → Import Package → Custom Package`
   - Выберите загруженный файл `.unitypackage`
   - Нажмите `Import`

### Установка .NET Standard пакета

Для автономных .NET проектов установите через NuGet:

```bash
dotnet add package C4G
```

## Быстрый старт

### 1. Настройка Google Sheets API

1. Перейдите в [Google Cloud Console](https://console.cloud.google.com/)
2. Создайте новый проект или выберите существующий
3. Включите Google Sheets API
4. Создайте учетные данные (OAuth 2.0 Client ID)
5. Загрузите JSON-файл с учетными данными

### 2. Настройка C4G в Unity

1. Откройте окно настроек C4G:
   - `Window → C4G → Settings`
2. Настройте конфигурацию:
   - **Google Sheets ID**: ID вашей таблицы из URL
   - **Client Secret**: Путь к JSON-файлу с учетными данными
   - **Generated Code Folder**: Куда будут генерироваться C#-классы
   - **Serialized Configs Folder**: Куда будут сохраняться JSON-файлы

### 3. Создайте свою первую таблицу конфигурации

В вашей Google-таблице создайте лист со следующей структурой:

| FieldName | FieldType | Values     |
|-----------|-----------|------------|
| id        | int       | 1, 2, 3    |
| name      | string    | Item1, ... |
| price     | float     | 10.5, ...  |

### 4. Генерация кода и конфигураций

1. Откройте окно C4G: `Window → C4G → Generate`
2. Нажмите `Generate Configs`
3. C4G выполнит:
   - Получение данных из Google Sheets
   - Генерацию C# DTO-классов
   - Сериализацию данных в JSON

### 5. Использование в вашей игре

```csharp
using C4G.Runtime;
using UnityEngine;

public class ConfigExample : MonoBehaviour
{
    void Start()
    {
        // Загрузка сгенерированной конфигурации
        var config = Resources.Load<TextAsset>("Configs/YourConfig");
        var data = JsonUtility.FromJson<YourConfig>(config.text);

        Debug.Log($"Загружено {data.items.Length} элементов");
    }
}
```

## Следующие шаги

- [Посмотрите справочник API](./api/introduction)
- [Прочитайте руководства](./guides/overview)
- [Получить помощь](./help)

## Получение помощи

- [GitHub Issues](https://github.com/vangogih/C4G/issues)
- [GitHub Discussions](https://github.com/vangogih/C4G/discussions)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/c4g)
