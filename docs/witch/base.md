# Witch

## Snapshot

- Type: base class
- Strategy category: `spellbook`
- Confidence: medium

## Confirmed Facts

- Witch has a prepared arcane spellbook.
- Base Witch is mostly spellbook-driven.
- Some optional hex features add generic resource pools.

## Relevant Blueprints

- `Classes/Witch/WitchClass.jbp`
- `Classes/Witch/WitchSpellbook.jbp`
- `Classes/Witch/WitchHexes/...*Resource.jbp`

## Runtime Model

- `WitchSpellbook.jbp`:
  - `Spontaneous = false`
  - `CastingAttribute = Intelligence`
  - `CanCopyScrolls = true`
- Base handling should follow prepared spellbook logic.
- Optional hex-related resources should be treated as add-on generic pools.

## Regeneration Strategy

- Use prepared spellbook slot restoration as the primary model.
- Restore hex/resource pools only if the unit actually owns those facts/resources.

## Archetype Notes

- High-priority archetypes:
  - `AccursedWitch`
  - `LeyLineGuardianWitch`
  - `HexChannelerWitch`
  - `CauldronWitch`
  - `DarkSisterWitch`
  - `WitchOfTheVeil`

## Open Questions

- Which Witch archetypes replace the spellbook?
- Which Witch archetypes add special resources or other unique recharge mechanics?
