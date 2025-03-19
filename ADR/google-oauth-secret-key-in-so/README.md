# Decision record for google oauth secret key in so

## Status

accepted

## Context

After extensive research, we became frustrated with implementing Google OAuth 2.0 authentication without storing the secret key in the repository.
Therefore, we settled on the current simple solution, but we're leaving users the freedom to implement their own solution.

## Decision

Store google oauth client secret inside scriptable object and allow it to be commited to end user's repository

## Consequences

Google oauth client secret will be imprinted to any repository history (considerably unsafe)
