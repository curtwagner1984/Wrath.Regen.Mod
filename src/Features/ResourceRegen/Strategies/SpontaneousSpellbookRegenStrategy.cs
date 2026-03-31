using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;

namespace WrathRegenMod;

internal sealed class SpontaneousSpellbookRegenStrategy : IResourceRegenStrategy
{
    private readonly Dictionary<string, float> elapsedByKey = new Dictionary<string, float>(StringComparer.Ordinal);

    public string Name => "SpontaneousSpellbookRegen";

    public void Tick(UnitEntityData unit, RegenTickContext context)
    {
        if (!context.Settings.ResourceRegen.EnableSpontaneousSpellbookRegen)
        {
            return;
        }

        if (unit == null || unit.Descriptor == null)
        {
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

    private void TickSpellbook(UnitEntityData unit, Spellbook spellbook, RegenTickContext context)
    {
        for (var spellLevel = 1; spellLevel <= 9; spellLevel++)
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
                ClearElapsed(unit, spellbook, spellLevel);
                continue;
            }

            var timerKey = CreateTimerKey(unit, spellbook, spellLevel);
            elapsedByKey.TryGetValue(timerKey, out var elapsedSeconds);
            elapsedSeconds += context.ElapsedSeconds;

            if (elapsedSeconds < intervalSeconds)
            {
                elapsedByKey[timerKey] = elapsedSeconds;
                continue;
            }

            var beforeRestore = availableSlots;
            spellbook.RestoreSpontaneousSlots(spellLevel, 1);
            var afterRestore = spellbook.GetSpontaneousSlots(spellLevel);
            var restoredSlots = Math.Max(0, afterRestore - beforeRestore);

            if (restoredSlots <= 0)
            {
                context.Logger.Verbose($"{Name} tried to restore a level {spellLevel} slot for {GetUnitName(unit)}, but the spellbook state did not change.");
                elapsedByKey[timerKey] = 0f;
                continue;
            }

            context.Logger.Info(
                $"{Name} restored {restoredSlots} level {spellLevel} slot for {GetUnitName(unit)} ({beforeRestore}/{maxSlots} -> {afterRestore}/{maxSlots}).");
            elapsedByKey[timerKey] = 0f;
        }
    }

    private void ClearElapsed(UnitEntityData unit, Spellbook spellbook, int spellLevel)
    {
        elapsedByKey.Remove(CreateTimerKey(unit, spellbook, spellLevel));
    }

    private static string CreateTimerKey(UnitEntityData unit, Spellbook spellbook, int spellLevel)
    {
        return string.Concat(unit.UniqueId, ":", RuntimeHelpers.GetHashCode(spellbook).ToString(), ":", spellLevel.ToString());
    }

    private static string GetUnitName(UnitEntityData unit)
    {
        return string.IsNullOrWhiteSpace(unit.CharacterName) ? "<unnamed>" : unit.CharacterName;
    }
}
