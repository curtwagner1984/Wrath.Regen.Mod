# Martyr

## Snapshot

- Type: Paladin archetype
- Strategy category: `mixed`
- Confidence: medium

## Confirmed Facts

- Adds a bard-like performance resource on top of Paladin systems.

## Relevant Blueprints

- `Classes/Paladin/Archetypes/Martyr/MartyrArchetype.jbp`
- `Classes/Paladin/Archetypes/Martyr/Stigmata/MartyrPerformanceResource.jbp`
- `Classes/Paladin/Archetypes/Martyr/Stigmata/MartyrPerformanceResourceFact.jbp`

## Runtime Model

- Base Paladin prepared spellbook plus archetype-specific generic resource pool.

## Regeneration Strategy

- Reuse base Paladin spellbook logic.
- Restore Martyr performance resource separately from Lay on Hands and Smite Evil.
