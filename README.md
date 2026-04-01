# Wrath Regen Mod

`Wrath Regen Mod` is a Unity Mod Manager mod for `Pathfinder: Wrath of the Righteous` that restores resources over time.

The mod is designed around runtime capability detection rather than hardcoding a single class flow, so it can already handle several major resource systems used by Wrath classes.

## Current Features

- Out-of-combat health regeneration
- Spontaneous spell slot regeneration
- Prepared spell slot regeneration
- Generic ability resource regeneration
  - examples: `Lay on Hands`, `Smite Evil`, and other limited-use class ability pools
- Kineticist burn regeneration with a configurable floor
- Optional visual effects for spell and resource restoration
- In-game UMM settings UI with tabs and structured configuration
- Configurable logging levels for debugging and troubleshooting

## What The Mod Supports Right Now

The current release works best for:

- spontaneous casters such as Oracle
- prepared casters such as Paladin and Eldritch Scoundrel
- classes and archetypes that use standard `BlueprintAbilityResource` pools
- Kineticists using fixed-floor burn restoration

Some classes and archetypes still have unique mechanics that may need deeper class-specific handling later, but the current system already covers a large part of the common resource-restoration cases.

## Installation

Extract the mod into your Wrath `Mods` folder so the final path looks like:

```text
<Pathfinder Wrath install folder>\Mods\Wrath.Regen.Mod\Info.json
```

If you are using a packaged release zip, it should already contain the correct `Wrath.Regen.Mod` folder layout.

## Prerequisites For Building

- Wrath installed locally
- Visual Studio or Rider
- .NET Framework 4.8 developer tools / targeting pack
- Unity Mod Manager already working in Wrath
- A decompiler such as `dnSpyEx`, `ILSpy`, or `ilspycmd`

## First-Time Setup

1. Open [Wrath.Regen.Mod.csproj](./Wrath.Regen.Mod.csproj).
2. Set your local game folder by either:
   - copying `Wrath.Regen.Mod.local.props.example` to `Wrath.Regen.Mod.local.props` and editing `WrathGameDir`, or
   - editing the local path configuration another way if you prefer
3. Confirm these files exist under your game install:
   - `Wrath_Data\Managed\Assembly-CSharp.dll`
   - `Wrath_Data\Managed\UnityModManager\0Harmony.dll`
   - `Wrath_Data\Managed\UnityModManager\UnityModManager.dll`
4. Build the project.
5. Check that these files appear in `<WrathGameDir>\Mods\Wrath.Regen.Mod\`:
   - `Wrath.Regen.Mod.dll`
   - `Info.json`
6. Launch the game and verify the mod appears in UMM.

Example local props file:

```xml
<Project>
  <PropertyGroup>
    <WrathGameDir>D:\SteamLibrary\steamapps\common\Pathfinder Second Adventure</WrathGameDir>
  </PropertyGroup>
</Project>
```

## Build

Run:

```powershell
dotnet build
```

The project builds the main output into [bin](./bin) and also copies the active mod files into the game `Mods\Wrath.Regen.Mod` directory when the local path is configured.

Typical build outputs:

- [bin/Info.json](./bin/Info.json)
- [bin/Wrath.Regen.Mod.dll](./bin/Wrath.Regen.Mod.dll)
- [bin/Wrath.Regen.Mod.pdb](./bin/Wrath.Regen.Mod.pdb)

## Logging

The mod supports multiple log levels in the in-game settings:

- `Error`
- `Info`
- `Verbose`

The main UMM log file is usually located at:

```text
<Pathfinder Wrath install folder>\Wrath_Data\Managed\UnityModManager\Log.txt
```

You can also optionally mirror mod messages into the in-game event log from the settings UI.

## Project Structure

Useful entry points in the codebase:

- [src/Main.cs](./src/Main.cs)
- [src/ModSettings.cs](./src/ModSettings.cs)
- [src/UI/SettingsRenderer.cs](./src/UI/SettingsRenderer.cs)
- [src/Infrastructure/ModLogger.cs](./src/Infrastructure/ModLogger.cs)
- [src/Features/HealthRegen](./src/Features/HealthRegen)
- [src/Features/ResourceRegen](./src/Features/ResourceRegen)

Research and implementation notes live in:

- [Tutorial.md](./Tutorial.md)
- [docs](./docs)

## Publishing

Release and packaging instructions are documented in:

- [Publish Guide.md](./Publish%20Guide.md)

## Status

This mod has moved well beyond the original scaffold phase. It now has a working multi-system regeneration prototype suitable for an initial public release, with room to expand into deeper class- and archetype-specific handling over time.
