# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Project Is

A Unity 2022.3 LTS (URP) Windows desktop application for teaching military equipment operation — two vehicle types: 装卡车 (loading truck) and 起重起卷机车 (crane/hoist vehicle). The output is a standalone `.exe`. All C# source code lives in `Assets/Scripts/`; Unity scene files, packages, and Editor settings are managed by whoever opens the project in the Unity Editor.

## Running Tests

Tests use Unity Test Framework (NUnit). **They cannot be run from the terminal** — open the project in Unity Editor, then use **Window > General > Test Runner > EditMode > Run All**.

Test files live in `Assets/Tests/EditMode/`. The five test files cover `TrainingConfig`, `JsonConfigLoader`, `DatabaseManager`, `TrainingStateMachine`, and `ScoringEngine`/`StepValidator`.

Assembly Definition (`.asmdef`) files must be created manually inside Unity Editor:
- One for `Assets/Scripts/` (runtime assembly)
- One for `Assets/Tests/EditMode/` (test assembly, references NUnit and the runtime assembly)

## Building

**File > Build Settings** in Unity Editor. Scenes must be added in order:

| Index | Scene |
|-------|-------|
| 0 | Assets/Scenes/MainMenu.unity |
| 1 | Assets/Scenes/StructureDisplay.unity |
| 2 | Assets/Scenes/PrincipleDemo.unity |
| 3 | Assets/Scenes/OperationTraining.unity |
| 4 | Assets/Scenes/ScoreManagement.unity |

Player Settings: IL2CPP backend, .NET Standard 2.1, 1920×1080. See `docs/unity-setup/windows-build-setup.md` for the full checklist.

Required manual steps before build:
- Place `SQLite.cs` (sqlite-net-pcl) in `Assets/Plugins/sqlite-net/`
- Place `sqlite3.dll` in `Assets/Plugins/x86_64/`
- Install `com.unity.nuget.newtonsoft-json` and `com.unity.addressables` via Package Manager
- Build Addressable bundles: **Window > Asset Management > Addressables > Groups > Build > New Build**
- See `docs/unity-setup/addressable-lod-setup.md` for Addressable group names and LOD face-count targets

## Architecture

Four layers, top to bottom:

**UI Toolkit layer** — All UI is Unity UI Toolkit (UXML + USS). UXML files in `Assets/UI/UXML/`, shared theme in `Assets/UI/USS/Theme.uss`. Controllers in `Assets/Scripts/UI/` call `rootVisualElement.Q<T>("element-name")` to bind elements.

**3D Scene layer** — `Assets/Scripts/Model/` handles mouse input (orbit/pan/zoom via `ModelOrbitController`), raycast-based part selection (`PartSelector` fires `OnPartSelected` event with `PartInfo`), camera presets, and explode-view animation (`ExplodeViewController` snapshots original positions in `Start()` and lerps via coroutine).

**C# State Machine / Training layer** — `TrainingStateMachine` (pure C#, no `MonoBehaviour`) enforces `Idle → Playing → Complete` transitions. `TrainingFlowController` (MonoBehaviour) is the integration hub: it receives `PartSelector.OnPartSelected`, delegates to `StepValidator.Validate(step, objectName)`, drives `TrainingSession` (step advance or error record), and calls `FinishTraining()` which inserts a `ScoreRecord` and loads the score scene. `TrainingTimer` (MonoBehaviour) tracks elapsed time and fires `OnTimeout`.

**Data layer** — JSON configs are read from `Assets/StreamingAssets/configs/` at runtime via `JsonConfigLoader.LoadFromFile(fileName)` using `File.ReadAllText` (Windows-only; do not use on other platforms). Deserialization uses Newtonsoft.Json with `[JsonProperty]` on all fields. `DatabaseManager` wraps sqlite-net-pcl; call `InsertScore` and read the return value (uses `SELECT last_insert_rowid()` — the input object's `.Id` field is NOT populated by sqlite-net-pcl after insert).

## Key Singletons

`AppManager` and `AudioManager` both use `DontDestroyOnLoad` with the guard `if (Instance != null && Instance != this)`. `SceneLoader` also uses `DontDestroyOnLoad`. All three live on GameObjects in the MainMenu scene and persist across scene loads.

`AppManager.Instance.Session` (a `UserSession` POCO) carries `TraineeName`, `TraineeId`, `SelectedEquipmentType` (filename string, e.g. `"sample_training.json"`), and `CurrentScoreRecordId` (set by `TrainingFlowController.FinishTraining()`, read by `ScoreReportController`).

## Training Configuration

Training flows are entirely JSON-driven — no recompile needed to change course content. Place `.json` files in `Assets/StreamingAssets/configs/`. See `sample_training.json` for the full schema. Key constraint: `score_weight` values across all steps must sum to exactly 100 (`TrainingConfig.ValidateTotalWeight`). The `trigger_action` and `trigger_params` fields are deserialized but not yet validated by `StepValidator` (only `target_object` name-matching is implemented).

To add a new equipment type, add its JSON config and register it in `MainMenuController.EquipmentConfigMap` (currently all entries point to `sample_training.json` as placeholder — a `Debug.LogWarning` fires for any non-first selection).

## Voice Audio

Voice files are `.mp3`, loaded at runtime from `Assets/StreamingAssets/configs/audio/` via `UnityWebRequestMultimedia.GetAudioClip` with a `file://` URI. The `voice_file` field in JSON is a relative path from `StreamingAssets` (e.g. `"configs/audio/step_01.mp3"`).

## Model Pipeline

SolidWorks STEP/IGES → PiXYZ Studio or CAD Exchanger (decimation) → FBX → Unity import. Target face counts: LOD0 ≤100k, LOD1 ≤30k, LOD2 ≤10k. Place FBX files in `Assets/Models/<EquipmentType>/<TypeA|TypeB>/`. See `docs/unity-setup/addressable-lod-setup.md` for Addressable group names.
