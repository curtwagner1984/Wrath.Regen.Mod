# Class Research Docs

This folder is our class-by-class design notebook for resource regeneration.

We use it to answer:

- what runtime subsystems a class uses
- which archetypes change those subsystems
- what regeneration strategy fits that class or archetype
- what open questions still need verification before coding

## Layout

Recommended structure:

- `docs/<class>/base.md`
- `docs/<class>/<ArchetypeName>.md`

Examples:

- `docs/oracle/base.md`
- `docs/paladin/base.md`
- `docs/witch/base.md`
- `docs/witch/AccursedWitch.md`

## Strategy Categories

Use one or more of these labels in each file:

- `spellbook-spontaneous`
- `spellbook-prepared`
- `resource-generic`
- `special-subsystem`
- `mixed`

## Notes

- Prefer local blueprint and assembly evidence over memory.
- Record concrete blueprint file names when we find them.
- Keep implementation ideas separate from confirmed facts.
- If an archetype does not materially change the regeneration model, note that briefly instead of over-documenting it.
