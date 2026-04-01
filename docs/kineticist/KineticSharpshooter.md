# KineticSharpshooter

## Snapshot

- Type: Kineticist archetype
- Strategy category: `special-subsystem`
- Confidence: medium

## Confirmed Facts

- Has archetype-specific burn resources.

## Relevant Blueprints

- `Classes/Kineticist/Archetypes/KineticSharpshooter/KineticSharpshooterArchetype.jbp`
- `Classes/Kineticist/Archetypes/KineticSharpshooter/Resources/KineticSharpshooterBurnResource.jbp`
- `Classes/Kineticist/Archetypes/KineticSharpshooter/Features/KineticSharpshootterMainMechanicalHiddenFeature.jbp`

## Runtime Model

- Burn handling appears customized enough that it should not be assumed identical to base Kineticist without verification.

## Regeneration Strategy

- Support fixed burn-floor mode first.
- Verify smart-floor inference separately for this archetype.

## Archetype Notes

- This is another likely special case for burn/resource behavior.

## Open Questions

- Does KineticSharpshooter reinterpret "useful burn" in a way that differs from Elemental Overflow style bonuses?
