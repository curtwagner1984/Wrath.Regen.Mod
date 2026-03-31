using System.Runtime.CompilerServices;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;

namespace WrathRegenMod;

internal static class ResourceRegenHelpers
{
    public static string CreateTimerKey(string strategyName, UnitEntityData unit, object spellbookIdentity, int spellLevel)
    {
        return string.Concat(
            strategyName,
            ":",
            unit.UniqueId,
            ":",
            RuntimeHelpers.GetHashCode(spellbookIdentity).ToString(),
            ":",
            spellLevel.ToString());
    }

    public static string GetUnitName(UnitEntityData unit)
    {
        return string.IsNullOrWhiteSpace(unit?.CharacterName) ? "<unnamed>" : unit.CharacterName;
    }

    public static string GetPreparedSpellName(SpellSlot slot)
    {
        var spellName = slot?.SpellShell?.Name;
        return string.IsNullOrWhiteSpace(spellName) ? "<empty>" : spellName;
    }
}
