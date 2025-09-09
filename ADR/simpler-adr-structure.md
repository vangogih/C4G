# Decision Record for Simpler ADR Structure

## Status

Proposed

## Context

The previously suggested scheme for structuring ADRs is cumbersome and non-intuitive.
A folder must be created for every single ADR, named as a short label for the ADR.
Inside each folder, a README.md file must be located with a header that contains a reference to the folder name.

## Decision

Restructure ADRs following the pattern used in https://github.com/dotnet/csharplang/tree/main/proposals.
Each accepted ADR will now be placed in the version folder corresponding to when it was accepted.
Proposed adrs will now be placed in ADR folder

## Consequences

This results in a simplified and clearer file structure for working with ADRs.
