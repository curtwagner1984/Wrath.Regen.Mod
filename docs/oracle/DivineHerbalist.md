# DivineHerbalist

## Snapshot

- Type: Oracle archetype
- Strategy category: `mixed`
- Confidence: medium

## Confirmed Facts

- Keeps the Oracle spontaneous spellbook model.
- Adds archetype-specific healing resource behavior.

## Relevant Blueprints

- `Classes/Oracle/Archetypes/DivineHerbalist/DivineHerbalistArchetype.jbp`
- `Classes/Oracle/Archetypes/DivineHerbalist/HealersWayResource.jbp`

## Runtime Model

- Base Oracle spontaneous spellbook handling still applies.
- Additional archetype resource pool should be handled through generic resource restoration.

## Regeneration Strategy

- Reuse base Oracle spell-slot logic.
- Add optional regeneration for `HealersWayResource`.

## Open Questions

- Does `HealersWayResource` need distinct pacing from normal Oracle revelation resources?
