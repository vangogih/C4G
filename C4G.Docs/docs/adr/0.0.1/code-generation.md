# Decision record for code generation

## Status

accepted

## Context

Configs schema changes relatively often and must have its DTO representation in the project.
Though developers have to often create or change DTO classes

## Decision

Generate DTO classes on c# lang according to specifically formatted input data

## Consequences

No need for manual DTO creation and editing