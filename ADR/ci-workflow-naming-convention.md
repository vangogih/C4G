# Decision Record for CI Workflow File Naming Convention

## Status

proposed

## Context

The `.github/workflows/` directory contains two kinds of YAML files: orchestrator workflows that define triggers (`on: push`, `on: pull_request`, `on: workflow_dispatch`) and reusable workflows that only expose `on: workflow_call`. GitHub Actions requires all workflow files to live in the same flat directory (subfolders are not supported for reusable workflow references). Without a naming convention the two kinds are hard to tell apart at a glance.

## Decision

Every workflow YAML file in `.github/workflows/` must follow this naming scheme:

```
<prefix>-<name>.yaml
```

### Prefixes

| Prefix | Meaning | Allowed triggers |
|--------|---------|------------------|
| `flow` | Orchestrator workflow — the entry point that GitHub displays as a single run. Calls one or more `job-` workflows. | Any (`push`, `pull_request`, `workflow_dispatch`, etc.) |
| `job`  | Reusable workflow — contains the actual work. Must only be called from `flow-` files via `uses:`. | `workflow_call` (required). `workflow_dispatch` is allowed as an exception for standalone manual runs. No other triggers. |

### Separators

| Position | Character | Example |
|----------|-----------|---------|
| After the prefix (`flow` / `job`) | `-` (hyphen) | `flow-on_pull_request.yaml` |
| Within the name | `_` (underscore) | `job-dotnet_test_coverage.yaml` |

### Job and step names

Every job and every step must have a human-readable `name:` that describes **what it does**, not what tool it uses.

| Level | Rule | Good | Bad |
|---|---|---|---|
| Job (`name:` under `jobs.<id>`) | Required. Describe the purpose of the job. | `name: Run .NET tests with coverage` | _(missing)_ or `name: test-coverage` |
| Step (`name:` on each step) | Required. Describe the action being performed. | `name: Build Docusaurus site` | `name: Checkout` or _(missing)_ |

Guidelines:
- Start with a verb: "Run ...", "Build ...", "Deploy ...", "Upload ...", "Check ...", etc.
- Be specific enough to distinguish from other steps at a glance in the GitHub Actions UI.
- For steps that use dynamic values, include them: `name: Bump version to ${{ inputs.tag }}`.

### Files

| File | Type | Purpose |
|---|---|---|
| `flow-on_pull_request.yaml` | flow | PR checks: test coverage + website build |
| `flow-on_push_master.yaml` | flow | Push to master: test coverage + Unity tests + conditional website deploy |
| `flow-release.yaml` | flow | Manual release: version bump -> wait for tests -> create release |
| `job-dotnet_test_coverage.yaml` | job | .NET standalone tests with coverage; PR comment |
| `job-website_build.yaml` | job | Build Docusaurus site; optional artifact upload |
| `job-website_deploy.yaml` | job | Build + deploy website to GitHub Pages |
| `job-unity_tests.yaml` | job | Unity editmode tests across LTS matrix |
| `job-patch_versions.yaml` | job | Bump versions, commit, tag, push (also has `workflow_dispatch`) |
| `job-wait_for_action_status.yaml` | job | Poll GitHub Checks API until all checks pass |
| `job-create_release.yaml` | job | Create draft prerelease on GitHub |

## Consequences

- The file listing in `.github/workflows/` is self-documenting: `flow-` files are entry points, `job-` files are building blocks.
- All `uses:` references inside `flow-` files follow the pattern `./.github/workflows/job-<name>.yaml`.
- Adding a new CI capability means creating a `job-` file and wiring it into the appropriate `flow-` file(s).
- `job-patch_versions.yaml` retains `workflow_dispatch` as a documented exception so it can be run standalone for manual version bumps outside of a release.

## See also

- [CI Workflow Coordination](ci-workflow-coordination.md) — the orchestrator/reusable-job architecture and release flow
