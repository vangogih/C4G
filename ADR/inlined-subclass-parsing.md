# Decision Record for Inlined Subclass Parsing

## Status

accepted

## Context

Some game configs contain nested data structures where a property belongs to a typed sub-object (e.g., a reward with an amount and a type).
Previously, such structures required separate sheets or manual post-processing.
Developers needed a way to express sub-objects inline within a single sheet row using a naming convention.

## Decision

Support inlined subclass declaration via dot notation in column headers: `SubTypeName.PropertyName`.

![Inlined subclass example](inlined-subclass-parsing-example.png)

During sheet parsing, any property whose name contains a dot is split into a subtype name and a property name.
Subtypes are collected per config and referenced by index, so the generated code can reconstruct the nested object without extra sheets or runtime lookups.

## Consequences

A single sheet row can now represent an object with one or more typed sub-objects without introducing additional sheets.
Column naming becomes a contract: a dot in a header is always interpreted as a subtype separator.
Configs that happen to use dots in column names for other purposes will be misinterpreted and must be renamed.
