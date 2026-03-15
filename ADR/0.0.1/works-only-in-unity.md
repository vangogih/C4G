# Decision record for Unity as the only entry point

## Status

accepted

## Related

[Unity 2021 LTS Upgrade](../unity-2021-lts-upgrade.md)

## Context

To release early and prove the concept of the tool, we must limit support of platforms, engines, and frameworks,
and also the user stories of how they could use the tool.
The tool must be simple and provide the required amount of functionality to solve the main user issues.

## Decision

Use only Unity as the entry point for the tool.

## Consequences

Much less code compared to a multi-framework approach (e.g., Unreal Engine integration).
Simplicity and convenience of the codebase.
Does not maximize user coverage.
