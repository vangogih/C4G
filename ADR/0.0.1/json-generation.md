# Decision record for JSON generation

## Status

accepted

## Context

We must provide processed and serialized data from the initial data source to the user
to provide flexibility in runtime config data placement.

## Decision

Generate valid JSON with config data according to specifically formatted input data.

## Consequences

Users can place the JSON-serialized configs wherever they like.
Users have to manage the JSON flow themselves.
