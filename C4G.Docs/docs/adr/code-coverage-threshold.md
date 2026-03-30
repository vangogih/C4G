# Decision Record for Code Coverage Threshold in CI

## Status

proposed

## Context

The [Unit Tests](0.0.1/unit-tests.md) ADR established a 95%+ code coverage target.
However, there was no automated enforcement — coverage was a goal, not a gate.
Commits that dropped coverage below the target could still be merged.

## Decision

GitHub workflow will fail every commit to the branch if coverage is less than the 95% threshold.
The check runs as part of `job-dotnet_test_coverage` and validates line, branch, and method coverage against the threshold.

## Consequences

No commit can be merged if it lowers coverage below 95%.
This makes the coverage target from the Unit Tests ADR enforceable rather than aspirational.

## See also

- [Unit Tests](0.0.1/unit-tests.md) — original decision establishing 95%+ coverage target
