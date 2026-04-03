using Kingmaker;
using Kingmaker.EntitySystem.Entities;

namespace WrathRegenMod;

internal static class PartyProbeController
{
    private static float elapsedSeconds;
    private static bool loggedReadyMessage;
    private static bool loggedMissingGameMessage;

    public static void Tick(ModLogger logger, ModSettings settings, float deltaTime)
    {
        if (!settings.Diagnostics.EnablePartyDiagnostics)
        {
            return;
        }

        if (!loggedReadyMessage)
        {
            logger.Info("PartyProbeController is running.");
            loggedReadyMessage = true;
        }

        if (!Game.HasInstance || Game.Instance.Player == null)
        {
            if (!loggedMissingGameMessage)
            {
                if (logger.IsVerbose)
                    logger.Verbose("Diagnostics skipped because game instance or player data is not ready yet.");
                loggedMissingGameMessage = true;
            }

            return;
        }

        loggedMissingGameMessage = false;
        elapsedSeconds += deltaTime;
        if (elapsedSeconds < settings.Diagnostics.TickIntervalSeconds)
        {
            return;
        }

        elapsedSeconds = 0f;
        LogPartySnapshot(logger, settings);
    }

    private static void LogPartySnapshot(ModLogger logger, ModSettings settings)
    {
        var party = Game.Instance.Player.Party;
        if (party == null)
        {
            if (logger.IsError)
                logger.Error("Diagnostics could not read the party list because it was null.");
            return;
        }

        if (logger.IsVerbose)
            logger.Verbose($"Party snapshot: {party.Count} unit(s).");

        if (!settings.Diagnostics.LogVerbose)
        {
            return;
        }

        foreach (var unit in party)
        {
            LogUnit(logger, unit);
        }
    }

    private static void LogUnit(ModLogger logger, UnitEntityData unit)
    {
        if (unit == null)
        {
            if (logger.IsError)
                logger.Error("Diagnostics encountered a null party member entry.");
            return;
        }

        var currentHp = unit.Stats?.HitPoints?.ModifiedValue ?? -1;
        var maxHp = unit.MaxHP;
        var inCombat = unit.CombatState?.IsInCombat ?? false;
        var isDead = unit.State?.IsDead ?? false;
        var isUnconscious = unit.State?.IsUnconscious ?? false;
        var isUndead = unit.Descriptor?.IsUndead ?? false;
        var name = string.IsNullOrWhiteSpace(unit.CharacterName) ? "<unnamed>" : unit.CharacterName;

        if (logger.IsVerbose)
            logger.Verbose(
                $"Party unit: {name} | HP {currentHp}/{maxHp} | InCombat={inCombat} | Dead={isDead} | Unconscious={isUnconscious} | Undead={isUndead}");
    }
}
