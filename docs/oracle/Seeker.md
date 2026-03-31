# Seeker

## Snapshot

- Type: Oracle archetype
- Strategy category: `spellbook-spontaneous`, `feature-review`
- Confidence: medium

## Confirmed Facts

- Keeps the base Oracle spontaneous spellbook.
- Does not replace the spellbook.
- No obvious archetype-specific `*Resource.jbp` pool was found in this first pass.

## Relevant Blueprints

- `Classes/Oracle/Archetypes/Seeker/SeekerArchetype.jbp`
- `Classes/Oracle/Archetypes/Seeker/SeekerFeatSelection.jbp`
- `Classes/Oracle/Archetypes/Seeker/SeekerTrapfinding.jbp`

## Runtime Model

- Base Oracle spontaneous spell-slot logic still applies.
- Archetype changes found so far are feature-selection oriented, not obviously resource-pool oriented.

## Regeneration Strategy

- Reuse base Oracle spontaneous slot logic.
- Only add special archetype handling later if deeper feature inspection reveals a dedicated resource subsystem.

## Open Questions

- Are there Seeker-specific abilities later in progression that introduce hidden resources not visible from the first-pass archetype files?
