using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;

namespace WrathRegenMod;

internal sealed class PreparedSpellbookRegenStrategy : IResourceRegenStrategy
{
    private readonly Dictionary<(Spellbook, int), float> elapsedByKey = new();

    public string Name => "PreparedSpellbookRegen";

    public void Tick(UnitEntityData unit, RegenTickContext context)
    {
        if (!context.Settings.ResourceRegen.EnablePreparedSpellbookRegen)
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
            if (spellbook?.Blueprint == null || !spellbook.Blueprint.MemorizeSpells)
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
        for (var spellLevel = 1; spellLevel <= 9; spellLevel++)
        {
            var intervalSeconds = context.Settings.ResourceRegen.GetIntervalSecondsForSpellLevel(spellLevel);
            if (intervalSeconds <= 0f)
            {
                continue;
            }

            var memorizedSlots = spellbook.GetMemorizedSpellSlots(spellLevel)
                .Where(slot => slot?.SpellShell != null)
                .OrderBy(slot => slot.Index)
                .ToList();
            if (memorizedSlots.Count == 0)
            {
                continue;
            }

            var spentSlots = memorizedSlots.Where(slot => !slot.Available).ToList();
            if (spentSlots.Count == 0)
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
                context.Logger.Verbose(
                    $"{Name} is waiting for level {spellLevel} on {ResourceRegenHelpers.GetUnitName(unit)} ({elapsedSeconds:0.##}/{intervalSeconds:0.##} seconds, {spentSlots.Count} spent prepared slot(s)).");
                continue;
            }

            var chosenSlot = spentSlots[0];
            var spellName = ResourceRegenHelpers.GetPreparedSpellName(chosenSlot);
            var beforeAvailable = CountAvailableSlots(memorizedSlots);

            chosenSlot.Available = true;
            if (chosenSlot.SpellShell != null && spellbook.RemoveMemorizedSpells.Contains(chosenSlot.SpellShell))
            {
                spellbook.RemoveMemorizedSpells.Remove(chosenSlot.SpellShell);
            }

            var afterAvailable = CountAvailableSlots(memorizedSlots);
            var restoredSlots = Math.Max(0, afterAvailable - beforeAvailable);

            if (restoredSlots <= 0)
            {
                context.Logger.Verbose(
                    $"{Name} tried to restore level {spellLevel} slot #{chosenSlot.Index} ({spellName}) for {ResourceRegenHelpers.GetUnitName(unit)}, but the prepared spellbook state did not change.");
                elapsedByKey[key] = 0f;
                continue;
            }

            ResourceRegenFxPlayer.TryPlayOnUnit(context.Logger, context.Settings, unit);
            context.Logger.Info(
                $"{Name} restored level {spellLevel} slot #{chosenSlot.Index} ({spellName}) for {ResourceRegenHelpers.GetUnitName(unit)} ({beforeAvailable}/{memorizedSlots.Count} -> {afterAvailable}/{memorizedSlots.Count} available prepared slot(s)).");
            elapsedByKey[key] = 0f;
        }
    }

    private static int CountAvailableSlots(List<SpellSlot> slots)
    {
        return slots.Count(slot => slot.Available);
    }
}
