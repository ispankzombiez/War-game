# War-game Unity MVP (Portrait 2D Siege Battler)

## 1) Project summary
This repository now contains a Unity-ready MVP foundation for a portrait mobile 2D siege battler. The game loop is: deploy allied squads from the bottom lane, units auto-fight defenders, and then attack the enemy gate. Destroy gate = victory. If deploys are exhausted and no allied units remain while the gate survives = defeat.

## 2) Target Unity version
- **Unity 2022.3 LTS** (recommended: `2022.3.35f1`)
- Uses built-in Unity UI (`UnityEngine.UI`) and simple placeholder sprites created at runtime.

## 3) Current MVP features
- Portrait-oriented 2D battlefield setup
- Deploy button (mouse-click works in Editor, tap works on mobile)
- Deploy count tracking
- Allied squad spawn per deploy
- Enemy defenders spawned at battle start
- Auto-targeting combat (nearest enemy first)
- Allied fallback target is enemy gate
- Gate HP display in UI + visual gate HP fill
- Victory/defeat flow
- Restart button

## 4) Repository/file structure overview

```text
Assets/
  Art/                  (placeholder folder)
  Prefabs/              (placeholder folder)
  Scenes/               (store your scene here)
  Scripts/
    Bootstrap/
      BattleBootstrap.cs
    Core/
      BattleManager.cs
      Health.cs
      Team.cs
      UnitSpawner.cs
    Structures/
      Gate.cs
    UI/
      BattleUI.cs
    Units/
      Unit.cs
README.md
```

## 5) How to open the project in Unity
If this repo is still missing Unity-generated project files (`ProjectSettings`, `Packages`), initialize the project directly in this folder:

1. Open Unity Hub.
2. Click **New project** → **2D (Core)**.
3. Set project location to this repository root.
4. Use Unity `2022.3 LTS`.
5. Create/open project.

Unity will keep the `Assets` content already committed here.

## 6) How to run the battle scene
1. Create a scene at `Assets/Scenes/BattleScene.unity`.
2. In that scene, create an empty GameObject named `Bootstrap`.
3. Attach `BattleBootstrap` to `Bootstrap`.
4. Save scene and press **Play**.

The bootstrap script builds the camera, battlefield, units, gate, and UI at runtime.

## 7) How the deploy mechanic works
- Press **DEPLOY** to consume one deploy charge.
- Each deploy spawns a small allied squad near the bottom.
- Allies auto-move toward nearest enemy defenders.
- When defenders are gone, allies target and damage the enemy gate.

## 8) Script responsibilities
- `Health.cs`: shared health/damage/death events.
- `Unit.cs`: unit stats, nearest-target logic, movement, attacking, death cleanup.
- `Gate.cs`: gate objective with health visual updates.
- `UnitSpawner.cs`: deploy count + allied squad spawning.
- `BattleManager.cs`: battle state, victory/defeat checks, deploy command entry point.
- `BattleUI.cs`: deploy UI, gate HP text, result panel, restart handling.
- `BattleBootstrap.cs`: runtime scene construction and wiring of all systems.

## 9) Manual setup steps still required
- Create/open Unity 2D project in this repo folder (if not already a Unity project).
- Create `BattleScene.unity`.
- Add one empty GameObject with `BattleBootstrap` attached.
- Optional: add scene to Build Settings for mobile build workflow.

## 10) Suggested next improvements
- Replace primitive placeholders with sprites/animations.
- Add ally/enemy unit variety and costs.
- Add cooldown-based deploy pacing and FX feedback.
- Add lane/path constraints and better collision separation.
- Add audio, polish, and mobile-safe-area UI handling.
- Convert runtime-generated objects into authored prefabs/scene content.
