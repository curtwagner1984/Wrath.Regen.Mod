# Publish Guide

This guide describes how to publish `Wrath.Regen.Mod` as a Unity Mod Manager mod for:

- Nexus Mods
- GitHub Releases
- ModFinder

The recommended setup is:

- Nexus Mods for discoverability and downloads
- GitHub Releases as the canonical release host
- ModFinder pointing at the GitHub release when possible

That gives the smoothest install/update story for Wrath players.

## Release Layout

The published archive should contain the mod folder exactly as a user would place it in the game `Mods` directory:

```text
Wrath.Regen.Mod/
  Info.json
  Wrath.Regen.Mod.dll
  Wrath.Regen.Mod.pdb
  README.md
```

Notes:

- `Wrath.Regen.Mod.pdb` is optional, but useful for debugging.
- Do not ship `Settings.xml`.
- Do not ship local logs.
- Do not ship `src/`, `obj/`, `.idea/`, or solution/project files.

## Before You Publish

Before every release:

1. Make sure the repo is clean.
2. Run a final build:

```powershell
dotnet build
```

3. Update [Info.json](./Info.json):
   - `Version`
   - `HomePage`
   - `Repository`
4. Make sure the version in `Info.json` matches the release version you intend to publish.
5. Test the built mod in game one last time.

Suggested `Info.json` fields:

```json
{
  "Id": "Wrath.Regen.Mod",
  "DisplayName": "Wrath Regen Mod",
  "Author": "mihae",
  "Version": "0.1.0",
  "ManagerVersion": "0.27.0",
  "Requirements": [],
  "AssemblyName": "Wrath.Regen.Mod.dll",
  "EntryMethod": "WrathRegenMod.Main.Load",
  "HomePage": "https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/...",
  "Repository": "https://github.com/<you>/<repo>"
}
```

## Build The Release Files

This project already builds the main release files into [bin](./bin):

- [Info.json](./bin/Info.json)
- [Wrath.Regen.Mod.dll](./bin/Wrath.Regen.Mod.dll)
- [Wrath.Regen.Mod.pdb](./bin/Wrath.Regen.Mod.pdb)

If you want `README.md` included in the archive, copy it into the release staging folder before zipping.

## Create A Release Zip

Create a temporary staging folder shaped like the real mod folder:

```text
release/
  Wrath.Regen.Mod/
    Info.json
    Wrath.Regen.Mod.dll
    Wrath.Regen.Mod.pdb
    README.md
```

Then zip the `Wrath.Regen.Mod` folder itself, not only its contents.

Example PowerShell flow:

```powershell
New-Item -ItemType Directory -Force -Path .\release\Wrath.Regen.Mod | Out-Null
Copy-Item .\bin\Info.json .\release\Wrath.Regen.Mod\
Copy-Item .\bin\Wrath.Regen.Mod.dll .\release\Wrath.Regen.Mod\
Copy-Item .\bin\Wrath.Regen.Mod.pdb .\release\Wrath.Regen.Mod\
Copy-Item .\README.md .\release\Wrath.Regen.Mod\
Compress-Archive -Path .\release\Wrath.Regen.Mod -DestinationPath .\release\Wrath.Regen.Mod-0.1.0.zip -Force
```

After creating the zip, test it manually:

1. Extract it somewhere temporary.
2. Confirm it produces a `Wrath.Regen.Mod` folder.
3. Confirm that folder contains `Info.json` and `Wrath.Regen.Mod.dll`.

## Publish To Nexus Mods

1. Log into Nexus Mods.
2. Go to the `Pathfinder: Wrath of the Righteous` game page.
3. Create a new mod page.
4. Fill in:
   - name
   - summary
   - full description
   - version
   - category/tags
   - installation instructions
5. Upload the release zip.
6. In the description, explain that this is a UMM mod and should be extracted into the game `Mods` directory.

Recommended install text for the Nexus page:

```text
Extract the archive into:
<Pathfinder Wrath install folder>\Mods\

The final path should look like:
<Pathfinder Wrath install folder>\Mods\Wrath.Regen.Mod\Info.json
```

Recommended extra details for the Nexus page:

- supported game version, if you want to track that
- whether the mod is safe to add mid-save
- major supported features
- known limitations
- whether ModFinder/GitHub Releases is also available

## Publish To GitHub Releases

GitHub Releases are strongly recommended even if Nexus is your main page.

1. Push your latest commits.
2. Create a git tag that matches the release version.
3. Push the tag.
4. Create a GitHub Release from that tag.
5. Upload the same release zip used for Nexus.
6. Copy the GitHub Release URL for use in ModFinder discussions or PRs.

Example:

```powershell
git tag 0.1.0
git push origin main
git push origin 0.1.0
```

Important:

- Keep the Git tag version aligned with `Info.json`.
- If you use `v0.1.0` style tags instead, stay consistent and verify that any external tooling still resolves them correctly.

## Publish To ModFinder

ModFinder is maintained separately, so your mod is not added automatically just because it exists on Nexus or GitHub.

The usual flow is:

1. Publish the mod first, ideally as a GitHub Release.
2. Open the ModFinder repository:
   - `Pathfinder-WOTR-Modding-Community/ModFinder`
3. Ask for your mod to be added, or submit a PR if you are comfortable doing it yourself.
4. Provide:
   - mod name
   - author
   - repository URL
   - release URL
   - Nexus URL
   - version
   - a short description

The ModFinder repo keeps its mod catalog in its internal manifest, so adding support usually means adding a new entry there and pointing it at the release source.

Recommended metadata to provide in the request:

- `Id`: `Wrath.Regen.Mod`
- `DisplayName`: `Wrath Regen Mod`
- version
- GitHub repo
- GitHub release asset URL
- Nexus page URL
- short summary of what the mod restores

## Recommended Release Process

Use this as the default release checklist:

1. Finish changes and commit them.
2. Update version in [Info.json](./Info.json).
3. Run `dotnet build`.
4. Create the release zip.
5. Test the zip manually.
6. Publish a GitHub Release.
7. Publish the same zip on Nexus.
8. Request or submit a ModFinder entry.
9. Update `HomePage` and `Repository` in [Info.json](./Info.json) if needed for the next release.

## Suggested First Release Notes

For the current state of the mod, a good first release summary would be:

- health regeneration
- spontaneous spell slot regeneration
- prepared spell slot regeneration
- generic ability resource regeneration
- Kineticist burn regeneration
- configurable settings and logging
- optional visual effects for resource restoration

## Sources

These were the main references used when writing this guide:

- ModFinder repository:
  - <https://github.com/Pathfinder-WOTR-Modding-Community/ModFinder>
- ModFinder internal manifest location:
  - <https://github.com/Pathfinder-WOTR-Modding-Community/ModFinder/blob/main/ManifestUpdater/Resources/internal_manifest.json>
