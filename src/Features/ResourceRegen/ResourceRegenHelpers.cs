using System.Runtime.CompilerServices;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;

namespace WrathRegenMod;

internal static class ResourceRegenHelpers
{
    public static string CreateTimerKey(string strategyName, UnitEntityData unit, object identity, int subKey)
    {
        return string.Concat(
            strategyName,
            ":",
            unit.UniqueId,
            ":",
            RuntimeHelpers.GetHashCode(identity).ToString(),
            ":",
            subKey.ToString());
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

    public static string GetResourceName(BlueprintScriptableObject blueprint)
    {
        return string.IsNullOrWhiteSpace(blueprint?.name) ? "<unnamed-resource>" : blueprint.name;
    }
}
