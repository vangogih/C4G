# Decision record for tool integration as local package

## Status

accepted

## Context

Currently we are on early stage of development
We still dont have to provide tool to the users in all convenient ways

## Decision

Dont setup complicated CI/CD pipelines, npm servers etc
Just provide correct package setup in tool folder to support tool integration as local package

## Consequences

Users could easilly integrate tool to the project

## See also

- [CI Workflow Coordination](../ci-workflow-coordination.md) — CI/CD pipelines that were eventually introduced for test and release automation
- [.NET Scripts in CI Workflows](../0.0.4/dotnet-scripts-for-ci.md) — why CI scripts are written in C# instead of Bash, Python, or Cake
