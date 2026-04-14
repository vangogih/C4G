## BumpVersion.cs

```csharp
dotnet run --file .github/workflows/scripts/BumpVersion.cs -e GITHUB_WORKSPACE=/usr/absolute_path_to_c4g/ version
```
Requires: `dotnet SDK 10.0.100+`, see: [tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/file-based-programs)

Where:
- `/usr/absolute_path_to_c4g/` you must replace with absolute path to C4G root folder
- `version` - version you want to patch in semantic versioning format. Like: `0.0.1`, `1.2.3`

## CheckCoverageThreshold.cs

```bash
dotnet run --file .github/workflows/scripts/CheckCoverageThreshold.cs -- --summary-path "C4G.Unity/CodeCoverage/Summary.json" --threshold 97
```
Requires: `dotnet SDK 10.0.100+`, see: [tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/file-based-programs)

Where:
- `--summary-path` (required): path to [ReportGenerator](https://github.com/danielpalme/ReportGenerator) `Summary.json`
- `--threshold` (optional): minimum percent for `linecoverage`, `branchcoverage`, `methodcoverage`. Default: **95**