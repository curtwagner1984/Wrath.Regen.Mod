using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Root.Fx;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.Visual.Particles;
using UnityEngine;

namespace WrathRegenMod;

internal static class ResourceRegenFxPlayer
{
    public static void TryPlayOnUnit(ModLogger logger, ModSettings settings, UnitEntityData unit)
    {
        if (!settings.ResourceRegen.ShowVisualEffects || unit?.View == null)
        {
            return;
        }

        var prefab = GetPrefab(settings.ResourceRegen.VisualEffectStyle);
        if (prefab == null)
        {
            logger.Verbose("Resource regen visual effect was enabled, but no built-in prefab was available for the selected style.");
            return;
        }

        FxHelper.SpawnFxOnUnit(prefab, unit.View, unit.IsPlayerFaction);
    }

    private static GameObject GetPrefab(ResourceRegenVisualEffectStyle style)
    {
        if (BlueprintRoot.Instance?.FxRoot == null)
        {
            return null;
        }

        switch (style)
        {
            case ResourceRegenVisualEffectStyle.ArcaneRefresh:
                return BlueprintRoot.Instance.FxRoot.GetCast(SpellSource.Arcane, CastSource.SingleHand);
            case ResourceRegenVisualEffectStyle.DivineRefresh:
            default:
                return BlueprintRoot.Instance.FxRoot.GetCast(SpellSource.Divine, CastSource.SingleHand);
        }
    }
}
