# LUDO Philosophy

## What it wants to solve
- Goal: Ship small games fast, keep code changeable, avoid “big framework” gravity.
- Constraints: Solo/small team, Unity 6, WebGL/mobile targets, short sessions, tight loops.
- Invariants: Deterministic boot, in-editor start from any scene, clear ownership, low coupling, easy to delete/replace.

## Architecture Overview
- **Core** – bootstrapping, the service locator and small utilities shared across the project.
- **Scenes** – scene loading helpers and a light state-machine framework.
- **Attributes** – ScriptableObject data used to configure gameplay.
- **UI** – menu and HUD components.
- **Pools** – simple object pooling to avoid allocations.
- **Localization** – basic string tables for multiple languages.

Each module lives under `Assets/Ludo` and is designed to be replaced or removed without touching the rest of the code.

## App Root & Service Locator
- `AAppRoot` is the persistent entry component. `Game.Core.AppRoot` (under `Assets/Game/Scripts/Core`) derives from it to set frame rate and plug services like the event hub, localization, scene loading and pooling.
- `ServiceLocator` is a tiny static registry where these services are registered so any system can retrieve them with `ServiceLocator.Get<T>()` and swap them out when needed.

## Scene Flow
Scenes are orchestrated through a tiny state machine.

- A scene hosts a `SceneFlowController<TEvent>` component.
- The controller creates the initial `FlowState<TEvent>` and drives a `StateMachine<TEvent>`.
- States implement `Enter`, `Tick`, `Exit` and `Handle` to react to events and request transitions.
- `SceneService` loads or unloads Unity scenes when the flow moves between them.

This keeps transitions explicit and deterministic while remaining easy to reason about.

