# Wrath Regen Mod

Unity mod for Pathfinder: Wrath of the Righteous using UnityModManager.

## Build

```
dotnet build
```

Requires `WRATH_GAME_DIR` environment variable pointing to the game installation (set via `Wrath.Regen.Mod.local.props`).

## Key rules for this codebase

### Keep the tick path allocation-free

Everything in `OnUpdate` / `Tick` runs every frame or every few seconds for every unit. Avoid allocations on this path:

- **Log calls**: Always guard interpolated log strings with `IsVerbose` / `IsInfo` / `IsError` before calling the log method. The interpolated string is allocated *before* it's passed to the logger, regardless of log level.
- **Dictionary keys**: Use value tuples `(Spellbook, int)` or direct object references — never build string keys per tick.
- **LINQ materializations**: Don't call `.ToList()` unless you actually need indexed access or count. Iterate the `IEnumerable` directly and use a counter if needed.

### Clean up long-lived state on area transitions

Dictionaries keyed by game objects (units, spellbooks) will hold stale entries after area transitions. Subscribe to `EventBus` with `IAreaHandler` and clear dictionaries in `OnAreaDidLoad`. Unsubscribe when the mod is disabled.

### Use game APIs over hardcoded values

Use `spellbook.Blueprint.MaxSpellLevel` instead of hardcoding `9`. When the game value exceeds what settings support, fall back to the highest configured tier rather than silently skipping.

### Nullability should be intentional

Don't use `?.` on parameters that are guaranteed non-null by their callers. Null-conditional operators signal "this might be null" to readers — use them only when that's true.
