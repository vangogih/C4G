# Decision record for Google OAuth secret key in ScriptableObject

## Status

accepted

## Context

After extensive research, we became frustrated with implementing Google OAuth 2.0 authentication without storing the secret key in the repository.
Therefore, we settled on the current simple solution, but we are leaving users the freedom to implement their own solution.

## Decision

Store the Google OAuth client secret inside a ScriptableObject and allow it to be committed to the end user's repository.

## Consequences

The Google OAuth client secret will be embedded in the repository history (considerably unsafe).
