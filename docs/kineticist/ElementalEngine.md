# ElementalEngine

## Snapshot

- Type: Kineticist archetype
- Strategy category: `special-subsystem`
- Confidence: high

## Confirmed Facts

- Adds explicit burnout mechanics.
- `AddKineticistElementalOverflow` has an `ElementalEngine` path that changes bonus behavior when `MaxBurn == AcceptedBurn`.

## Relevant Blueprints

- `Classes/Kineticist/Archetypes/ElementalEngine/ElementalEngineArchetype.jbp`
- `Classes/Kineticist/Archetypes/ElementalEngine/ElementalEngineBurnoutFeature.jbp`
- `Classes/Kineticist/Archetypes/ElementalEngine/ElementalEngineBurnoutTriggerFeature.jbp`
- `Classes/Kineticist/Archetypes/ElementalEngine/ElementalEngineBurnoutSuperchargeBuff.jbp`

## Runtime Model

- This archetype explicitly rewards being at maximum burn.

## Regeneration Strategy

- A generic `Keep burn for max bonuses` mode is not sufficient here.
- Elemental Engine likely needs an archetype-specific option such as:
  - do not heal below maximum burn while burnout bonuses are desired
  - or a dedicated `preserve Elemental Engine burnout` toggle

## Archetype Notes

- This is the clearest example where a naive smart-floor mode could be wrong.

## Open Questions

- Which Elemental Engine bonuses are lost immediately when burn drops below maximum?
