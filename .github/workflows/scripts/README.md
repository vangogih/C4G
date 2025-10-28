## BumpVersion.cs

```csharp
dotnet run --file ./scripts/BumpVersion.cs -e GITHUB_WORKSPACE=/usr/absolute_path_to_c4g/ version
```
Where:
- `/usr/absolute_path_to_c4g/` you must replace with absolute path to C4G root folder
- `version` - version you want to patch in sematic versioning format. Like: `0.0.1`, `1.2.3`