# TorturedCrusader

## Snapshot

- Type: Paladin archetype
- Strategy category: `mixed`
- Confidence: medium

## Confirmed Facts

- Uses a custom spellbook.
- Adds custom resource pools on top of Paladin systems.

## Relevant Blueprints

- `Classes/Paladin/Archetypes/TorturedCrusader/TorturedCrusader.jbp`
- `Classes/Paladin/Archetypes/TorturedCrusader/PaladinSpellbookTorturedCrusader.jbp`
- `Classes/Paladin/Archetypes/TorturedCrusader/AllIsDarkness/AllIsDarknessResource.jbp`
- `Classes/Paladin/Archetypes/TorturedCrusader/AloneInTheDark/TorturedCrusaderLayOnHandsResource.jbp`

## Runtime Model

- Prepared spellbook still applies, but with custom spellbook config.
- Also has archetype-specific generic resources.

## Regeneration Strategy

- Handle via prepared spellbook engine.
- Add separate resource restoration for TorturedCrusader-specific pools.
