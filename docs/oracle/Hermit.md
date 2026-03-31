# Hermit

## Snapshot

- Type: Oracle archetype
- Strategy category: `mixed`
- Confidence: medium

## Confirmed Facts

- Keeps the Oracle spontaneous spellbook model.
- Adds multiple archetype-specific revelation resources.

## Relevant Blueprints

- `Classes/Oracle/Archetypes/Hermit/HermitArchetype.jbp`
- `Classes/Oracle/Archetypes/Hermit/HermitRevelations/HermitRevelationFadeFromMemoryResource.jbp`
- `Classes/Oracle/Archetypes/Hermit/HermitRevelations/HermitRevelationReclusesStride10Resource.jbp`

## Runtime Model

- Base Oracle spellbook logic plus additional generic resource pools.

## Regeneration Strategy

- Reuse base Oracle spontaneous slot logic.
- Layer archetype resource restoration on top.

## Open Questions

- Which Hermit resources are important enough to expose directly in settings?
