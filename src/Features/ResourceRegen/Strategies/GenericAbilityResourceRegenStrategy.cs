using System;
using System.Collections.Generic;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;

namespace WrathRegenMod;

internal sealed class GenericAbilityResourceRegenStrategy : IResourceRegenStrategy
{
    private readonly Dictionary<(UnitEntityData, BlueprintAbilityResource, int), float> elapsedByKey = new();

    public string Name => "GenericAbilityResourceRegen";

    public void Tick(UnitEntityData unit, RegenTickContext context)
    {
        if (!context.Settings.ResourceRegen.EnableGenericAbilityResourceRegen)
        {
            return;
        }

        if (unit == null || unit.Descriptor?.Resources == null)
        {
            context.Logger.Error($"{Name} encountered a unit with missing resource state.");
            return;
        }

        foreach (BlueprintScriptableObject blueprint in unit.Descriptor.Resources)
        {
            if (!(blueprint is BlueprintAbilityResource abilityResource))
            {
                if (context.Logger.IsVerbose)
                    context.Logger.Verbose($"{Name} skipped non-standard resource {ResourceRegenHelpers.GetResourceName(blueprint)} on {ResourceRegenHelpers.GetUnitName(unit)}.");
                continue;
            }

            TickResource(unit, abilityResource, context);
        }
    }

    public void Reset()
    {
        elapsedByKey.Clear();
    }

    private void TickResource(UnitEntityData unit, BlueprintAbilityResource resource, RegenTickContext context)
    {
        var currentAmount = unit.Descriptor.Resources.GetResourceAmount(resource);
        var maxAmount = resource.GetMaxAmount(unit.Descriptor);
        if (maxAmount <= 0)
        {
            return;
        }

        if (currentAmount >= maxAmount)
        {
            ClearElapsed(unit, resource);
            return;
        }

        var resourceTier = context.Settings.ResourceRegen.GetGenericResourceTier(maxAmount);
        var intervalSeconds = context.Settings.ResourceRegen.GetIntervalSecondsForGenericResourceMax(maxAmount);
        if (resourceTier <= 0 || intervalSeconds <= 0f)
        {
            if (context.Logger.IsVerbose)
                context.Logger.Verbose($"{Name} disabled regeneration for {ResourceRegenHelpers.GetResourceName(resource)} on {ResourceRegenHelpers.GetUnitName(unit)} because its tier is disabled.");
            ClearElapsed(unit, resource);
            return;
        }

        var key = (unit, resource, resourceTier);
        elapsedByKey.TryGetValue(key, out var elapsedSeconds);
        elapsedSeconds += context.ElapsedSeconds;

        if (elapsedSeconds < intervalSeconds)
        {
            elapsedByKey[key] = elapsedSeconds;
            if (context.Logger.IsVerbose)
                context.Logger.Verbose(
                    $"{Name} is waiting for {ResourceRegenHelpers.GetResourceName(resource)} on {ResourceRegenHelpers.GetUnitName(unit)} (tier {resourceTier}, max {maxAmount}, {elapsedSeconds:0.##}/{intervalSeconds:0.##} seconds, {currentAmount}/{maxAmount}).");
            return;
        }

        var beforeAmount = currentAmount;
        unit.Descriptor.Resources.Restore(resource, context.Settings.ResourceRegen.GenericResourceRestoreAmount);
        var afterAmount = unit.Descriptor.Resources.GetResourceAmount(resource);
        var restoredAmount = Math.Max(0, afterAmount - beforeAmount);

        if (restoredAmount <= 0)
        {
            if (context.Logger.IsVerbose)
                context.Logger.Verbose(
                    $"{Name} tried to restore {ResourceRegenHelpers.GetResourceName(resource)} for {ResourceRegenHelpers.GetUnitName(unit)} (tier {resourceTier}, max {maxAmount}), but the resource state did not change.");
            elapsedByKey[key] = 0f;
            return;
        }

        ResourceRegenFxPlayer.TryPlayOnUnit(context.Logger, context.Settings, unit);
        if (context.Logger.IsInfo)
            context.Logger.Info(
                $"{Name} restored {restoredAmount} charge(s) to {ResourceRegenHelpers.GetResourceName(resource)} for {ResourceRegenHelpers.GetUnitName(unit)} (tier {resourceTier}, {beforeAmount}/{maxAmount} -> {afterAmount}/{maxAmount}).");
        elapsedByKey[key] = 0f;
    }

    private void ClearElapsed(UnitEntityData unit, BlueprintAbilityResource resource)
    {
        for (var tier = 1; tier <= 6; tier++)
        {
            elapsedByKey.Remove((unit, resource, tier));
        }
    }
}
