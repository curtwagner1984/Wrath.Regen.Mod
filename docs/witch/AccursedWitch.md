# AccursedWitch

## Snapshot

- Type: Witch archetype
- Strategy category: `spellbook-spontaneous`
- Confidence: medium

## Confirmed Facts

- Uses a custom spontaneous spellbook.
- This changes the restore model from prepared to spontaneous.

## Relevant Blueprints

- `Classes/Witch/Archetypes/AccursedWitch/AccursedWitchArchetype.jbp`
- `Classes/Witch/Archetypes/AccursedWitch/AccursedWitchSpellbook.jbp`

## Runtime Model

- `AccursedWitchSpellbook.jbp`:
  - `Spontaneous = true`
  - `CastingAttribute = Charisma`
  - `CanCopyScrolls = false`

## Regeneration Strategy

- Use spontaneous spell-slot restoration, not base Witch prepared-slot logic.
