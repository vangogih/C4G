---
sidebar_position: 2
---

# Установка

Это руководство описывает установку C4G для Unity и автономных .NET-проектов.

## Системные требования

### Unity-пакет

- **Версия Unity**: 2019.4 или выше
- **Целевая платформа**: .NET Standard 2.0 или .NET 4.x
- **Операционная система**: Windows, macOS или Linux
- **Поддерживаемые платформы**: Только редактор (загрузка конфигов во время выполнения работает на всех платформах)

### Автономный .NET-пакет

- **Версия .NET**: .NET Standard 2.0 и выше
- **Операционная система**: Кроссплатформенно (Windows, macOS, Linux)

## Установка Unity-пакета

### Способ 1: Unity Package Manager (рекомендуется)

1. Скачайте последний `.unitypackage` из [GitHub Releases](https://github.com/vangogih/C4G/releases)

2. В Unity откройте Package Manager:
   - `Window → Package Manager`

3. Импортируйте пользовательский пакет:
   - `Assets → Import Package → Custom Package`
   - Выберите скачанный файл `C4G-v*.unitypackage`
   - Нажмите `Import` для импорта всех файлов

### Способ 2: Ручная установка

1. Скачайте и распакуйте последний релиз с GitHub

2. Скопируйте папку `C4G` в ваш Unity-проект:
   ```
   YourProject/
   └── Assets/
       └── C4G/
           ├── Core/
           ├── Editor/
           ├── Plugins/
           └── package.json
   ```

3. Unity автоматически обнаружит и импортирует пакет

### Проверка установки

После установки убедитесь, что C4G установлен правильно:

1. Откройте Unity Editor
2. Проверьте строку меню на наличие `Window → C4G`
3. Вы должны увидеть два пункта меню:
   - `Window → C4G → Settings`
   - `Window → C4G → Generate`

Если эти пункты меню появились — установка прошла успешно!

## Установка .NET Standard-пакета

Для автономных .NET-проектов установите C4G через NuGet:

### Через .NET CLI

```bash
dotnet add package C4G
```

### Через Package Manager Console (Visual Studio)

```powershell
Install-Package C4G
```

### Через PackageReference

Добавьте в файл `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="C4G" Version="0.0.4" />
</ItemGroup>
```

## Зависимости

C4G включает следующие зависимости (добавляются автоматически):

### Google Sheets API

- `Google.Apis.dll`
- `Google.Apis.Core.dll`
- `Google.Apis.Auth.dll`
- `Google.Apis.Sheets.v4.dll`

### Сериализация JSON

- `Newtonsoft.Json.dll`

Для Unity-установки эти зависимости находятся в `Assets/C4G/Plugins/`.

## Настройка после установки

После установки C4G вам потребуется:

1. **Настроить Google Sheets API** — [Руководство по настройке OAuth 2.0](./guides/oauth-setup)
2. **Создать первую Google-таблицу** — [Руководство по настройке Google Sheets](./guides/google-sheets-setup)
3. **Настроить параметры C4G** — Откройте `Window → C4G → Settings` в Unity

## Обновление C4G

### Unity-пакет

1. Скачайте последнюю версию из [GitHub Releases](https://github.com/vangogih/C4G/releases)
2. Удалите существующую папку `Assets/C4G` (предварительно сделайте резервную копию сгенерированных конфигов!)
3. Импортируйте новый `.unitypackage`, следуя шагам установки выше
4. При необходимости перенастройте параметры C4G

### .NET-пакет

```bash
dotnet add package C4G --version [новая-версия]
```

Или обновите через Package Manager в Visual Studio.

## Устранение неполадок

### «Пункты меню C4G не появляются»

**Решение**:
- Перезапустите Unity Editor
- Проверьте консоль на наличие ошибок импорта
- Убедитесь, что версия Unity — 2019.4 или выше

### «Ошибки DLL Google APIs»

**Решение**:
- Убедитесь, что все DLL в `Assets/C4G/Plugins/` импортированы
- Проверьте Inspector каждой DLL — убедитесь, что они настроены для платформы Editor
- При необходимости переимпортируйте пакет

### «Пространство имён 'C4G' не найдено»

**Решение**:
- Убедитесь, что ваше определение сборки ссылается на C4G.Core
- Убедитесь, что файл C4G.Core.asmdef существует
- Перезапустите Unity для обновления ссылок на сборки

### «Конфликты версий с существующими пакетами»

**Решение**:
- C4G использует `Newtonsoft.Json.dll` — если у вас есть другая версия, возможно, потребуется удалить одну из них
- При конфликтах с Google APIs убедитесь, что вы не импортируете другую интеграцию с Google Sheets

## Удаление

### Unity-пакет

1. Закройте Unity Editor
2. Удалите папку `Assets/C4G` из проекта
3. Удалите сгенерированные файлы конфигов (опционально)
4. Перезапустите Unity

### .NET-пакет

```bash
dotnet remove package C4G
```

## Следующие шаги

- [Настройка OAuth 2.0](./guides/oauth-setup) — Настройка доступа к Google Sheets API
- [Настройка Google Sheets](./guides/google-sheets-setup) — Создайте свою первую таблицу конфигов
- [Рабочий процесс в редакторе](./guides/editor-workflow) — Как использовать C4G в Unity Editor
