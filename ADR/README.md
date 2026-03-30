# C4G Architecture Decision Records (ADR)

Architecture Decision Records (later ADR) are living documents describing important architectural decisions made in the project.
They capture the context, alternatives considered, and consequences of each decision to provide historical context for future development.

## Structure and Lifecycle

ADRs can have the status *proposed*, *accepted*, *rejected*, or *inactive*.

*proposed* ADRs are stored directly in the ADR folder as individual markdown files.

*accepted* ADRs are archived in folders corresponding to the project version when they were accepted.

*rejected* and *superseded* ADRs are stored in separate subfolders corresponding to its status names.

This flat structure for proposed ADRs makes them easier to browse and manage, while version-based archiving provides clear historical organization.
Each ADR should contain the context of the problem, alternatives considered, the decision made, and its consequences to help new team members quickly understand the reasoning behind architectural choices.

## Index

✅ accepted · 📝 proposed · ❌ rejected · ⚪ inactive

### Organizational

| Status | ADR                                                                               | Version |
|--------|-----------------------------------------------------------------------------------|---------|
| ✅     | [Why Use ADR](0.0.1/why-use-adr.md)                                               | 0.0.1   |
| ✅     | [GitHub as Workflow Management Tool](0.0.1/github-as-workflow-management-tool.md) | 0.0.1   |
| ✅     | [Unit Tests](0.0.1/unit-tests.md)                                                 | 0.0.1   |
| ✅     | [Simpler ADR Structure](0.0.3/simpler-adr-structure.md)                           | 0.0.3   |

### Project

| Status | ADR                                                                                                   | Version |
|--------|-------------------------------------------------------------------------------------------------------|---------|
| ✅     | [Code Generation](0.0.1/code-generation.md)                                                           | 0.0.1   |
| ✅     | [Google OAuth Secret Key in ScriptableObject](0.0.1/google-oauth-secret-key-in-scriptable-object.md) | 0.0.1   |
| ✅     | [Google Sheets as Only Data Source](0.0.1/google-sheets-as-only-data-source.md)                       | 0.0.1   |
| ✅     | [Google Sheets Integration Method](0.0.1/google-sheets-integration-method.md)                         | 0.0.1   |
| ✅     | [Integration as Local Package](0.0.1/integration-as-local-package.md)                                 | 0.0.1   |
| ✅     | [JSON Generation](0.0.1/json-generation.md)                                                           | 0.0.1   |
| ✅     | [Unity as the Only Entry Point](0.0.1/works-only-in-unity.md)                                         | 0.0.1   |
| ✅     | [Modular C4G Design](0.0.3/modular-design.md)                                                         | 0.0.3   |
| 📝     | [Unity 2021 LTS Upgrade](unity-2021-lts-upgrade.md)                                                   |         |

### Release

| Status | ADR                                                                   | Version |
|--------|-----------------------------------------------------------------------|---------|
| ✅     | [.NET Scripts for CI Workflows](0.0.4/dotnet-scripts-for-ci.md)       | 0.0.4  |
| 📝     | [Code Coverage Threshold in CI](code-coverage-threshold.md)           |        |
| 📝     | [CI Workflow Coordination](ci-workflow-coordination.md)                |        |
| 📝     | [CI Workflow Naming Convention](ci-workflow-naming-convention.md)      |        |
