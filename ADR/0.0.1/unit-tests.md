# Decision record for unit tests

## Status

accepted

## Context

As stated in MANIFEST.md, we are to provide a production-ready solution.
We must also ensure there are no critical issues.

## Decision

Create and maintain automated tests with 95%+ code coverage.

## Consequences

Early issue detection.
Better code-level architecture.

## See also

- [Code Coverage Threshold in CI](../code-coverage-threshold.md) — automated enforcement of the 95% coverage target in GitHub workflows
- [CI Workflow Coordination](../ci-workflow-coordination.md) — automated CI pipelines for running standalone and Unity tests
- [.NET Scripts in CI Workflows](../0.0.4/dotnet-scripts-for-ci.md) — why CI scripts are written in C# instead of Bash, Python, or Cake
