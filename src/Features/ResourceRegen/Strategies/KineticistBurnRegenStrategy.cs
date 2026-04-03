using System;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Class.Kineticist;

namespace WrathRegenMod;

internal sealed class KineticistBurnRegenStrategy : IResourceRegenStrategy
{
    private readonly Dictionary<UnitEntityData, float> elapsedByUnit = new();

    public string Name => "KineticistBurnRegen";

    public void Tick(UnitEntityData unit, RegenTickContext context)
    {
        if (!context.Settings.ResourceRegen.EnableKineticistBurnRegen)
        {
            return;
        }

        if (unit?.Descriptor == null)
        {
            if (context.Logger.IsError)
                context.Logger.Error($"{Name} encountered a unit with a missing descriptor.");
            return;
        }

        var kineticistPart = unit.Get<UnitPartKineticist>();
        if (kineticistPart == null)
        {
            return;
        }

        var currentBurn = kineticistPart.AcceptedBurn;
        var floor = Math.Max(0, context.Settings.ResourceRegen.KineticistBurnFloor);
        if (currentBurn <= floor)
        {
            elapsedByUnit.Remove(unit);
            if (context.Logger.IsVerbose)
                context.Logger.Verbose($"{Name} skipped {ResourceRegenHelpers.GetUnitName(unit)} because current burn {currentBurn} is already at or below the floor {floor}.");
            return;
        }

        var intervalSeconds = context.Settings.ResourceRegen.KineticistBurnRestoreIntervalSeconds;
        if (intervalSeconds <= 0f)
        {
            elapsedByUnit.Remove(unit);
            if (context.Logger.IsVerbose)
                context.Logger.Verbose($"{Name} is disabled for {ResourceRegenHelpers.GetUnitName(unit)} because the restore interval is set to 0.");
            return;
        }

        elapsedByUnit.TryGetValue(unit, out var elapsedSeconds);
        elapsedSeconds += context.ElapsedSeconds;

        if (elapsedSeconds < intervalSeconds)
        {
            elapsedByUnit[unit] = elapsedSeconds;
            if (context.Logger.IsVerbose)
                context.Logger.Verbose(
                    $"{Name} is waiting for {ResourceRegenHelpers.GetUnitName(unit)} ({elapsedSeconds:0.##}/{intervalSeconds:0.##} seconds, burn {currentBurn}, floor {floor}, max burn {kineticistPart.MaxBurn}).");
            return;
        }

        var beforeBurn = kineticistPart.AcceptedBurn;
        kineticistPart.HealBurn(1);
        var afterBurn = kineticistPart.AcceptedBurn;
        var healedBurn = Math.Max(0, beforeBurn - afterBurn);

        if (healedBurn <= 0)
        {
            if (context.Logger.IsVerbose)
                context.Logger.Verbose(
                    $"{Name} tried to heal burn for {ResourceRegenHelpers.GetUnitName(unit)} (burn {beforeBurn}, floor {floor}), but the Kineticist burn state did not change.");
            elapsedByUnit[unit] = 0f;
            return;
        }

        ResourceRegenFxPlayer.TryPlayOnUnit(context.Logger, context.Settings, unit);
        if (context.Logger.IsInfo)
            context.Logger.Info(
                $"{Name} healed {healedBurn} burn from {ResourceRegenHelpers.GetUnitName(unit)} ({beforeBurn} -> {afterBurn}, floor {floor}, max burn {kineticistPart.MaxBurn}).");
        elapsedByUnit[unit] = 0f;
    }

    public void Reset()
    {
        elapsedByUnit.Clear();
    }
}
