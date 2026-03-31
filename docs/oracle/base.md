# Oracle

## Snapshot

- Type: base class
- Strategy category: `mixed`
- Confidence: high

## Confirmed Facts

- Oracle uses a spontaneous spellbook.
- Oracle casting is Charisma-based.
- Oracle is not spellbook-only.
- Oracle revelations and mysteries add many generic resource pools.

## Relevant Blueprints

- `Classes/Oracle/OracleClass.jbp`
- `Classes/Oracle/OracleSpellbook.jbp`
- `Classes/Oracle/OracleProgression.jbp`
- `Classes/Oracle/OracleRevelations/...*Resource.jbp`
- `Classes/Oracle/OracleMysteries/...*Resource.jbp`

## Runtime Model

- Spellbook path:
  - `UnitDescriptor.Spellbooks`
  - `UnitDescriptor.GetSpellbook(...)`
- Generic resource path:
  - `UnitDescriptor.Resources`
- `OracleSpellbook.jbp`:
  - `Spontaneous = true`
  - `CastingAttribute = Charisma`
  - includes both `m_SpellsPerDay` and `m_SpellsKnown`

## Regeneration Strategy

- Use spontaneous spell-slot restoration by spell level.
- Also restore generic `BlueprintAbilityResource` pools for revelations and mystery powers.

## Archetype Notes

- No custom Oracle archetype spellbook files were found in this pass.
- Important archetypes still add archetype-specific resource pools.
- Oracle archetypes confirmed:
  - `DivineHerbalist`
  - `DualCursedOracle`
  - `EnlightenedPhilosopher`
  - `Hermit`
  - `PossessedOracle`
  - `PurifierAasimar`
  - `Seeker`
  - `WindWhisperer`
- Highest-value archetypes to track:
  - `DivineHerbalist`
  - `DualCursedOracle`
  - `Hermit`
  - `PossessedOracle`
  - `PurifierAasimar`
  - `Seeker`
  - `WindWhisperer`
  - `EnlightenedPhilosopher`

## Open Questions

- Which Oracle resource pools should be exposed as configurable user-facing targets?
- Which archetypes meaningfully alter recharge policy, rather than just adding more generic pools?
