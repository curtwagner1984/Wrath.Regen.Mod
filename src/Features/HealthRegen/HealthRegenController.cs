using System;
using System.Collections.Generic;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;

namespace WrathRegenMod;

internal static class HealthRegenController
{
    private static readonly HashSet<string> ReportedUndeadUnits = new HashSet<string>();

    private static float elapsedSeconds;
    private static bool loggedReadyMessage;

    public static void Tick(ModLogger logger, ModSettings settings, float deltaTime)
    {
        if (!settings.EnableHealthRegenPrototype)
        {
            return;
        }

        if (!loggedReadyMessage)
        {
            logger.Log("HealthRegenController is running. Prototype healing uses Wrath's built-in RuleHealDamage flow.");
            loggedReadyMessage = true;
        }

        if (!Game.HasInstance || Game.Instance.Player == null)
        {
            return;
        }

        elapsedSeconds += deltaTime;
        if (elapsedSeconds < settings.TickIntervalSeconds)
        {
            return;
        }

        elapsedSeconds = 0f;

        if (settings.OnlyRegenOutOfCombat && Game.Instance.Player.IsInCombat)
        {
            logger.Verbose("Health prototype skipped because the party is in combat.");
            return;
        }

        RunPartyHealingTick(logger, settings);
    }

    private static void RunPartyHealingTick(ModLogger logger, ModSettings settings)
    {
        var party = Game.Instance.Player.Party;
        if (party == null || party.Count == 0)
        {
            logger.Verbose("Health prototype skipped because no party units were available.");
            return;
        }

        var healedUnits = 0;
        foreach (var unit in party)
        {
            if (TryHealUnit(logger, settings, unit))
            {
                healedUnits++;
            }
        }

        if (healedUnits == 0)
        {
            logger.Verbose("Health prototype found no party members who needed healing.");
        }
    }

    private static bool TryHealUnit(ModLogger logger, ModSettings settings, UnitEntityData unit)
    {
        if (unit == null || unit.State == null || unit.Descriptor == null)
        {
            return false;
        }

        if (unit.State.IsDead)
        {
            return false;
        }

        var name = string.IsNullOrWhiteSpace(unit.CharacterName) ? "<unnamed>" : unit.CharacterName;
        if (unit.Descriptor.IsUndead)
        {
            if (ReportedUndeadUnits.Add(unit.UniqueId))
            {
                logger.Log($"Health prototype skipped undead unit {name}. Positive-energy healing would be unsafe here until we add the correct undead restoration path.");
            }

            return false;
        }

        var missingHp = unit.Damage;
        if (missingHp <= 0)
        {
            return false;
        }

        var healAmount = Math.Min(settings.HealthRegenAmountPerTick, missingHp);
        var healRule = new RuleHealDamage(unit, unit, healAmount)
        {
            DisableBattleLogSelf = !settings.ShowHealingInGameLog
        };

        Rulebook.Trigger(healRule);

        if (healRule.Value <= 0)
        {
            logger.Verbose($"Health prototype attempted to heal {name}, but no HP was restored.");
            return false;
        }

        logger.Log($"Health prototype restored {healRule.Value} HP to {name}.");
        return true;
    }
}
