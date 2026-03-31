# Wrath Modding Tutorial

This file is our living guide for building `Wrath.Regen.Mod`.

We will update it as we learn more about Pathfinder: Wrath of the Righteous modding, discover useful APIs, and make real progress in the mod.

## Goal

Build a small Unity Mod Manager mod for Pathfinder: Wrath of the Righteous that eventually adds controlled regeneration behavior.

## What We Have Working

- A minimal UMM mod project builds successfully.
- The mod appears in the in-game mod manager.
- The mod has a settings UI.
- The mod runs an update loop through `OnUpdate`.
- The mod writes log output into the Unity Mod Manager log.

## Important Files

- [Wrath.Regen.Mod.csproj](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/Wrath.Regen.Mod.csproj)
- [Info.json](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/Info.json)
- [src/Main.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/Main.cs)
- [src/ModSettings.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/ModSettings.cs)
- [src/Features/Diagnostics/PartyProbeController.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/Features/Diagnostics/PartyProbeController.cs)
- [src/Features/HealthRegen/HealthRegenController.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/Features/HealthRegen/HealthRegenController.cs)
- [src/UI/SettingsRenderer.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/UI/SettingsRenderer.cs)
- [README.md](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/README.md)

## Build Setup

### Target framework

- The project targets `.NET Framework 4.8` via `net48`.
- We originally tried `net472`, but the installed Harmony/UMM combination in this Wrath install expects `.NET Framework 4.8`.

### Game path

- Current game root:
  `D:\SteamLibrary\steamapps\common\Pathfinder Second Adventure`

### Key references

The project references game and modding DLLs from:

- `D:\SteamLibrary\steamapps\common\Pathfinder Second Adventure\Wrath_Data\Managed`
- `D:\SteamLibrary\steamapps\common\Pathfinder Second Adventure\Wrath_Data\Managed\UnityModManager`

Important DLLs:

- `Assembly-CSharp.dll`
- `UnityModManager\UnityModManager.dll`
- `UnityModManager\0Harmony.dll`
- `UnityEngine.CoreModule.dll`
- `UnityEngine.IMGUIModule.dll`

## Where The Mod Is Deployed

The built mod is copied into:

- `D:\SteamLibrary\steamapps\common\Pathfinder Second Adventure\Mods\Wrath.Regen.Mod`

Expected files there:

- `Wrath.Regen.Mod.dll`
- `Info.json`

## Logging

Unity Mod Manager writes logs to:

- [Log.txt](/D:/SteamLibrary/steamapps/common/Pathfinder%20Second%20Adventure/Wrath_Data/Managed/UnityModManager/Log.txt)

Our current test messages include:

- `PartyProbeController is running.`
- `HealthRegenController is running. Prototype healing uses Wrath's built-in rule system.`

This confirms:

- the mod loads
- the update loop is active
- our settings UI is connected

## What We Learned About Wrath Modding

### Unity is not required for this kind of mod

For a code-driven gameplay or QoL mod, we do not need the Unity Editor yet.

What we do need:

- C# project
- game DLL references
- UMM mod manifest
- logging
- a decompiler or assembly inspection workflow

### Decompiling is normal

For Wrath modding, one of the main workflows is:

1. Identify the gameplay system we care about.
2. Inspect game assemblies to find relevant classes, properties, and methods.
3. Add safe logging first.
4. Confirm runtime behavior in game.
5. Only then modify behavior.

This is normal and expected in Wrath modding.

## Tools We Use

### Core tools

- Rider or Visual Studio for editing and building the mod
- `dotnet` CLI for building and tool management
- Unity Mod Manager for loading the mod in game
- `ilspycmd` for command-line decompilation
- UMM log file for runtime debugging

### Why we use them

- IDE: edit C# code and manage the project comfortably
- `dotnet build`: compile and deploy the DLL
- Unity Mod Manager: load the mod and expose the settings UI
- `ilspycmd`: inspect game classes and methods directly from `Assembly-CSharp.dll`
- `Log.txt`: verify that our runtime assumptions are correct

## Installing The Decompiler Tool

We installed `ilspycmd` as a global .NET tool.

Command used:

```powershell
dotnet tool install --global ilspycmd
```

We verified the install with:

```powershell
ilspycmd --version
```

At the time of setup, this returned:

- `ilspycmd: 9.1.0.7988`
- `ICSharpCode.Decompiler: 9.1.0.7988`

## How We Use ilspycmd

General pattern:

```powershell
ilspycmd -t Full.Type.Name "D:\SteamLibrary\steamapps\common\Pathfinder Second Adventure\Wrath_Data\Managed\Assembly-CSharp.dll"
```

Example:

```powershell
ilspycmd -t Kingmaker.Game "D:\SteamLibrary\steamapps\common\Pathfinder Second Adventure\Wrath_Data\Managed\Assembly-CSharp.dll"
```

This lets us:

- decompile a specific type
- inspect properties and methods
- search for the right runtime APIs to use in our mod

## Debugging Workflow

Current workflow for new features:

1. Identify a gameplay concept we want to read or modify.
2. Inspect likely classes with `ilspycmd`.
3. Add small logging code to the mod.
4. Build the mod.
5. Reload the game and inspect the UMM log.
6. Record what we learned in this tutorial.

## Useful Runtime Types We Found

These were found by inspecting `Assembly-CSharp.dll` and are promising for our regen work.

### Game and player access

- `Kingmaker.Game`
- `Kingmaker.Game.Instance`
- `Kingmaker.Game.Instance.Player`
- `Kingmaker.Player.Party`
- `Kingmaker.Player.PartyAndPets`
- `Kingmaker.Player.ActiveCompanions`

### Unit data

- `Kingmaker.EntitySystem.Entities.UnitEntityData`
- `UnitEntityData.MaxHP`
- `UnitEntityData.Stats`
- `UnitEntityData.State`
- `UnitEntityData.CombatState`

### HP and state

- `Kingmaker.EntitySystem.Stats.CharacterStats`
- `CharacterStats.HitPoints`
- `Kingmaker.EntitySystem.Stats.ModifiableValueHitPoints`
- `ModifiableValueHitPoints.ModifiedValue`
- `UnitState.IsDead`
- `UnitState.IsConscious`
- `UnitCombatState.IsInCombat`

## Healing Prototype Findings

For the health prototype, we inspected Wrath's built-in healing path instead of editing HP directly.

### Important healing types

- `Kingmaker.RuleSystem.Rules.Damage.RuleHealDamage`
- `Kingmaker.UnitLogic.Mechanics.Actions.ContextActionHealTarget`
- `Kingmaker.UnitLogic.FactLogic.SufferFromHealing`
- `Kingmaker.PubSubSystem.IHealingHandler`

### What this means

- `RuleHealDamage` is the game's built-in healing rule.
- Triggering it is better than mutating HP manually.
- It fires the game's healing hooks and should participate in the normal logging/event systems.
- `ContextActionHealTarget` also uses `RuleHealDamage`, which confirms this is the right direction for a prototype.

### Undead caveat

`SufferFromHealing` intercepts `RuleHealDamage` and converts healing into damage.

That means:

- normal positive-energy healing is not safe for undead
- if we blindly used `RuleHealDamage` on an undead target, the prototype could hurt them

The prototype now checks:

- `UnitDescriptor.IsUndead`

and routes undead restoration through:

- `RuleDealDamage`
- `EnergyDamage`
- `DamageEnergyType.NegativeEnergy`

This follows the game's own damage/healing pipeline instead of mutating HP directly.

We also found a useful supporting system in:

- `AddEnergyDamageImmunity`

which can convert incoming energy damage into healing through `HealOnDamage`.

That makes the negative-energy path a reasonable prototype for undead restoration.

## Pets And Summons

We confirmed that pets are represented separately from the core party list.

Relevant APIs:

- `Player.Party`
- `Player.PartyAndPets`
- `UnitEntityData.IsPet`
- `UnitEntityData.Master`

So pet support should come from `PartyAndPets`, not by guessing ownership manually.

For summons, a useful marker is:

- `UnitPartSummonedMonster`

The current prototype includes separate configuration toggles for:

- pets
- summons

with summons discovered from `Player.AllCrossSceneUnits` and filtered to player-side active summoned units.

## In-Game Log Integration

We found a clean way to mirror our own mod messages into Wrath's in-game event log.

Relevant type:

- `Kingmaker.PubSubSystem.ILogMessageUIHandler`

It can be raised through `EventBus`, which means our mod can send plain text messages into the UI event log when enabled.

Current setting:

- `Mirror mod messages to the in-game event log`

Note:

- this can become noisy if verbose diagnostics are enabled
- built-in healing events use the game's own healing log path separately

## Current Code Structure

We started splitting the mod into clearer areas so the later resource-regeneration work stays manageable.

Current structure:

- `src/Infrastructure/ModLogger.cs`
- `src/Features/Diagnostics/PartyProbeController.cs`
- `src/Features/HealthRegen/HealthRegenController.cs`
- `src/UI/SettingsRenderer.cs`

This lets us keep:

- diagnostics separate from gameplay logic
- feature code separate from infrastructure
- UI rendering separate from mod entrypoint wiring
- future spell/resource regeneration work in its own files/folder

## Settings Architecture

The settings model is now split by responsibility instead of keeping all fields at the top level.

Current sections:

- `GeneralSettings`
- `DiagnosticsSettings`
- `HealthRegenSettings`

These are stored inside:

- [src/ModSettings.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/ModSettings.cs)

The UI is now rendered through:

- [src/UI/SettingsRenderer.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/UI/SettingsRenderer.cs)

### UI goals

- tabs per feature area
- explicit numeric entry fields instead of only sliders
- clear min/max ranges shown next to values
- short help text through tooltips and a contextual help panel

## Current Development Strategy

We are intentionally not jumping straight to “full regen logic.”

Instead, we will use these steps:

1. Confirm the game instance exists.
2. Confirm a save is loaded.
3. Read the party list.
4. Log each unit’s name, HP, max HP, and combat state.
5. Add a tiny out-of-combat-only regeneration effect.
6. Add safety checks and settings.
7. Expand carefully from there.

## Progress Update

We have now moved from a dummy tick logger to a real party snapshot logger.

The current probe in [src/Features/Diagnostics/PartyProbeController.cs](/c:/Users/mihae/antigravity/Wrath.Regen.Mod/src/Features/Diagnostics/PartyProbeController.cs) is designed to:

- wait until `Game.Instance` and `Game.Instance.Player` are available
- read `Game.Instance.Player.Party`
- log party size every interval
- optionally log each party member in verbose mode

The unit detail logger currently reads:

- `UnitEntityData.CharacterName`
- `UnitEntityData.Stats.HitPoints.ModifiedValue`
- `UnitEntityData.MaxHP`
- `UnitEntityData.CombatState.IsInCombat`
- `UnitEntityData.State.IsDead`
- `UnitEntityData.State.IsUnconscious`

This is our first real runtime inspection pass over live game data.

## Notes For Ourselves

- Prefer logging and inspection before patching behavior.
- Keep each step testable in game.
- When something is unclear, verify it from local assemblies first.
- Online resources can help, but the game DLLs are the best source of truth for exact APIs.
