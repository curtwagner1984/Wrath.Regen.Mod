using UnityModManagerNet;

namespace WrathRegenMod;

public sealed class ModSettings : UnityModManager.ModSettings
{
    public bool Enabled = true;
    public bool EnablePartyDiagnostics = true;
    public bool EnableHealthRegenPrototype = false;
    public bool OnlyRegenOutOfCombat = true;
    public bool IncludePetsInHealthRegen = true;
    public bool IncludeSummonsInHealthRegen = false;
    public bool LogVerbose = false;
    public bool MirrorModLogsToGameLog = false;
    public bool ShowHealingInGameLog = true;
    public float TickIntervalSeconds = 5.0f;
    public int HealthRegenAmountPerTick = 1;

    public override void Save(UnityModManager.ModEntry entry)
    {
        Save(this, entry);
    }
}
