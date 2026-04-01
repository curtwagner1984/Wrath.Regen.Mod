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

- [Wrath.Regen.Mod.csproj](./Wrath.Regen.Mod.csproj)
- [Info.json](./Info.json)
- [src/Main.cs](./src/Main.cs)
- [src/ModSettings.cs](./src/ModSettings.cs)
- [src/Features/Diagnostics/PartyProbeController.cs](./src/Features/Diagnostics/PartyProbeController.cs)
- [src/Features/HealthRegen/HealthRegenController.cs](./src/Features/HealthRegen/HealthRegenController.cs)
- [src/UI/SettingsRenderer.cs](./src/UI/SettingsRenderer.cs)
- [README.md](./README.md)

## Build Setup

### Target framework

- The project targets `.NET Framework 4.8` via `net48`.
- We originally tried `net472`, but the installed Harmony/UMM combination in this Wrath install expects `.NET Framework 4.8`.

### Game path

- Current game root:
  `<WrathGameDir>`

### Key references

The project references game and modding DLLs from:

- `<WrathGameDir>\Wrath_Data\Managed`
- `<WrathGameDir>\Wrath_Data\Managed\UnityModManager`

Important DLLs:

- `Assembly-CSharp.dll`
- `UnityModManager\UnityModManager.dll`
- `UnityModManager\0Harmony.dll`
- `UnityEngine.CoreModule.dll`
- `UnityEngine.IMGUIModule.dll`

## Where The Mod Is Deployed

The built mod is copied into:

- `<WrathGameDir>\Mods\Wrath.Regen.Mod`

Expected files there:

- `Wrath.Regen.Mod.dll`
- `Info.json`

## Logging

Unity Mod Manager writes logs to:

- `<WrathGameDir>\Wrath_Data\Managed\UnityModManager\Log.txt`

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
ilspycmd -t Full.Type.Name "<WrathGameDir>\Wrath_Data\Managed\Assembly-CSharp.dll"
```

Example:

```powershell
ilspycmd -t Kingmaker.Game "<WrathGameDir>\Wrath_Data\Managed\Assembly-CSharp.dll"
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

- [src/ModSettings.cs](./src/ModSettings.cs)

The UI is now rendered through:

- [src/UI/SettingsRenderer.cs](./src/UI/SettingsRenderer.cs)

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

The current probe in [src/Features/Diagnostics/PartyProbeController.cs](./src/Features/Diagnostics/PartyProbeController.cs) is designed to:

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

## Resource Regeneration Research

This section is research only. We are not implementing spell-slot or ability-resource regeneration yet.

### Main question

Can we determine at runtime what a unit can regenerate?

Current answer:

- yes, for a large part of the game this looks feasible
- Wrath already exposes spellbooks, generic ability-resource pools, progression, and unit facts on the runtime unit model
- resting is not magic; it appears to restore concrete unit systems that we can inspect and possibly drive in smaller increments

### Core runtime model

The main type is:

- `Kingmaker.UnitLogic.UnitDescriptor`

Important members we found:

- `Resources`
- `Progression`
- `Abilities`
- `ActivatableAbilities`
- `Spellbooks`
- `GetSpellbook(...)`
- `IsUndead`

This is important because it means the unit already knows:

- what spellbooks it has
- what resource pools it has
- what facts and abilities are active
- what class progression it has

That strongly suggests we can inspect a character at runtime and decide what kind of regeneration rules are relevant.

## Generic ability resources

Many limited-use class features do not appear to be special one-off storage systems.

Instead, they are often backed by:

- `Kingmaker.Blueprints.BlueprintAbilityResource`
- `Kingmaker.UnitLogic.UnitAbilityResource`
- `Kingmaker.UnitLogic.UnitAbilityResourceCollection`

Important `UnitAbilityResourceCollection` methods:

- `GetResourceAmount(BlueprintScriptableObject blueprint)`
- `Restore(BlueprintScriptableObject blueprint, int amount)`
- `Restore(BlueprintScriptableObject blueprint)`
- `FullRestoreAll(...)`
- `Spend(BlueprintScriptableObject blueprint, int amount)`
- `HasEnoughResource(...)`
- `HasMaxAmount(...)`

What this means:

- a lot of per-rest abilities are probably stored as named resource pools
- the game already has a built-in "restore some amount" API for those pools
- we may be able to regenerate many class features generically once we identify the right resource blueprint

### How abilities connect to resources

Relevant component types:

- `Kingmaker.UnitLogic.Abilities.Components.AbilityResourceLogic`
- `Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbilityResourceLogic`
- `Kingmaker.Designers.Mechanics.Facts.AddAbilityResources`

What we learned:

- `AbilityResourceLogic` points an ability at a `RequiredResource`
- spending the ability usually calls `unit.Descriptor.Resources.Spend(...)`
- `ActivatableAbilityResourceLogic` does the same for toggled/activatable abilities
- `AddAbilityResources` is the component that grants or restores a resource pool on the unit

This is a very promising sign.

It suggests that features like:

- Lay on Hands
- Smite Evil
- Judgments
- Rage rounds
- Channel Energy

may often reduce to:

1. identify the correct `BlueprintAbilityResource`
2. inspect current and max amount
3. restore some amount over time with the built-in resource API

## How max resource amounts are calculated

`BlueprintAbilityResource.GetMaxAmount(UnitDescriptor unit)` contains the blueprint-driven formula for the pool.

The max can depend on:

- class level
- archetype filters
- step-based progression formulas
- bonus stat modifiers
- event-based bonuses
- explicit min and max clamps

This is useful because it means:

- we should not hardcode resource maximums in the mod
- the game already knows the legal maximum for a resource pool
- our mod can probably work in terms of "restore N points toward max"

## Spellbooks

Spellcasting is a separate system from generic ability resources, but it also looks accessible.

Relevant type:

- `Kingmaker.UnitLogic.Spellbook`

Important members discovered through reflection:

- `CanRestoreSpells`
- `Rest`
- `GetSpellsPerDay`
- `GetSpellSlotsCount`
- `GetSpontaneousSlots`
- `RestoreSpontaneousSlots`
- `GetMemorizeSlots`
- `GetFreeSlots`
- `GetAvailableForCastSpellCount`
- `CanSpend`
- `Spend`
- `TryRestoreKnownSpells`

What this means:

- spellbooks are not opaque blobs
- the game exposes methods for slot counts, available casts, spending, and restoration
- there is a good chance we can restore spellcasting in smaller pieces instead of simulating a full rest every time

### Likely design implication

Spell slot regeneration probably belongs in a spellbook-specific subsystem, not the generic ability-resource subsystem.

That means:

- full casters like Oracle, Cleric, Wizard, Sorcerer and similar classes should probably be handled through `Spellbook`
- non-spell limited-use class features should often be handled through `UnitDescriptor.Resources`

## Rest system findings

Relevant type:

- `Kingmaker.Controllers.Rest.RestController`

Important methods we found:

- `FakeRestUnit(UnitEntityData unit)`
- `HealAndApplyRest(UnitEntityData unit, RestStatus status)`
- `ApplyRest(UnitDescriptor unit)`

What this tells us:

- Wrath has a real per-unit rest application path
- rest eventually applies concrete restoration logic to a `UnitDescriptor`
- there may be cases where we can inspect rest behavior to learn exactly which systems are restored, even if we do not want to call "full rest" directly

Current interpretation:

- using full rest APIs for periodic regeneration is probably too heavy-handed
- but the rest code is valuable as a source of truth for what systems need restoring

## Kineticist burn

Kineticist burn is not a normal generic resource pool in the same way as many other class features.

Relevant type:

- `Kingmaker.UnitLogic.Class.Kineticist.UnitPartKineticist`

Important members:

- `AcceptedBurn`
- `AcceptedBurnThisRound`
- `MaxBurn`
- `LeftBurn`
- `LeftBurnThisRound`
- `AcceptBurn(int burn, AbilityData ability)`
- `HealBurn(int value)`
- `ClearAcceptedBurn()`

What this means:

- burn has a dedicated runtime state holder
- the game already provides a built-in decrement path through `HealBurn(...)`
- kineticist regeneration should probably use that dedicated API instead of trying to treat burn like an ordinary `BlueprintAbilityResource`
- `MaxBurn` itself is backed by resources, but accepted burn state is still managed by `UnitPartKineticist`

### Overflow bonus inference

The clearest burn-bonus component found so far is:

- `Kingmaker.UnitLogic.Class.Kineticist.AddKineticistElementalOverflow`

Important behavior:

- it computes an intermediate cap with:
  - `Math.Min(Bonus.Calculate(context), unitPartKineticist.ClassLevel / 3)`
- unless `IgnoreBurn` is set, it then caps the effective bonus again by:
  - `unitPartKineticist.AcceptedBurn`

Practical conclusion:

- a possible "max useful burn" threshold exists
- but it is not exposed as a simple property on `UnitPartKineticist`
- it appears to depend on:
  - class level
  - the current overflow fact's configured bonus
  - accepted burn

This means a smart `keep burn for max bonuses` mode is plausible, but it is inference-driven rather than a simple direct read from the core burn unit part.

### Elemental Engine caveat

`AddKineticistElementalOverflow` also has an `ElementalEngine` path:

- when `unitPartKineticist.MaxBurn == unitPartKineticist.AcceptedBurn`
- the overflow bonuses are increased further

That means Elemental Engine is an important exception:

- a generic smart-floor mode may be wrong for it
- it may want to preserve full burn rather than just base overflow thresholds

This matches the gameplay concern we already discussed:

- burn affects other kineticist mechanics
- users may want a floor instead of always regenerating to zero
- kineticist support will likely need class-specific settings

## What looks generic vs class-specific

Current best guess:

Generic enough to centralize:

- many per-rest or per-day ability pools backed by `BlueprintAbilityResource`
- a lot of activatable/toggle resources
- probably many class charges that are shown as "X uses per day"

Needs dedicated handling:

- spellbooks and spell slots
- kineticist burn
- any feature whose recharge rule depends on special class state rather than a plain resource pool

Needs further verification:

- features that partially restore only specific sub-pools
- classes with multiple spellbooks or mythic/merged spellbook interactions
- edge cases where a fact uses `UseThisAsResource`

## Working hypothesis for the mod design

The mod probably should not be organized as "one class, one hardcoded implementation" for everything.

A better model is likely:

1. generic resource-regeneration engine for `BlueprintAbilityResource` pools
2. spellbook-regeneration engine for spell slots and spellbook-specific rules
3. specialized adapters for exceptional systems like kineticist burn

That still leaves room for class-specific policy.

For example:

- Paladin may want separate settings for Smite Evil and Lay on Hands even if both are generic resources underneath
- Oracle may regenerate by spell level or by spellbook-wide rules
- Kineticist may regenerate burn down to a floor rather than to zero

## Research conclusions so far

- Yes, we can determine a lot at runtime.
- No, we probably do not need to hardcode every class from the bottom up.
- A large part of the problem looks discoverable from runtime spellbooks and resource pools.
- Some systems, especially kineticist burn, clearly need dedicated handling.
- The rest system is useful as a reference model, but probably not as the direct periodic-regeneration mechanism.

## Next research targets

Before implementation, the next useful things to inspect are:

- which concrete `BlueprintAbilityResource` pools exist on a few sample classes
- how to enumerate a unit's current resource blueprints cleanly
- how prepared spellbooks vs spontaneous spellbooks should be restored incrementally
- whether mythic and merged spellbooks need special handling
- whether there are other class-specific systems like burn that expose dedicated restore methods

## Tooling note

`ilspycmd` has been very helpful, but on some very large Wrath types it can stack overflow during decompilation.

So our current research workflow is:

1. use `ilspycmd` when it can fully decompile the type
2. fall back to reflection-based method and property inspection when `ilspycmd` struggles
3. use the rest system and runtime logging later to confirm behavior in game

## Oracle And Paladin Case Study

We inspected concrete blueprint files from:

- `<WrathGameDir>\blueprints.zip`

This let us move beyond generic code structure and confirm how real class mechanics are wired.

### How to inspect blueprint data

Useful discovery:

- the game ships a searchable `blueprints.zip`
- it contains `.jbp` blueprint files by folder and feature name

That means we can often answer "what blueprint backs this class mechanic?" without writing probe code first.

## Enumerating a unit's live resource blueprints

This looks feasible at runtime.

Key pieces:

- `UnitDescriptor.Resources` is a `UnitAbilityResourceCollection`
- `UnitAbilityResourceCollection` exposes an enumerator over its blueprint keys
- each concrete entry is a `UnitAbilityResource`
- `UnitAbilityResource` exposes at least:
  - `Blueprint`
  - `Amount`
  - `RetainCount`

Implication:

- we should be able to iterate a unit's currently granted resource blueprints directly
- we do not need to guess resource names from class names alone
- for generic resource regeneration, the runtime data model is already friendly

## Oracle mapping

### Concrete blueprint files

- `Classes/Oracle/OracleClass.jbp`
- `Classes/Oracle/OracleSpellbook.jbp`
- `Classes/Oracle/OracleProgression.jbp`

### What the class blueprint tells us

`OracleClass.jbp` points directly at:

- `m_Spellbook = !bp_6c03364712b415941a98f74522a81273`

which is:

- `Classes/Oracle/OracleSpellbook.jbp`

### Oracle spellbook facts

`OracleSpellbook.jbp` is a `BlueprintSpellbook` with:

- `Spontaneous = true`
- `CastingAttribute = Charisma`
- `m_SpellsPerDay = !bp_dbff16956df2eda48a1da5c9617cc836`
- `m_SpellsKnown = !bp_d232bc78d967a964bac4d4d38e7ca5f4`
- `m_SpellList = !bp_8443ce803d2d31347897a3d85cc32f53`
- `m_CharacterClass = !bp_20ce9bf8af32bee4c8557a045ab499b1`

This is a strong confirmation that Oracle spell regeneration should be treated as spellbook regeneration, not generic resource regeneration.

### Runtime mapping for Oracle

The likely runtime path is:

1. inspect `unit.Descriptor.Spellbooks`
2. or call `unit.Descriptor.GetSpellbook(oracleClassBlueprint)`
3. operate on the resulting `Spellbook`

Relevant `Spellbook` APIs we already found earlier:

- `GetSpellsPerDay`
- `GetSpellSlotsCount`
- `GetAvailableForCastSpellCount`
- `GetSpontaneousSlots`
- `RestoreSpontaneousSlots`
- `CanRestoreSpells`
- `Rest`

### Design implication for Oracle

Your proposed settings model makes sense here.

For a spontaneous caster like Oracle, a settings model such as:

- level 1 slot every 30 seconds
- level 2 slot every 40 seconds
- level 3 slot every 60 seconds

looks conceptually compatible with the runtime model.

Main caveat:

- we still need to verify the exact best API for restoring a single spontaneous slot at a chosen spell level
- but the spellbook model clearly stores slot state per spell level, so this is not a wild guess

## Paladin mapping

Paladin turned out to be a very good example of the generic resource pattern.

### Concrete blueprint files

Core files:

- `Classes/Paladin/PaladinClass.jbp`
- `Classes/Paladin/PaladinSpellbook.jbp`
- `Classes/Paladin/PaladinProgression.jbp`

Lay on Hands files:

- `Classes/Paladin/LayOnHands/LayOnHandsFeature.jbp`
- `Classes/Paladin/LayOnHands/LayonHandsFact.jbp`
- `Classes/Paladin/LayOnHands/LayOnHandsResource.jbp`
- `Classes/Paladin/LayOnHands/LayOnHandsSelf.jbp`
- `Classes/Paladin/LayOnHands/LayOnHandsOthers.jbp`

Smite Evil files:

- `Classes/Paladin/SmiteEvil/SmiteEvilFeature.jbp`
- `Classes/Paladin/SmiteEvil/SmiteEvilResource.jbp`
- `Classes/Paladin/SmiteEvil/SmiteEvilAbility.jbp`

### Paladin spellbook facts

`PaladinSpellbook.jbp` is also a `BlueprintSpellbook`, but unlike Oracle:

- `Spontaneous = false`
- `AllSpellsKnown = true`
- `CastingAttribute = Charisma`
- `CasterLevelModifier = -3`

So Paladin has both:

- spellbook-backed spellcasting
- generic class resources like Lay on Hands and Smite Evil

This makes Paladin a useful "mixed system" example for the mod architecture.

### Smite Evil concrete runtime mapping

`SmiteEvilFeature.jbp` contains:

- `AddFacts` granting `SmiteEvilAbility`
- `AddAbilityResources` granting `SmiteEvilResource`

`SmiteEvilAbility.jbp` contains:

- `AbilityResourceLogic`
- `m_RequiredResource = !bp_b4274c5bb0bf2ad4190eb7c44859048b`

which is:

- `Classes/Paladin/SmiteEvil/SmiteEvilResource.jbp`

So the runtime picture is:

1. feature grants ability and resource
2. resource lives in `unit.Descriptor.Resources`
3. ability spends it through `AbilityResourceLogic`
4. our mod could restore it through `unit.Descriptor.Resources.Restore(smiteresource, amount)`

This is exactly the kind of generic resource-backed feature we hoped to find.

### Lay on Hands concrete runtime mapping

`LayOnHandsFeature.jbp` grants:

- a fact
- `LayOnHandsOthers`
- `LayOnHandsSelf`

The important detail is that the resource is granted by the fact:

- `Classes/Paladin/LayOnHands/LayonHandsFact.jbp`

That fact contains:

- `AddAbilityResources`
- `m_Resource = !bp_9dedf41d995ff4446a181f143c3db98c`
- `RestoreAmount = true`

which is:

- `Classes/Paladin/LayOnHands/LayOnHandsResource.jbp`

Then both Lay on Hands abilities spend that same resource through `AbilityResourceLogic`.

So the runtime picture is:

1. feature grants fact and abilities
2. fact grants resource pool
3. abilities spend resource pool
4. our mod could restore the pool generically through the unit resource collection

### Paladin resource formulas

`LayOnHandsResource.jbp` is especially useful because it shows a real max formula:

- `BaseValue = 1`
- level-based increase every 2 Paladin levels starting at level 2
- increased by `Charisma`

`SmiteEvilResource.jbp` is much simpler:

- `BaseValue = 1`
- no stat-based or step-based increase in this base blueprint

This is important because it confirms:

- we should trust blueprint max formulas instead of hardcoding counts
- two Paladin resources can use very different recharge math even though both are generic `BlueprintAbilityResource` pools

## Concrete conclusion from Oracle and Paladin

We now have one clear example of each major model:

- Oracle:
  - pure spellbook-driven class resource model for spellcasting
- Paladin:
  - generic `BlueprintAbilityResource` pools for class features
  - plus a spellbook for Paladin spells

So the architecture is getting clearer:

1. spell-slot regeneration system for spellbooks like Oracle
2. generic resource-pool regeneration system for features like Paladin Lay on Hands and Smite Evil
3. separate handling for special systems like kineticist burn

## Practical next step after research

If we continue this research before coding, the most useful next checks are:

- verify the exact `Spellbook` method signatures for restoring a single level of spontaneous slots
- inspect a prepared caster example to compare with Oracle
- inspect how to log live `Blueprint` names and GUIDs for current resources from a real unit

At this point, though, the path is concrete enough that we can start designing settings and feature boundaries without guessing.

## Prepared Casters Research

We followed up on prepared casters to test the idea that they prepare individual spell instances into ordered slots.

### Concrete prepared spellbook examples

We inspected:

- `Classes/Wizard/WizardSpellbook.jbp`
- `Classes/Cleric/ClericSpellbook.jbp`

Both are `BlueprintSpellbook` assets with:

- `Spontaneous = false`

Wizard details:

- `CastingAttribute = Intelligence`
- `Spontaneous = false`
- `AllSpellsKnown = false`
- `SpellsPerLevel = 2`
- `CanCopyScrolls = true`

Cleric details:

- `CastingAttribute = Wisdom`
- `Spontaneous = false`
- `AllSpellsKnown = true`
- `SpellsPerLevel = 0`

So both Wizard and Cleric are clearly on the prepared side of the spellbook model, even though they differ in known-spell behavior.

### The important runtime type

The key type is:

- `Kingmaker.UnitLogic.SpellSlot`

This is the strongest evidence so far for how prepared spell restoration should work.

Important members on `SpellSlot`:

- `SpellLevel`
- `Type`
- `Index`
- `AbilityData SpellShell`
- `bool Available`
- `SpellSlot[] LinkedSlots`
- `bool IsOpposition`
- `void Spend()`
- `void Clear()`

### What this means

Prepared casting is not stored as only a number like "3 level-1 casts left."

Instead, it appears to be modeled as individual slot objects that:

- belong to a spell level
- have an index/order
- can hold a specific prepared spell instance in `SpellShell`
- separately track whether that prepared slot is still available

This is very important for design.

It means your intuition was correct:

- for prepared casters, restoring magic probably means restoring specific prepared slot entries, not just incrementing a generic count

## Design implications for prepared spell regeneration

### Your proposed idea fits the model

Your example:

- 4 level-1 prepared slots
- 1 Magic Missile
- 3 Grease
- ordered as `Grease, Grease, Grease, Magic Missile`

is consistent with the `SpellSlot` model.

Because each slot has:

- its own `Index`
- its own `SpellShell`
- its own `Available` flag

we can plausibly define restoration policies over real prepared slots.

### Candidate restoration policies

The simplest possible policy:

- restore the first spent slot in slot order

A more balanced policy like the one you suggested:

- walk in slot order
- prefer the first spent slot
- avoid restoring the same spell again immediately if another distinct prepared spell at that level is also empty
- then continue through the remaining spent copies

This now looks technically reasonable rather than speculative.

### Important caution

We still need to verify the best API for marking a spent prepared slot available again.

What we know already:

- `SpellSlot.Spend()` exists and sets `Available = false`
- a slot keeps its prepared spell in `SpellShell`

What we still need to verify:

- whether the correct way to restore one prepared cast is to flip slot availability through a spellbook helper method
- or whether `Spellbook` exposes a higher-level restore path we should prefer over touching `SpellSlot` directly

So the model is clear, but the safest write path still needs confirmation.

## Practical conclusion for spell settings

We now have a better split for future design:

For spontaneous casters like Oracle:

- settings by spell level make sense
- restoration probably means adding available casts/slots at that level

For prepared casters like Wizard or Cleric:

- settings by spell level also still make sense
- but implementation probably restores specific prepared slot entries inside that level

That means the user-facing setting can stay simple:

- "restore one level-1 slot every 30 seconds"

while the internal policy can differ by spellbook type:

- spontaneous: restore a free cast at that level
- prepared: restore one specific prepared slot at that level according to our chosen ordering rule

## Current best guess for prepared restoration policy

If we choose to support prepared casters, a reasonable first policy would be:

1. only consider spent slots at the selected spell level
2. process them in slot `Index` order
3. restore the first spent slot whose prepared spell differs from the last spell restored at that level, if possible
4. otherwise restore the first spent slot in order

That would roughly match the behavior you described and should be explainable in the UI.

If we adopt it later, we should definitely document it in a tooltip or help text.

## Spell Slot Restore Path Findings

We found a very important built-in component:

- `Kingmaker.UnitLogic.Abilities.Components.AbilityRestoreSpellSlot`

This is the clearest evidence so far for how Owlcat itself restores a single spell slot.

### What `AbilityRestoreSpellSlot` does

It requires:

- a target `Spellbook`
- a target spell level

Then it branches by spellbook type.

For spontaneous spellbooks:

- it checks `paramSpellbook.Blueprint.Spontaneous`
- reads:
  - `GetSpontaneousSlots(level)`
  - `GetSpellsPerDay(level)`
- restores one slot with:
  - `RestoreSpontaneousSlots(level, 1)`

For prepared spellbooks:

- it loops over `paramSpellbook.GetMemorizedSpellSlots(level)`
- it compares each slot's `SpellShell` to the selected parameter spell slot
- when it finds a spent matching slot, it restores it by:
  - setting `memorizedSpellSlot.Available = true`

This is huge because it gives us the game's own intended write path for one-slot restoration.

## What this means for our design

### Spontaneous casters

For classes like Oracle, the likely safe path is:

- find the spellbook
- pick the spell level
- call `RestoreSpontaneousSlots(level, 1)`

That matches our earlier research and confirms the user-facing idea of:

- "restore one level-1 slot every 30 seconds"

is a natural fit.

### Prepared casters

For prepared casters, Owlcat's own logic does not restore an abstract count.

Instead, it:

1. enumerates `GetMemorizedSpellSlots(level)`
2. identifies the specific prepared `SpellShell` it wants to restore
3. flips one matching spent slot back to `Available = true`

That means our prepared-caster policy really is about selecting which prepared slot entry to reactivate.

So your intuition was correct:

- for prepared spellbooks, restoring a specific prepared spell instance is the real model

## Built-in bulk restore clue

We also found:

- `Kingmaker.UnitLogic.Abilities.Components.ContextActionRestoreAllSpellSlots`

This component confirms the same split:

- for prepared spellbooks it loops over memorized slots and sets `Available = true`
- for spontaneous spellbooks it calls `RestoreSpontaneousSlots(...)`

It also clears:

- `tgtSpellbook.RemoveMemorizedSpells`

That is a useful hint that prepared-slot restoration may need to respect spellbook-side bookkeeping, not just the visible slot flag.

## Updated design conclusion

The mod probably should not decide behavior primarily by class name.

A better approach is:

1. inspect the unit's capabilities at runtime
2. identify the relevant subsystem
3. apply the correct restoration policy for that subsystem

Examples:

- if the unit has a spontaneous spellbook:
  - use spontaneous spell-slot restoration logic
- if the unit has a prepared spellbook:
  - use prepared slot-selection logic
- if the unit has generic `BlueprintAbilityResource` pools:
  - use generic resource restoration logic
- if the unit has a dedicated special subsystem like kineticist burn:
  - use that subsystem's dedicated API

### Where class-specific work still matters

This does not eliminate class-specific research.

We will still need class-by-class understanding for:

- unusual resources
- exceptions to generic patterns
- feature-specific UI names and settings
- special recharge rules

So the likely process is:

- detect subsystem type generically at runtime
- then layer class or feature-specific policy on top only where needed

That should scale better than hardcoding every class from scratch.

## Practical roadmap after Oracle and Paladin

After this proof of concept, a good research roadmap is:

1. survey each class for:
   - spellbook type
   - generic ability resources
   - obvious dedicated special subsystems
2. classify each class into:
   - spellbook only
   - generic resource only
   - mixed
   - special-case subsystem
3. only add bespoke logic where the generic models do not cover the feature well

At this point, the architecture feels much less mysterious:

- runtime capability detection first
- subsystem-specific restore policy second
- class-specific exceptions third

## First implementation slice

The first real resource-regeneration implementation now exists in code.

Scope:

- separate `Resource Regen` settings tab
- dedicated `ResourceRegenController`
- first strategy: spontaneous spellbook regeneration
- current target shape: Oracle-style spontaneous spellbooks
- current write path: `Spellbook.RestoreSpontaneousSlots(level, 1)`

Files:

- `src/Features/ResourceRegen/ResourceRegenController.cs`
- `src/Features/ResourceRegen/Strategies/SpontaneousSpellbookRegenStrategy.cs`
- `src/Features/ResourceRegen/Models/RegenTickContext.cs`

Current behavior:

- scans party members on a configurable controller tick
- skips while in combat if configured
- inspects each unit's spellbooks
- acts on:
  - spontaneous spellbooks by restoring one slot at a spell level
  - prepared spellbooks by restoring one spent prepared slot in slot order at a spell level
  - generic `BlueprintAbilityResource` pools by restoring one charge at a tiered interval based on the resource's runtime max amount
  - Kineticist burn by calling `HealBurn(1)` until a configured floor is reached
- tracks separate timers per:
  - strategy
  - unit
  - spellbook instance
  - spell level
- for prepared spellbooks, the current policy is intentionally simple:
  - choose the first spent prepared slot by slot index
  - set `Available = true`
  - remove that spell from `RemoveMemorizedSpells` if needed
- for generic ability resources, the current policy is:
  - map the resource's runtime max amount to a regeneration tier
  - tier 1: max uses `10+`
  - tier 2: max uses `8-9`
  - tier 3: max uses `6-7`
  - tier 4: max uses `4-5`
  - tier 5: max uses `2-3`
  - tier 6: max uses `1`
  - restore `1` charge when that tier's timer completes
- for Kineticist burn, the current low-risk policy is:
  - enable or disable the Kineticist strategy independently
  - wait for its own seconds-per-burn timer
  - heal `1` accepted burn with `UnitPartKineticist.HealBurn(1)`
  - stop once current burn is at or below the configured floor

Settings model:

- controller on/off
- out-of-combat only toggle
- spontaneous spellbook strategy on/off
- prepared spellbook strategy on/off
- generic ability-resource strategy on/off
- controller scan interval
- per-spell-level restore interval for levels 1 through 9
- setting a level interval to `0` disables regeneration for that spell level
- generic resource restore amount
- generic resource tier intervals stored separately from spell intervals, with defaults seeded to the same timing curve
- Kineticist burn strategy on/off
- Kineticist seconds per `1` burn restored
- Kineticist fixed burn floor

Logging model:

- `Info`:
  - controller startup
  - successful spell-slot restoration
- `Verbose`:
  - in-combat skips
  - failed restore attempt where the spellbook state did not change
- `Error`:
  - unexpected null descriptor state
  - per-strategy exceptions

This is intentionally still a narrow slice. Generic ability resources and special systems like kineticist burn should be added as separate strategies rather than folded into the spellbook strategies.
