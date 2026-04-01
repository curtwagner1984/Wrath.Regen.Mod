# Wrath Regen Mod

Minimal Unity Mod Manager scaffold for Pathfinder: Wrath of the Righteous.

## What This Gives You

- A `net472` C# mod project
- `Info.json` manifest for UMM
- A working `Main.Load` entrypoint
- A simple in-game settings toggle
- Optional post-build deploy into the game's `Mods` folder

## Prerequisites

- Wrath installed locally
- Visual Studio or Rider
- .NET Framework 4.7.2 targeting pack / desktop build tools
- Unity Mod Manager already working in Wrath
- A decompiler such as dnSpyEx or ILSpy

## First-Time Setup

1. Open [Wrath.Regen.Mod.csproj](./Wrath.Regen.Mod.csproj).
2. Set your game folder locally by either:
   - defining the `WRATH_GAME_DIR` environment variable, or
   - copying `Wrath.Regen.Mod.local.props.example` to `Wrath.Regen.Mod.local.props` and editing `WrathGameDir`
3. Confirm these files exist under your game install:
   - `Wrath_Data\Managed\Assembly-CSharp.dll`
   - `Wrath_Data\Managed\0Harmony.dll`
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

## What To Expect In Game

The first build should do almost nothing except:

- register with UMM
- write a log line when loaded
- show a small settings UI with one toggle

That is intentional. Once the scaffold is loading, you can safely move on to actual game behavior.

## Recommended Next Steps

1. Confirm the mod loads without errors.
2. Add logging around one known game event or update loop.
3. Implement a tiny out-of-combat-only regeneration tick.
4. Add settings for enable/disable and tick interval.
5. Only after that, add more advanced resource logic.

## Good Files To Edit Next

- [src/Main.cs](./src/Main.cs)
- [src/ModSettings.cs](./src/ModSettings.cs)
- [src/RegenController.cs](./src/RegenController.cs)
