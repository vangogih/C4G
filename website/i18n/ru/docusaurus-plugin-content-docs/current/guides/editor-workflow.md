---
sidebar_position: 4
---

# Рабочий процесс в редакторе

Узнайте, как использовать C4G в Unity Editor для генерации кода конфигурации и данных из Google Sheets.

## Обзор

Рабочий процесс C4G в редакторе включает:
1. Настройку параметров (одноразово)
2. Генерацию C#-классов и JSON-конфигов
3. Использование сгенерированных конфигов в коде игры

## Открытие окон C4G

C4G предоставляет два основных окна редактора:

### Окно настроек C4G

**Меню**: `Window → C4G → Settings`

Используйте это окно для настройки:
- Подключения к Google Sheets
- Папок вывода
- Парсеров пользовательских типов
- Определений листов

### Окно генерации C4G

**Меню**: `Window → C4G → Generate`

Используйте это окно для:
- Генерации конфигов из Google Sheets
- Отслеживания прогресса генерации
- Просмотра ошибок и предупреждений

## Первоначальная настройка

### Шаг 1: Базовые настройки

1. Откройте `Window → C4G → Settings`

2. Настройте следующие обязательные поля:

#### Google Sheets ID

- **Метка**: «Table ID» или «Spreadsheet ID»
- **Значение**: ID вашей таблицы из URL Google Sheets
- **Пример**: `1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms`

#### Client Secret

- **Метка**: «Client Secret»
- **Значение**: Путь к JSON-файлу с OAuth-учётными данными
- **Пример**: `C:/Projects/MyGame/Credentials/c4g_credentials.json`
- **Примечание**: Используйте абсолютные пути

#### Root Config Name

- **Метка**: «Root Config Name»
- **Значение**: Имя главного контейнерного класса конфигов
- **Пример**: `GameConfig`
- **По умолчанию**: `RootConfig`

#### Generated Code Folder

- **Метка**: «Generated Code Folder»
- **Значение**: Папка для генерации C#-классов
- **Пример**: `Assets/Scripts/Generated/`
- **Рекомендация**: Храните сгенерированный код в отдельной папке

#### Serialized Configs Folder

- **Метка**: «Serialized Configs Folder»
- **Значение**: Папка для сохранения JSON-файлов
- **Пример**: `Assets/Resources/Configs/`
- **Рекомендация**: Используйте `Resources/` для загрузки во время выполнения

### Шаг 2: Определение парсеров листов

Парсеры листов сообщают C4G, какие листы читать из вашей таблицы.

1. В окне настроек найдите раздел **«Sheet Parsers»**

2. Нажмите **«+»** для добавления нового листа

3. Настройте каждый лист:
   - **Sheet Name**: Должно точно совпадать с именем в Google Sheets (с учётом регистра)
   - **Parser Type**: Обычно «Default» (продвинутые пользователи могут создавать собственные парсеры)

4. Пример конфигурации:
   ```
   Sheet Name: Items
   Parser Type: Default Sheet Parser

   Sheet Name: Enemies
   Parser Type: Default Sheet Parser

   Sheet Name: LevelConfigs
   Parser Type: Default Sheet Parser
   ```

### Шаг 3: Настройка парсеров пользовательских типов (опционально)

Если вы используете перечисления или пользовательские типы:

1. Найдите раздел **«Alias Parsers»**

2. Нажмите **«+»** для добавления пользовательского типа

3. Настройте:
   - **Alias Name**: Имя типа в Google Sheets
   - **Parser Type**: Выберите подходящий парсер (Enum, Custom и т.д.)

4. Пример для перечислений:
   ```
   Alias Name: ItemRarity
   Parser Type: Enum Parser
   Enum Type: MyGame.ItemRarity
   ```

## Генерация конфигов

### Базовая генерация

1. Откройте `Window → C4G → Generate`

2. Нажмите кнопку **«Generate Configs»**

3. C4G выполнит:
   - Подключение к Google Sheets (при первом запуске может открыть браузер для OAuth)
   - Получение данных листов
   - Парсинг данных
   - Генерацию C#-классов
   - Сериализацию JSON-файлов
   - Отображение результатов в консоли

4. Дождитесь сообщения об успехе:
   ```
   C4G: Config generation successful!
   ```

### Первичная авторизация

При первой генерации потребуется авторизация:

1. Браузер по умолчанию откроется автоматически

2. Войдите в Google (если ещё не вошли)

3. Ознакомьтесь с разрешениями и примите их

4. Вернитесь в Unity — генерация продолжится автоматически

5. Авторизация сохраняется, повторять её не нужно

### Прогресс генерации

Во время генерации следите за консолью:

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

### Проверка сгенерированных файлов

После успешной генерации проверьте вывод:

1. **C#-классы** (в папке Generated Code Folder):
   ```
   Assets/Scripts/Generated/
   ├── Items.cs
   ├── Enemies.cs
   └── GameConfig.cs
   ```

2. **JSON-конфиги** (в папке Serialized Configs Folder):
   ```
   Assets/Resources/Configs/
   └── GameConfig.json
   ```

3. **Проверьте консоль Unity** на наличие предупреждений или ошибок

## Использование сгенерированных конфигов в коде

### Загрузка конфигов во время выполнения

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
        // Загрузка из папки Resources
        TextAsset configFile = Resources.Load<TextAsset>("Configs/GameConfig");

        if (configFile != null)
        {
            gameConfig = JsonConvert.DeserializeObject<GameConfig>(configFile.text);
            Debug.Log($"Загружено {gameConfig.items.Length} предметов");
        }
        else
        {
            Debug.LogError("Файл конфигурации не найден!");
        }
    }
}
```

### Доступ к данным конфига

```csharp
// Доступ к предметам
foreach (var item in gameConfig.items)
{
    Debug.Log($"Предмет: {item.name}, Цена: {item.price}");
}

// Поиск предмета по ID
var sword = System.Array.Find(gameConfig.items, i => i.id == 1);
if (sword != null)
{
    Debug.Log($"Найдено: {sword.name}");
}

// Доступ к врагам
foreach (var enemy in gameConfig.enemies)
{
    Debug.Log($"Враг: {enemy.name}, HP: {enemy.health}");
}
```

### Создание синглтона конфига

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

// Использование в других скриптах:
var itemConfig = ConfigManager.Instance.Config.items[0];
```

## Рабочий процесс разработки

### Рекомендуемый порядок работы

1. **Проектируйте конфиги** в Google Sheets
2. **Тестируйте данные** с несколькими примерами
3. **Генерируйте конфиги** в Unity (`Window → C4G → Generate`)
4. **Проверяйте, компилируется ли** сгенерированный код без ошибок
5. **Тестируйте в режиме Play** с тестовыми данными
6. **Добавляйте полный датасет** в Google Sheets
7. **Перегенерируйте конфиги**
8. **Добавляйте сгенерированный JSON** в систему контроля версий
9. **Опционально добавляйте C#-классы** (на усмотрение команды)

### Когда перегенерировать

Перегенерируйте конфиги при:
- ✅ Добавлении новых полей в лист
- ✅ Добавлении новых листов
- ✅ Изменении значений данных
- ✅ Изменении типов полей
- ✅ Добавлении/удалении строк данных

Перегенерация не нужна при:
- ❌ Изменениях только в коде
- ❌ Изменениях сцен Unity
- ❌ Правках UI

### Контроль версий

**Рекомендуемый подход**:

1. **Добавляйте JSON-файлы в систему контроля версий**:
   - Это ваши конфиги для выполнения
   - Члены команды нуждаются в них для запуска игры
   - Небольшой размер файлов

2. **Добавляйте C#-классы в `.gitignore`** (опционально):
   - Их можно перегенерировать из таблиц
   - Уменьшает количество конфликтов слияния
   - Каждый разработчик генерирует локально

**Альтернативный подход**:

1. **Добавляйте в систему контроля версий и JSON, и C#-файлы**:
   - Полезно, если не все члены команды имеют доступ OAuth
   - Предоставляет полный снимок конфигов
   - Возможны конфликты слияния

## Горячие клавиши

В C4G нет встроенных горячих клавиш, но вы можете создать свои:

1. Создайте скрипт в `Assets/Editor/`:

```csharp
using UnityEditor;
using C4G.Editor;

public class C4GShortcuts
{
    [MenuItem("Tools/C4G/Generate Configs _g")]  // Alt+G
    static void GenerateConfigs()
    {
        // Открыть окно генерации C4G
        EditorWindow.GetWindow<C4GWindow>();
    }

    [MenuItem("Tools/C4G/Open Settings _s")]  // Alt+S
    static void OpenSettings()
    {
        EditorWindow.GetWindow<C4GSettingsProvider>();
    }
}
```

## Устранение неполадок

### Ошибки «Generation failed»

**Проверьте**:
1. Консоль на наличие конкретных сообщений об ошибках
2. Подключение к интернету
3. Разрешения Google Sheets
4. Действительность OAuth-учётных данных

### Сгенерированные классы содержат ошибки компиляции

**Частые причины**:
- Зарезервированные ключевые слова C# используются как имена полей
- Недопустимые имена типов
- Циклические зависимости

**Решения**:
- Переименуйте поля в Google Sheets
- Используйте допустимые имена типов C#
- Проверьте правописание имён полей

### JSON-файл не найден во время выполнения

**Причины**:
- Файл не в папке `Resources/`
- Неверный путь к файлу в коде
- Файл не добавлен в систему контроля версий

**Решения**:
- Убедитесь, что файл находится в `Assets/Resources/Configs/`
- Проверьте соответствие имени файла и кода
- Перегенерируйте конфиги

### OAuth-авторизация запрашивается повторно

**Причины**:
- Токен истёк
- Файл токена удалён
- Учётные данные изменились

**Решения**:
- Повторно авторизуйтесь (один раз)
- Убедитесь, что файл учётных данных не переместился
- Проверьте действительность учётных данных в Google Cloud Console

## Продвинутые возможности

### Пакетная обработка

Для обновления нескольких конфигов:

1. Обновите все листы в Google Sheets
2. Запустите генерацию один раз — C4G обработает все листы вместе
3. Просмотрите все изменения в одном коммите

### Валидация конфигов

Добавьте валидацию после загрузки:

```csharp
void ValidateConfigs()
{
    foreach (var item in gameConfig.items)
    {
        if (item.price < 0)
        {
            Debug.LogWarning($"У предмета {item.name} отрицательная цена!");
        }
    }
}
```

### Горячая перезагрузка (продвинутый уровень)

Для быстрой итерации во время разработки:

```csharp
#if UNITY_EDITOR
[UnityEditor.MenuItem("Tools/Reload Configs")]
static void ReloadConfigs()
{
    var manager = FindObjectOfType<ConfigManager>();
    manager.LoadConfigs();
    Debug.Log("Конфиги перезагружены!");
}
#endif
```

## Следующие шаги

- [Поддерживаемые версии](./supported-versions) — Проверьте совместимость
- [Справочник API](../api/introduction) — Подробнее об API C4G
- [Парсеры пользовательских типов](./overview) — Расширение функциональности C4G
