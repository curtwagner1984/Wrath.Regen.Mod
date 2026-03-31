using System.Collections.Generic;

namespace WrathRegenMod;

internal sealed class SettingsSnapshot
{
    public bool GeneralEnabled;
    public bool MirrorLogsToGameLog;
    public LogLevel LogLevel;
    public bool DiagnosticsEnabled;
    public float DiagnosticsTickIntervalSeconds;
    public bool HealthRegenEnabled;
    public bool HealthRegenOutOfCombatOnly;
    public bool HealthRegenIncludePets;
    public bool HealthRegenIncludeSummons;
    public bool HealthRegenShowInGameLog;
    public float HealthRegenTickIntervalSeconds;
    public int HealthRegenHealthPerTick;
    public bool ResourceRegenEnabled;
    public bool ResourceRegenOutOfCombatOnly;
    public float ResourceRegenTickIntervalSeconds;
    public bool ResourceRegenEnableSpontaneousSpellbookRegen;
    public bool ResourceRegenEnablePreparedSpellbookRegen;
    public bool ResourceRegenEnableGenericAbilityResourceRegen;
    public bool ResourceRegenShowVisualEffects;
    public ResourceRegenVisualEffectStyle ResourceRegenVisualEffectStyle;
    public float ResourceRegenLevel1IntervalSeconds;
    public float ResourceRegenLevel2IntervalSeconds;
    public float ResourceRegenLevel3IntervalSeconds;
    public float ResourceRegenLevel4IntervalSeconds;
    public float ResourceRegenLevel5IntervalSeconds;
    public float ResourceRegenLevel6IntervalSeconds;
    public float ResourceRegenLevel7IntervalSeconds;
    public float ResourceRegenLevel8IntervalSeconds;
    public float ResourceRegenLevel9IntervalSeconds;
    public int ResourceRegenGenericResourceRestoreAmount;
    public float ResourceRegenGenericTier1IntervalSeconds;
    public float ResourceRegenGenericTier2IntervalSeconds;
    public float ResourceRegenGenericTier3IntervalSeconds;
    public float ResourceRegenGenericTier4IntervalSeconds;
    public float ResourceRegenGenericTier5IntervalSeconds;
    public float ResourceRegenGenericTier6IntervalSeconds;

    public static SettingsSnapshot Capture(ModSettings settings)
    {
        return new SettingsSnapshot
        {
            GeneralEnabled = settings.General.Enabled,
            MirrorLogsToGameLog = settings.General.MirrorModLogsToGameLog,
            LogLevel = settings.General.LogLevel,
            DiagnosticsEnabled = settings.Diagnostics.EnablePartyDiagnostics,
            DiagnosticsTickIntervalSeconds = settings.Diagnostics.TickIntervalSeconds,
            HealthRegenEnabled = settings.HealthRegen.Enabled,
            HealthRegenOutOfCombatOnly = settings.HealthRegen.OnlyRegenOutOfCombat,
            HealthRegenIncludePets = settings.HealthRegen.IncludePets,
            HealthRegenIncludeSummons = settings.HealthRegen.IncludeSummons,
            HealthRegenShowInGameLog = settings.HealthRegen.ShowHealingInGameLog,
            HealthRegenTickIntervalSeconds = settings.HealthRegen.TickIntervalSeconds,
            HealthRegenHealthPerTick = settings.HealthRegen.HealthPerTick,
            ResourceRegenEnabled = settings.ResourceRegen.Enabled,
            ResourceRegenOutOfCombatOnly = settings.ResourceRegen.OnlyRegenOutOfCombat,
            ResourceRegenTickIntervalSeconds = settings.ResourceRegen.TickIntervalSeconds,
            ResourceRegenEnableSpontaneousSpellbookRegen = settings.ResourceRegen.EnableSpontaneousSpellbookRegen,
            ResourceRegenEnablePreparedSpellbookRegen = settings.ResourceRegen.EnablePreparedSpellbookRegen,
            ResourceRegenEnableGenericAbilityResourceRegen = settings.ResourceRegen.EnableGenericAbilityResourceRegen,
            ResourceRegenShowVisualEffects = settings.ResourceRegen.ShowVisualEffects,
            ResourceRegenVisualEffectStyle = settings.ResourceRegen.VisualEffectStyle,
            ResourceRegenLevel1IntervalSeconds = settings.ResourceRegen.Level1IntervalSeconds,
            ResourceRegenLevel2IntervalSeconds = settings.ResourceRegen.Level2IntervalSeconds,
            ResourceRegenLevel3IntervalSeconds = settings.ResourceRegen.Level3IntervalSeconds,
            ResourceRegenLevel4IntervalSeconds = settings.ResourceRegen.Level4IntervalSeconds,
            ResourceRegenLevel5IntervalSeconds = settings.ResourceRegen.Level5IntervalSeconds,
            ResourceRegenLevel6IntervalSeconds = settings.ResourceRegen.Level6IntervalSeconds,
            ResourceRegenLevel7IntervalSeconds = settings.ResourceRegen.Level7IntervalSeconds,
            ResourceRegenLevel8IntervalSeconds = settings.ResourceRegen.Level8IntervalSeconds,
            ResourceRegenLevel9IntervalSeconds = settings.ResourceRegen.Level9IntervalSeconds,
            ResourceRegenGenericResourceRestoreAmount = settings.ResourceRegen.GenericResourceRestoreAmount,
            ResourceRegenGenericTier1IntervalSeconds = settings.ResourceRegen.GenericTier1IntervalSeconds,
            ResourceRegenGenericTier2IntervalSeconds = settings.ResourceRegen.GenericTier2IntervalSeconds,
            ResourceRegenGenericTier3IntervalSeconds = settings.ResourceRegen.GenericTier3IntervalSeconds,
            ResourceRegenGenericTier4IntervalSeconds = settings.ResourceRegen.GenericTier4IntervalSeconds,
            ResourceRegenGenericTier5IntervalSeconds = settings.ResourceRegen.GenericTier5IntervalSeconds,
            ResourceRegenGenericTier6IntervalSeconds = settings.ResourceRegen.GenericTier6IntervalSeconds
        };
    }

    public IEnumerable<string> DescribeChanges(SettingsSnapshot previous)
    {
        if (previous == null)
        {
            yield break;
        }

        if (GeneralEnabled != previous.GeneralEnabled)
        {
            yield return $"General.Enabled: {previous.GeneralEnabled} -> {GeneralEnabled}";
        }

        if (MirrorLogsToGameLog != previous.MirrorLogsToGameLog)
        {
            yield return $"General.MirrorModLogsToGameLog: {previous.MirrorLogsToGameLog} -> {MirrorLogsToGameLog}";
        }

        if (LogLevel != previous.LogLevel)
        {
            yield return $"General.LogLevel: {previous.LogLevel} -> {LogLevel}";
        }

        if (DiagnosticsEnabled != previous.DiagnosticsEnabled)
        {
            yield return $"Diagnostics.EnablePartyDiagnostics: {previous.DiagnosticsEnabled} -> {DiagnosticsEnabled}";
        }

        if (DiagnosticsTickIntervalSeconds != previous.DiagnosticsTickIntervalSeconds)
        {
            yield return $"Diagnostics.TickIntervalSeconds: {previous.DiagnosticsTickIntervalSeconds:0.###} -> {DiagnosticsTickIntervalSeconds:0.###}";
        }

        if (HealthRegenEnabled != previous.HealthRegenEnabled)
        {
            yield return $"HealthRegen.Enabled: {previous.HealthRegenEnabled} -> {HealthRegenEnabled}";
        }

        if (HealthRegenOutOfCombatOnly != previous.HealthRegenOutOfCombatOnly)
        {
            yield return $"HealthRegen.OnlyRegenOutOfCombat: {previous.HealthRegenOutOfCombatOnly} -> {HealthRegenOutOfCombatOnly}";
        }

        if (HealthRegenIncludePets != previous.HealthRegenIncludePets)
        {
            yield return $"HealthRegen.IncludePets: {previous.HealthRegenIncludePets} -> {HealthRegenIncludePets}";
        }

        if (HealthRegenIncludeSummons != previous.HealthRegenIncludeSummons)
        {
            yield return $"HealthRegen.IncludeSummons: {previous.HealthRegenIncludeSummons} -> {HealthRegenIncludeSummons}";
        }

        if (HealthRegenShowInGameLog != previous.HealthRegenShowInGameLog)
        {
            yield return $"HealthRegen.ShowHealingInGameLog: {previous.HealthRegenShowInGameLog} -> {HealthRegenShowInGameLog}";
        }

        if (HealthRegenTickIntervalSeconds != previous.HealthRegenTickIntervalSeconds)
        {
            yield return $"HealthRegen.TickIntervalSeconds: {previous.HealthRegenTickIntervalSeconds:0.###} -> {HealthRegenTickIntervalSeconds:0.###}";
        }

        if (HealthRegenHealthPerTick != previous.HealthRegenHealthPerTick)
        {
            yield return $"HealthRegen.HealthPerTick: {previous.HealthRegenHealthPerTick} -> {HealthRegenHealthPerTick}";
        }

        if (ResourceRegenEnabled != previous.ResourceRegenEnabled)
        {
            yield return $"ResourceRegen.Enabled: {previous.ResourceRegenEnabled} -> {ResourceRegenEnabled}";
        }

        if (ResourceRegenOutOfCombatOnly != previous.ResourceRegenOutOfCombatOnly)
        {
            yield return $"ResourceRegen.OnlyRegenOutOfCombat: {previous.ResourceRegenOutOfCombatOnly} -> {ResourceRegenOutOfCombatOnly}";
        }

        if (ResourceRegenTickIntervalSeconds != previous.ResourceRegenTickIntervalSeconds)
        {
            yield return $"ResourceRegen.TickIntervalSeconds: {previous.ResourceRegenTickIntervalSeconds:0.###} -> {ResourceRegenTickIntervalSeconds:0.###}";
        }

        if (ResourceRegenEnableSpontaneousSpellbookRegen != previous.ResourceRegenEnableSpontaneousSpellbookRegen)
        {
            yield return $"ResourceRegen.EnableSpontaneousSpellbookRegen: {previous.ResourceRegenEnableSpontaneousSpellbookRegen} -> {ResourceRegenEnableSpontaneousSpellbookRegen}";
        }

        if (ResourceRegenEnablePreparedSpellbookRegen != previous.ResourceRegenEnablePreparedSpellbookRegen)
        {
            yield return $"ResourceRegen.EnablePreparedSpellbookRegen: {previous.ResourceRegenEnablePreparedSpellbookRegen} -> {ResourceRegenEnablePreparedSpellbookRegen}";
        }

        if (ResourceRegenEnableGenericAbilityResourceRegen != previous.ResourceRegenEnableGenericAbilityResourceRegen)
        {
            yield return $"ResourceRegen.EnableGenericAbilityResourceRegen: {previous.ResourceRegenEnableGenericAbilityResourceRegen} -> {ResourceRegenEnableGenericAbilityResourceRegen}";
        }

        if (ResourceRegenShowVisualEffects != previous.ResourceRegenShowVisualEffects)
        {
            yield return $"ResourceRegen.ShowVisualEffects: {previous.ResourceRegenShowVisualEffects} -> {ResourceRegenShowVisualEffects}";
        }

        if (ResourceRegenVisualEffectStyle != previous.ResourceRegenVisualEffectStyle)
        {
            yield return $"ResourceRegen.VisualEffectStyle: {previous.ResourceRegenVisualEffectStyle} -> {ResourceRegenVisualEffectStyle}";
        }

        if (ResourceRegenLevel1IntervalSeconds != previous.ResourceRegenLevel1IntervalSeconds)
        {
            yield return $"ResourceRegen.Level1IntervalSeconds: {previous.ResourceRegenLevel1IntervalSeconds:0.###} -> {ResourceRegenLevel1IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel2IntervalSeconds != previous.ResourceRegenLevel2IntervalSeconds)
        {
            yield return $"ResourceRegen.Level2IntervalSeconds: {previous.ResourceRegenLevel2IntervalSeconds:0.###} -> {ResourceRegenLevel2IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel3IntervalSeconds != previous.ResourceRegenLevel3IntervalSeconds)
        {
            yield return $"ResourceRegen.Level3IntervalSeconds: {previous.ResourceRegenLevel3IntervalSeconds:0.###} -> {ResourceRegenLevel3IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel4IntervalSeconds != previous.ResourceRegenLevel4IntervalSeconds)
        {
            yield return $"ResourceRegen.Level4IntervalSeconds: {previous.ResourceRegenLevel4IntervalSeconds:0.###} -> {ResourceRegenLevel4IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel5IntervalSeconds != previous.ResourceRegenLevel5IntervalSeconds)
        {
            yield return $"ResourceRegen.Level5IntervalSeconds: {previous.ResourceRegenLevel5IntervalSeconds:0.###} -> {ResourceRegenLevel5IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel6IntervalSeconds != previous.ResourceRegenLevel6IntervalSeconds)
        {
            yield return $"ResourceRegen.Level6IntervalSeconds: {previous.ResourceRegenLevel6IntervalSeconds:0.###} -> {ResourceRegenLevel6IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel7IntervalSeconds != previous.ResourceRegenLevel7IntervalSeconds)
        {
            yield return $"ResourceRegen.Level7IntervalSeconds: {previous.ResourceRegenLevel7IntervalSeconds:0.###} -> {ResourceRegenLevel7IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel8IntervalSeconds != previous.ResourceRegenLevel8IntervalSeconds)
        {
            yield return $"ResourceRegen.Level8IntervalSeconds: {previous.ResourceRegenLevel8IntervalSeconds:0.###} -> {ResourceRegenLevel8IntervalSeconds:0.###}";
        }

        if (ResourceRegenLevel9IntervalSeconds != previous.ResourceRegenLevel9IntervalSeconds)
        {
            yield return $"ResourceRegen.Level9IntervalSeconds: {previous.ResourceRegenLevel9IntervalSeconds:0.###} -> {ResourceRegenLevel9IntervalSeconds:0.###}";
        }

        if (ResourceRegenGenericResourceRestoreAmount != previous.ResourceRegenGenericResourceRestoreAmount)
        {
            yield return $"ResourceRegen.GenericResourceRestoreAmount: {previous.ResourceRegenGenericResourceRestoreAmount} -> {ResourceRegenGenericResourceRestoreAmount}";
        }

        if (ResourceRegenGenericTier1IntervalSeconds != previous.ResourceRegenGenericTier1IntervalSeconds)
        {
            yield return $"ResourceRegen.GenericTier1IntervalSeconds: {previous.ResourceRegenGenericTier1IntervalSeconds:0.###} -> {ResourceRegenGenericTier1IntervalSeconds:0.###}";
        }

        if (ResourceRegenGenericTier2IntervalSeconds != previous.ResourceRegenGenericTier2IntervalSeconds)
        {
            yield return $"ResourceRegen.GenericTier2IntervalSeconds: {previous.ResourceRegenGenericTier2IntervalSeconds:0.###} -> {ResourceRegenGenericTier2IntervalSeconds:0.###}";
        }

        if (ResourceRegenGenericTier3IntervalSeconds != previous.ResourceRegenGenericTier3IntervalSeconds)
        {
            yield return $"ResourceRegen.GenericTier3IntervalSeconds: {previous.ResourceRegenGenericTier3IntervalSeconds:0.###} -> {ResourceRegenGenericTier3IntervalSeconds:0.###}";
        }

        if (ResourceRegenGenericTier4IntervalSeconds != previous.ResourceRegenGenericTier4IntervalSeconds)
        {
            yield return $"ResourceRegen.GenericTier4IntervalSeconds: {previous.ResourceRegenGenericTier4IntervalSeconds:0.###} -> {ResourceRegenGenericTier4IntervalSeconds:0.###}";
        }

        if (ResourceRegenGenericTier5IntervalSeconds != previous.ResourceRegenGenericTier5IntervalSeconds)
        {
            yield return $"ResourceRegen.GenericTier5IntervalSeconds: {previous.ResourceRegenGenericTier5IntervalSeconds:0.###} -> {ResourceRegenGenericTier5IntervalSeconds:0.###}";
        }

        if (ResourceRegenGenericTier6IntervalSeconds != previous.ResourceRegenGenericTier6IntervalSeconds)
        {
            yield return $"ResourceRegen.GenericTier6IntervalSeconds: {previous.ResourceRegenGenericTier6IntervalSeconds:0.###} -> {ResourceRegenGenericTier6IntervalSeconds:0.###}";
        }
    }
}
