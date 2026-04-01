# Kineticist

## Snapshot

- Type: base class
- Strategy category: `special-subsystem`
- Confidence: high

## Confirmed Facts

- Kineticist burn is not a normal `BlueprintAbilityResource` pool.
- Burn lives on `Kingmaker.UnitLogic.Class.Kineticist.UnitPartKineticist`.
- Burn restoration should use `UnitPartKineticist.HealBurn(int value)`.
- Burn-based bonuses are tied to overflow-style facts, not just the raw burn value alone.

## Relevant Blueprints

- `Classes/Kineticist/KineticistClass.jbp`
- `Classes/Kineticist/KineticistBurn.jbp`
- `Classes/Kineticist/Burn/BurnResource.jbp`
- `Classes/Kineticist/Burn/BurnPerRoundResource.jbp`
- `Classes/Kineticist/ElementalOverflow/ElementalOverflowFeature.jbp`
- `Classes/Kineticist/ElementalOverflow/ElementalOverflowProgression.jbp`
- `Classes/Kineticist/GatherPower/GatherPowerFeature.jbp`
- `Classes/Kineticist/Infusions/InfusionSpecializationProgression.jbp`

## Runtime Model

- Main runtime type:
  - `Kingmaker.UnitLogic.Class.Kineticist.UnitPartKineticist`
- Important members:
  - `AcceptedBurn`
  - `AcceptedBurnThisRound`
  - `MaxBurn`
  - `LeftBurn`
  - `LeftBurnThisRound`
  - `AcceptBurn(int burn, AbilityData ability)`
  - `HealBurn(int value)`
  - `ClearAcceptedBurn()`
- `MaxBurn` and `MaxBurnPerRound` are themselves backed by resources in `Owner.Resources`, but the actual accepted burn state is stored in the unit part.

## Burn Bonus Model

- The clearest bonus component found is:
  - `Kingmaker.UnitLogic.Class.Kineticist.AddKineticistElementalOverflow`
- Its effective bonus uses:
  - `Math.Min(Bonus.Calculate(context), unitPartKineticist.ClassLevel / 3)`
  - then, unless `IgnoreBurn` is set, it caps again at `AcceptedBurn`
- Practical reading:
  - current burn matters
  - class level matters
  - the active overflow fact's configured `Bonus` matters
- This means there is no obvious single runtime property on `UnitPartKineticist` that simply says "current max useful burn threshold".

## Regeneration Strategy

- Burn should be handled by a dedicated Kineticist strategy, not the generic resource engine.
- Baseline settings should be:
  - enable/disable burn regeneration
  - seconds per `1` burn healed
  - restore until fixed floor
- A smarter mode such as `Keep burn for max bonuses` looks possible, but should be treated as inferred behavior rather than a simple built-in threshold read.

## Archetype Notes

- Material archetypes to track:
  - `DarkElementalist`
  - `ElementalEngine`
  - `KineticSharpshooter`
  - `OverwhelmingSoul`
  - `Psychokineticist`
- `BloodKineticist` and `KineticKnight` look less likely to replace the burn model completely, but still need verification before assuming they are identical to base Kineticist.

## Open Questions

- What is the safest runtime way to infer the highest burn value that still preserves overflow-style bonuses?
- Should `Keep burn for max bonuses` be limited to base Kineticist and close variants instead of applying to every archetype?
- Which archetypes replace the effective burn-benefit model enough to require bespoke logic?
