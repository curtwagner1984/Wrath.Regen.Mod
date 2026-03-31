using System;
using Kingmaker;

namespace WrathRegenMod;

internal static class ResourceRegenController
{
    private static readonly IResourceRegenStrategy[] Strategies =
    {
        new SpontaneousSpellbookRegenStrategy(),
        new PreparedSpellbookRegenStrategy()
    };

    private static float elapsedSeconds;
    private static bool loggedReadyMessage;

    public static void Tick(ModLogger logger, ModSettings settings, float deltaTime)
    {
        if (!settings.ResourceRegen.Enabled)
        {
            return;
        }

        if (!loggedReadyMessage)
        {
            logger.Info("ResourceRegenController is running. Prototype resource regeneration currently targets spontaneous and prepared spellbooks.");
            loggedReadyMessage = true;
        }

        if (!Game.HasInstance || Game.Instance.Player == null)
        {
            return;
        }

        elapsedSeconds += deltaTime;
        if (elapsedSeconds < settings.ResourceRegen.TickIntervalSeconds)
        {
            return;
        }

        var tickElapsedSeconds = elapsedSeconds;
        elapsedSeconds = 0f;

        if (settings.ResourceRegen.OnlyRegenOutOfCombat && Game.Instance.Player.IsInCombat)
        {
            logger.Verbose("Resource regeneration skipped because the party is in combat.");
            return;
        }

        var context = new RegenTickContext(logger, settings, tickElapsedSeconds);
        foreach (var unit in Game.Instance.Player.Party)
        {
            if (unit == null || !unit.IsInGame || unit.Suppressed || unit.IsDetached || !unit.IsPlayerFaction)
            {
                continue;
            }

            foreach (var strategy in Strategies)
            {
                try
                {
                    strategy.Tick(unit, context);
                }
                catch (Exception ex)
                {
                    logger.Error($"{strategy.Name} failed for {GetUnitName(unit)}: {ex}");
                }
            }
        }
    }

    private static string GetUnitName(Kingmaker.EntitySystem.Entities.UnitEntityData unit)
    {
        return string.IsNullOrWhiteSpace(unit.CharacterName) ? "<unnamed>" : unit.CharacterName;
    }
}
