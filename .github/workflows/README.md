# CI Workflows

> **Before adding or modifying workflow files, read the [CI Workflow Naming Convention](../../ADR/ci-workflow-naming-convention.md) ADR.**

## Rules

1. **File prefixes** -- `flow-` for orchestrator workflows (entry points), `job-` for reusable workflows (building blocks). No other prefixes allowed.
2. **Separators** -- hyphen `-` after the prefix, underscores `_` within the name. Example: `flow-on_pull_request.yaml`, `job-dotnet_test_coverage.yaml`.
3. **Triggers** -- `flow-` files may use any trigger. `job-` files must use `workflow_call`; `workflow_dispatch` is the only allowed exception.
4. **Job and step names** -- every job and every step must have a human-readable `name:` starting with a verb that describes what it does.
5. **Flat directory** -- GitHub Actions does not support reusable workflows in subfolders. All `.yaml` files must live directly in this directory.

## Related ADRs

- [CI Workflow Naming Convention](../../ADR/ci-workflow-naming-convention.md) -- file naming, prefixes, separators, job/step naming
- [CI Workflow Coordination](../../ADR/ci-workflow-coordination.md) -- orchestrator/reusable-job architecture, release flow
