using System;
using System.Collections.Generic;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Parts;

namespace WrathRegenMod;

internal static class HealthRegenController
{
    private static float elapsedSeconds;
    private static bool loggedReadyMessage;

    public static void Tick(ModLogger logger, ModSettings settings, float deltaTime)
    {
        if (!settings.HealthRegen.Enabled)
        {
            return;
        }

        if (!loggedReadyMessage)
        {
            logger.Info("HealthRegenController is running. Prototype healing uses Wrath's built-in rule system.");
            loggedReadyMessage = true;
        }

        if (!Game.HasInstance || Game.Instance.Player == null)
        {
            return;
        }

        elapsedSeconds += deltaTime;
        if (elapsedSeconds < settings.HealthRegen.TickIntervalSeconds)
        {
            return;
        }

        elapsedSeconds = 0f;

        if (settings.HealthRegen.OnlyRegenOutOfCombat && Game.Instance.Player.IsInCombat)
        {
            if (logger.IsVerbose)
                logger.Verbose("Health prototype skipped because the party is in combat.");
            return;
        }

        RunPartyHealingTick(logger, settings);
    }

    private static void RunPartyHealingTick(ModLogger logger, ModSettings settings)
    {
        var healedUnits = 0;
        var eligibleUnits = 0;
        foreach (var unit in GetConfiguredTargets(settings))
        {
            eligibleUnits++;
            if (TryHealUnit(logger, settings, unit))
            {
                healedUnits++;
            }
        }

        if (eligibleUnits == 0)
        {
            if (logger.IsVerbose)
                logger.Verbose("Health prototype skipped because no eligible units were available.");
            return;
        }

        if (healedUnits == 0)
        {
            if (logger.IsVerbose)
                logger.Verbose("Health prototype found no party members who needed healing.");
        }
    }

    private static bool TryHealUnit(ModLogger logger, ModSettings settings, UnitEntityData unit)
    {
        if (unit == null || unit.State == null || unit.Descriptor == null)
        {
            if (logger.IsError)
                logger.Error("Health regen encountered a unit with missing state or descriptor.");
            return false;
        }

        if (unit.State.IsDead)
        {
            return false;
        }

        var name = string.IsNullOrWhiteSpace(unit.CharacterName) ? "<unnamed>" : unit.CharacterName;
        var missingHp = unit.Damage;
        if (missingHp <= 0)
        {
            return false;
        }

        if (unit.Descriptor.IsUndead)
        {
            return RestoreUndead(logger, settings, unit, name, missingHp);
        }

        var healAmount = Math.Min(settings.HealthRegen.HealthPerTick, missingHp);
        var healRule = new RuleHealDamage(unit, unit, healAmount)
        {
            DisableBattleLogSelf = !settings.HealthRegen.ShowHealingInGameLog
        };

        Rulebook.Trigger(healRule);

        if (healRule.Value <= 0)
        {
            if (logger.IsVerbose)
                logger.Verbose($"Health prototype attempted to heal {name}, but no HP was restored.");
            return false;
        }

        if (logger.IsInfo)
            logger.Info($"Health prototype restored {healRule.Value} HP to {name}.");
        return true;
    }

    private static bool RestoreUndead(ModLogger logger, ModSettings settings, UnitEntityData unit, string name, int missingHp)
    {
        var restoreAmount = Math.Min(settings.HealthRegen.HealthPerTick, missingHp);
        var damage = new EnergyDamage(DiceFormula.Zero, restoreAmount, DamageEnergyType.NegativeEnergy);
        var rule = new RuleDealDamage(unit, unit, damage)
        {
            DisableBattleLogSelf = !settings.HealthRegen.ShowHealingInGameLog
        };

        Rulebook.Trigger(rule);

        var restored = Math.Max(0, missingHp - unit.Damage);
        if (restored <= 0)
        {
            if (logger.IsVerbose)
                logger.Verbose($"Health prototype attempted undead restoration on {name}, but no HP was restored.");
            return false;
        }

        if (logger.IsInfo)
            logger.Info($"Health prototype restored {restored} HP to undead unit {name} via negative energy.");
        return true;
    }

    private static IEnumerable<UnitEntityData> GetConfiguredTargets(ModSettings settings)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var baseTargets = settings.HealthRegen.IncludePets
            ? Game.Instance.Player.PartyAndPets
            : Game.Instance.Player.Party;

        foreach (var unit in baseTargets)
        {
            if (ShouldInclude(unit) && seen.Add(unit.UniqueId))
            {
                yield return unit;
            }
        }

        if (!settings.HealthRegen.IncludeSummons)
        {
            yield break;
        }

        foreach (var unit in Game.Instance.Player.AllCrossSceneUnits)
        {
            if (ShouldIncludeSummon(unit) && seen.Add(unit.UniqueId))
            {
                yield return unit;
            }
        }
    }

    private static bool ShouldInclude(UnitEntityData unit)
    {
        if (unit == null || !unit.IsInGame || unit.Suppressed || unit.IsDetached)
        {
            return false;
        }

        return unit.IsPlayerFaction;
    }

    private static bool ShouldIncludeSummon(UnitEntityData unit)
    {
        if (!ShouldInclude(unit) || unit.IsPet)
        {
            return false;
        }

        var summonedMonster = unit.Get<UnitPartSummonedMonster>();
        if (summonedMonster == null)
        {
            return false;
        }

        return summonedMonster.Summoner?.IsPlayerFaction ?? false;
    }
}
