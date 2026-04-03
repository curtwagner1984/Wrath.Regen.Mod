using System;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;

namespace WrathRegenMod;

internal sealed class SpontaneousSpellbookRegenStrategy : IResourceRegenStrategy
{
    private readonly Dictionary<(Spellbook, int), float> elapsedByKey = new();

    public string Name => "SpontaneousSpellbookRegen";

    public void Tick(UnitEntityData unit, RegenTickContext context)
    {
        if (!context.Settings.ResourceRegen.EnableSpontaneousSpellbookRegen)
        {
            return;
        }

        if (unit == null || unit.Descriptor == null)
        {
            if (context.Logger.IsError)
                context.Logger.Error($"{Name} encountered a unit with a missing descriptor.");
            return;
        }

        var spellbooks = unit.Descriptor.Spellbooks;
        if (spellbooks == null)
        {
            return;
        }

        foreach (var spellbook in spellbooks)
        {
            if (spellbook == null || spellbook.Blueprint == null || !spellbook.Blueprint.Spontaneous)
            {
                continue;
            }

            TickSpellbook(unit, spellbook, context);
        }
    }

    public void Reset()
    {
        elapsedByKey.Clear();
    }

    private void TickSpellbook(UnitEntityData unit, Spellbook spellbook, RegenTickContext context)
    {
        for (var spellLevel = 1; spellLevel <= spellbook.Blueprint.MaxSpellLevel; spellLevel++)
        {
            var intervalSeconds = context.Settings.ResourceRegen.GetIntervalSecondsForSpellLevel(spellLevel);
            if (intervalSeconds <= 0f)
            {
                continue;
            }

            var maxSlots = spellbook.GetSpellsPerDay(spellLevel);
            if (maxSlots <= 0)
            {
                continue;
            }

            var availableSlots = spellbook.GetSpontaneousSlots(spellLevel);
            if (availableSlots >= maxSlots)
            {
                elapsedByKey.Remove((spellbook, spellLevel));
                continue;
            }

            var key = (spellbook, spellLevel);
            elapsedByKey.TryGetValue(key, out var elapsedSeconds);
            elapsedSeconds += context.ElapsedSeconds;

            if (elapsedSeconds < intervalSeconds)
            {
                elapsedByKey[key] = elapsedSeconds;
                continue;
            }

            var beforeRestore = availableSlots;
            spellbook.RestoreSpontaneousSlots(spellLevel, 1);
            var afterRestore = spellbook.GetSpontaneousSlots(spellLevel);
            var restoredSlots = Math.Max(0, afterRestore - beforeRestore);

            if (restoredSlots <= 0)
            {
                if (context.Logger.IsVerbose)
                    context.Logger.Verbose($"{Name} tried to restore a level {spellLevel} slot for {ResourceRegenHelpers.GetUnitName(unit)}, but the spellbook state did not change.");
                elapsedByKey[key] = 0f;
                continue;
            }

            ResourceRegenFxPlayer.TryPlayOnUnit(context.Logger, context.Settings, unit);
            if (context.Logger.IsInfo)
                context.Logger.Info(
                    $"{Name} restored {restoredSlots} level {spellLevel} slot for {ResourceRegenHelpers.GetUnitName(unit)} ({beforeRestore}/{maxSlots} -> {afterRestore}/{maxSlots}).");
            elapsedByKey[key] = 0f;
        }
    }
}
