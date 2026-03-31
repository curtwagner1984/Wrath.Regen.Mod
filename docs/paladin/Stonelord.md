# Stonelord

## Snapshot

- Type: Paladin archetype
- Strategy category: `mixed`, `special-subsystem`
- Confidence: medium

## Confirmed Facts

- Adds nonstandard Paladin resource behavior.
- Defensive stance and related resources likely deserve separate policy.

## Relevant Blueprints

- `Classes/Paladin/Archetypes/Stonelord/StonelordArchetype.jbp`
- `Classes/Paladin/Archetypes/Stonelord/StonestrikeResource.jbp`
- `Classes/Paladin/Archetypes/Stonelord/DefensiveStance/StonelordDefensiveStanceResource.jbp`

## Runtime Model

- Base Paladin shell with extra archetype resource pools.
- Defensive stance behavior may want special handling beyond generic restoration.

## Regeneration Strategy

- Start with generic resource restoration.
- Re-evaluate after verifying whether stance pacing should differ from simple pool recharge.
