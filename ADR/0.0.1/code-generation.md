# Decision record for code generation

## Status

accepted

## Context

The config schema changes relatively often and must have a DTO representation in the project.
Thus, developers often have to create or modify DTO classes.

## Decision

Generate DTO classes in C# according to specifically formatted input data.

## Consequences

No need for manual DTO creation and editing.
