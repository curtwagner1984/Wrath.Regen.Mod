using System.Xml.Serialization;
using UnityModManagerNet;

namespace WrathRegenMod;

[XmlRoot("WrathRegenModSettings")]
[XmlType("WrathRegenModSettings")]
public sealed class ModSettings : UnityModManager.ModSettings
{
    public GeneralSettings General = new GeneralSettings();
    public DiagnosticsSettings Diagnostics = new DiagnosticsSettings();
    public HealthRegenSettings HealthRegen = new HealthRegenSettings();

    public override void Save(UnityModManager.ModEntry entry)
    {
        Save(this, entry);
    }
}

public sealed class GeneralSettings
{
    public bool Enabled = true;
    public bool MirrorModLogsToGameLog = false;
    public LogLevel LogLevel = LogLevel.Info;
}

public sealed class DiagnosticsSettings
{
    public bool EnablePartyDiagnostics = true;
    public bool LogVerbose = false;
    public float TickIntervalSeconds = 5.0f;
}

public sealed class HealthRegenSettings
{
    public bool Enabled = false;
    public bool OnlyRegenOutOfCombat = true;
    public bool IncludePets = true;
    public bool IncludeSummons = false;
    public bool ShowHealingInGameLog = true;
    public float TickIntervalSeconds = 5.0f;
    public int HealthPerTick = 1;
}

public enum LogLevel
{
    Error = 0,
    Info = 1,
    Verbose = 2
}
