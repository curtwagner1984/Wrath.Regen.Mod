# Paladin

## Snapshot

- Type: base class
- Strategy category: `mixed`
- Confidence: high

## Confirmed Facts

- Paladin has a spellbook and generic class resources.
- Lay on Hands and Smite Evil are resource-backed.
- Paladin spellcasting is prepared, not spontaneous.

## Relevant Blueprints

- `Classes/Paladin/PaladinClass.jbp`
- `Classes/Paladin/PaladinSpellbook.jbp`
- `Classes/Paladin/LayOnHands/LayOnHandsFeature.jbp`
- `Classes/Paladin/LayOnHands/LayonHandsFact.jbp`
- `Classes/Paladin/LayOnHands/LayOnHandsResource.jbp`
- `Classes/Paladin/SmiteEvil/SmiteEvilFeature.jbp`
- `Classes/Paladin/SmiteEvil/SmiteEvilAbility.jbp`
- `Classes/Paladin/SmiteEvil/SmiteEvilResource.jbp`

## Runtime Model

- Spellbook path:
  - `PaladinSpellbook.jbp`
  - `Spontaneous = false`
  - `AllSpellsKnown = true`
  - `CastingAttribute = Charisma`
  - `CasterLevelModifier = -3`
- Generic resource path:
  - `UnitDescriptor.Resources`
- Lay on Hands:
  - feature grants fact and abilities
  - fact grants resource pool
- Smite Evil:
  - feature grants ability and resource pool directly

## Regeneration Strategy

- Spellbook logic for Paladin spells.
- Generic resource restoration for Lay on Hands and Smite Evil.

## Archetype Notes

- Material archetypes to track:
  - `TorturedCrusader`
  - `Martyr`
  - `Hospitaler`
  - `Stonelord`
  - `WarriorOfTheHolyLight`
- Several archetypes add additional resource pools without changing the overall mixed model.

## Open Questions

- Which archetypes add or replace meaningful resource systems?
- Which Paladin subsystems should get separate user-facing settings?
