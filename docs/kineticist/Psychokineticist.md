# Psychokineticist

## Snapshot

- Type: Kineticist archetype
- Strategy category: `special-subsystem`
- Confidence: medium

## Confirmed Facts

- Has archetype-specific burn resources and custom overflow progression.
- Uses `MentalOverflow` blueprints instead of the base `ElementalOverflow` structure.

## Relevant Blueprints

- `Classes/Kineticist/Archetypes/Psychokineticist/PsychokineticistArchetype.jbp`
- `Classes/Kineticist/Archetypes/Psychokineticist/PsychokineticistBurnFeature.jbp`
- `Classes/Kineticist/Archetypes/Psychokineticist/PsychokineticistBurnResource.jbp`
- `Classes/Kineticist/Archetypes/Psychokineticist/PsychokineticistMindBurn.jbp`
- `Classes/Kineticist/Archetypes/Psychokineticist/MentalOverflow/MentalOverflowProgression.jbp`

## Runtime Model

- This archetype has a materially different overflow package compared to base Kineticist.

## Regeneration Strategy

- Do not assume base Elemental Overflow thresholds apply directly.
- Support fixed burn-floor regeneration first.

## Archetype Notes

- Psychokineticist is one of the best candidates for bespoke smart-floor logic if we later implement automatic bonus preservation.

## Open Questions

- How should `Mind Burn` and `Mental Overflow` affect the inferred burn floor?
