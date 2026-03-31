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
            HealthRegenHealthPerTick = settings.HealthRegen.HealthPerTick
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
    }
}
