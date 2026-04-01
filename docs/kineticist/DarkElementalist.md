# DarkElementalist

## Snapshot

- Type: Kineticist archetype
- Strategy category: `special-subsystem`
- Confidence: medium

## Confirmed Facts

- Has archetype-specific burn resources and overflow progression.

## Relevant Blueprints

- `Classes/Kineticist/Archetypes/DarkElementalist/DarkElementalistArchetype.jbp`
- `Classes/Kineticist/Archetypes/DarkElementalist/DarkElementalistBurn/DarkElementalistBurnFeature.jbp`
- `Classes/Kineticist/Archetypes/DarkElementalist/DarkElementalistBurn/DarkElementalistBurnResource.jbp`
- `Classes/Kineticist/Archetypes/DarkElementalist/DarkElementalistElementalOverflowProgression.jbp`
- `Classes/Kineticist/Archetypes/DarkElementalist/DarkElementalistElementalOverflowBonusFeature.jbp`

## Runtime Model

- Burn/overflow behavior is materially customized compared to base Kineticist.

## Regeneration Strategy

- Do not assume base Kineticist smart-floor inference is correct here.
- Support fixed burn-floor regeneration first.

## Archetype Notes

- This is one of the strongest candidates for archetype-specific burn logic.

## Open Questions

- How does Dark Elementalist's custom overflow progression change the "max useful burn" threshold?
