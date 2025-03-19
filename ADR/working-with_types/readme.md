# Decision record for type handling

## Status

accepted

## Context

To release early and prove the concept of the tool, we must limit our type generation and validation

## Decision

We decided to implement configuration on the unity side with the mapping of type aliases from Google sheets to fully 
qualified c# domain type names. 
On Google sheets level user must define a set of aliases which will be used as type names in sheets with actual configs
These aliases then will be transformed to c# by using configuration in unity editor

## Consequences

### Positive
1. Non-developers can use simple type names in sheets without understanding C# namespaces
2. Full C# type names ensure proper type checking and type safety
3. Custom mappings allow for project-specific type handling
4. Unity editor can validate mappings and prevent errors before runtime


### Negative

1. User should keep type aliases in sheets and Unity mappings in sync
