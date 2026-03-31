# DualCursedOracle

## Snapshot

- Type: Oracle archetype
- Strategy category: `spellbook-spontaneous`, `feature-review`
- Confidence: medium

## Confirmed Facts

- Keeps the base Oracle spontaneous spellbook.
- Does not replace the spellbook.
- No obvious archetype-specific `*Resource.jbp` pool was found in this first pass.

## Relevant Blueprints

- `Classes/Oracle/Archetypes/DualCursedOracle/DualCursedOracleArchetype.jbp`
- `Classes/Oracle/Archetypes/DualCursedOracle/AdditionalRevelationsFeature.jbp`
- `Classes/Oracle/Archetypes/DualCursedOracle/DualCursedOracleBonusSpellFeature.jbp`
- `Classes/Oracle/Archetypes/DualCursedOracle/FortuneRevelationFeature.jbp`
- `Classes/Oracle/Archetypes/DualCursedOracle/FortuneCooldownBuff.jbp`
- `Classes/Oracle/Archetypes/DualCursedOracle/MisfortuneRevelationFeature.jbp`
- `Classes/Oracle/Archetypes/DualCursedOracle/MisfortuneCooldownBuff.jbp`
- `Classes/Oracle/Archetypes/DualCursedOracle/SecondCurseSelection.jbp`

## Runtime Model

- Base Oracle spontaneous spell-slot logic still applies.
- The notable archetype additions found so far look cooldown-buff driven rather than obviously resource-pool driven.

## Regeneration Strategy

- Reuse base Oracle spontaneous slot logic.
- Do not assume there is a dedicated archetype resource pool.
- Review fortune/misfortune style features individually if we want to support them, because they may be cooldown-based rather than `BlueprintAbilityResource`-based.

## Open Questions

- Are Fortune and Misfortune controlled purely by cooldown buffs, or is there a hidden resource path we have not mapped yet?
- Which Dual Cursed Oracle features are worth exposing in the mod at all, versus leaving as cooldown-only gameplay?
