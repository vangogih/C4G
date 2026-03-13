# Decision Record for .NET Scripts in CI Workflows

## Status

accepted

## Context

CI workflows often require custom scripting for tasks like version bumping, asset processing, or build orchestration.
Common choices include Bash/Shell, Python, Cake, PowerShell, or custom CLI tools.
The project is built entirely on .NET (C# Unity package + standalone .NET library), and the team already maintains C# codebases.

## Decision

Use .NET file-based programs (C# scripts via `dotnet run --file`) for all custom CI workflow logic instead of Bash, Python, Cake, or other scripting tools.

## Consequences

Single language across the entire project — production code, tests, and CI scripts are all C#.
Lower onboarding friction — contributors only need .NET knowledge.
Full access to .NET ecosystem (XML, JSON, file I/O, etc.) without third-party dependencies.
Requires .NET SDK 10.0+ on CI runners for file-based program support.
