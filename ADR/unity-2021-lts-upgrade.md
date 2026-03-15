# Decision record for upgrading Unity editor to 2021 LTS

## Status

proposed

## Context

The project's CI/CD pipelines rely on [game-ci](https://game.ci) to build and test Unity projects.
game-ci dropped support for Unity 2019 and 2020 editor versions, making it impossible to run CI workflows for those versions going forward.
The project was previously targeting Unity 2019/2020, which left it without viable CI support.
Unity 2021 LTS (Long Term Support) is the next supported major version with active game-ci support and official Unity maintenance.

See: [game-ci/versioning-backend#72](https://github.com/game-ci/versioning-backend/pull/72)

## Decision

Upgrade the project Unity editor version to **2021.3.45f2 LTS** and drop support for Unity 2019 and 2020.

## Consequences

CI pipelines remain functional under game-ci support.
Developers must use Unity 2021.3.45f2 or a compatible 2021 LTS release.
Unity 2019/2020-specific workarounds and compatibility code can be removed.
The project benefits from the extended support window of the 2021 LTS release cycle.
