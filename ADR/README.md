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
