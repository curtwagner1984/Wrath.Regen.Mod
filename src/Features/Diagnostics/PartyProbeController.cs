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
        if (!settings.EnablePartyDiagnostics)
        {
            return;
        }

        if (!loggedReadyMessage)
        {
            logger.Log("PartyProbeController is running. Logging party HP and combat state on each interval.");
            loggedReadyMessage = true;
        }

        if (!Game.HasInstance || Game.Instance.Player == null)
        {
            if (!loggedMissingGameMessage)
            {
                logger.Log("Game instance or player data is not ready yet.");
                loggedMissingGameMessage = true;
            }

            return;
        }

        loggedMissingGameMessage = false;
        elapsedSeconds += deltaTime;
        if (elapsedSeconds < settings.TickIntervalSeconds)
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
            logger.Log("Party list is null.");
            return;
        }

        logger.Log($"Party snapshot: {party.Count} unit(s).");

        if (!settings.LogVerbose)
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
            logger.Log("Party member entry was null.");
            return;
        }

        var currentHp = unit.Stats?.HitPoints?.ModifiedValue ?? -1;
        var maxHp = unit.MaxHP;
        var inCombat = unit.CombatState?.IsInCombat ?? false;
        var isDead = unit.State?.IsDead ?? false;
        var isUnconscious = unit.State?.IsUnconscious ?? false;
        var isUndead = unit.Descriptor?.IsUndead ?? false;
        var name = string.IsNullOrWhiteSpace(unit.CharacterName) ? "<unnamed>" : unit.CharacterName;

        logger.Log(
            $"Party unit: {name} | HP {currentHp}/{maxHp} | InCombat={inCombat} | Dead={isDead} | Unconscious={isUnconscious} | Undead={isUndead}");
    }
}
