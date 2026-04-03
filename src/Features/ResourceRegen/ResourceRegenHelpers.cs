using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.Kineticist;

namespace WrathRegenMod;

internal static class ResourceRegenHelpers
{
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

    public static string GetKineticistDisplayName(UnitPartKineticist kineticistPart)
    {
        if (kineticistPart?.Owner?.Unit == null)
        {
            return "<unnamed>";
        }

        return GetUnitName(kineticistPart.Owner.Unit);
    }
}
