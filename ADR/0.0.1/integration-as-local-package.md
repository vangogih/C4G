# Decision record for tool integration as local package

## Status

accepted

## Context

Currently we are at an early stage of development.
We don't yet need to provide the tool to users in all convenient ways.

## Decision

Don't set up complicated CI/CD pipelines, npm servers, etc.
Just provide the correct package setup in the tool folder to support tool integration as a local package.

## Consequences

Users can easily integrate the tool into the project.

## See also

- [CI Workflow Coordination](../ci-workflow-coordination.md) — CI/CD pipelines that were eventually introduced for test and release automation
- [.NET Scripts in CI Workflows](../0.0.4/dotnet-scripts-for-ci.md) — why CI scripts are written in C# instead of Bash, Python, or Cake
