# PossessedOracle

## Snapshot

- Type: Oracle archetype
- Strategy category: `spellbook-spontaneous`, `feature-review`
- Confidence: medium

## Confirmed Facts

- Keeps the base Oracle spontaneous spellbook.
- Does not replace the spellbook.
- No obvious archetype-specific `*Resource.jbp` pool was found in this first pass.

## Relevant Blueprints

- `Classes/Oracle/Archetypes/PossessedOracle/PossessedOracleArchetype.jbp`
- `Classes/Oracle/Archetypes/PossessedOracle/PossessedOracleTwoMindsFeature.jbp`

## Runtime Model

- Base Oracle spontaneous spell-slot logic still applies.
- Archetype changes appear feature-oriented rather than driven by a new explicit resource pool.

## Regeneration Strategy

- Reuse base Oracle spontaneous slot logic.
- Start with no special archetype-only resource handling unless later evidence shows hidden resource usage.

## Open Questions

- Does Possessed Oracle introduce any runtime resource pools through linked facts deeper in the feature tree?
