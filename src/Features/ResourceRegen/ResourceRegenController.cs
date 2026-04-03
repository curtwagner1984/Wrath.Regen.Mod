using System;
using Kingmaker;
using Kingmaker.Controllers;

namespace WrathRegenMod;

internal sealed class ResourceRegenController : IController
{
    private static readonly IResourceRegenStrategy[] Strategies =
    {
        new SpontaneousSpellbookRegenStrategy(),
        new PreparedSpellbookRegenStrategy(),
        new GenericAbilityResourceRegenStrategy(),
        new KineticistBurnRegenStrategy()
    };

    private readonly ModRuntime runtime;
    private float elapsedSeconds;
    private bool loggedReadyMessage;

    public ResourceRegenController(ModRuntime runtime)
    {
        this.runtime = runtime;
    }

    public void Activate()
    {
    }

    public void Deactivate()
    {
        ResetAllStrategies();
        loggedReadyMessage = false;
    }

    public void Tick()
    {
        if (!runtime.IsGameplayEnabled)
        {
            return;
        }

        var settings = runtime.Settings;
        var logger = runtime.Logger;

        try
        {
            if (!settings.ResourceRegen.Enabled)
            {
                return;
            }

            if (!loggedReadyMessage)
            {
                logger.Info("ResourceRegenController is running. Prototype resource regeneration currently targets spellbooks, generic ability resources, and Kineticist burn.");
                loggedReadyMessage = true;
            }

            if (!Game.HasInstance || Game.Instance.Player == null)
            {
                return;
            }

            elapsedSeconds += Game.Instance.TimeController.GameDeltaTime;
            if (elapsedSeconds < settings.ResourceRegen.TickIntervalSeconds)
            {
                return;
            }

            var tickElapsedSeconds = elapsedSeconds;
            elapsedSeconds = 0f;

            if (settings.ResourceRegen.OnlyRegenOutOfCombat && Game.Instance.Player.IsInCombat)
            {
                if (logger.IsVerbose)
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
                        if (logger.IsError)
                            logger.Error($"{strategy.Name} failed for {GetUnitName(unit)}: {ex}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (logger.IsError)
                logger.Error($"Unhandled exception in resource regeneration controller: {ex}");
        }
    }

    public void ResetAllStrategies()
    {
        foreach (var strategy in Strategies)
        {
            strategy.Reset();
        }

        elapsedSeconds = 0f;
    }

    private static string GetUnitName(Kingmaker.EntitySystem.Entities.UnitEntityData unit)
    {
        return string.IsNullOrWhiteSpace(unit.CharacterName) ? "<unnamed>" : unit.CharacterName;
    }
}
