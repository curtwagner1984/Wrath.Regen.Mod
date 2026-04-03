using System.Xml.Serialization;
using UnityModManagerNet;

namespace WrathRegenMod;

[XmlRoot("WrathRegenModSettings")]
[XmlType("WrathRegenModSettings")]
public sealed class ModSettings : UnityModManager.ModSettings
{
    public GeneralSettings General = new GeneralSettings();
    public HealthRegenSettings HealthRegen = new HealthRegenSettings();
    public ResourceRegenSettings ResourceRegen = new ResourceRegenSettings();

    public override void Save(UnityModManager.ModEntry entry)
    {
        Save(this, entry);
    }
}

public sealed class GeneralSettings
{
    public bool Enabled = true;
    public bool MirrorModLogsToGameLog = true;
    public LogLevel LogLevel = LogLevel.Error;
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

public sealed class ResourceRegenSettings
{
    public bool Enabled = false;
    public bool OnlyRegenOutOfCombat = true;
    public float TickIntervalSeconds = 1.0f;
    public bool EnableSpontaneousSpellbookRegen = true;
    public bool EnablePreparedSpellbookRegen = true;
    public bool EnableGenericAbilityResourceRegen = true;
    public bool ShowVisualEffects = true;
    public ResourceRegenVisualEffectStyle VisualEffectStyle = ResourceRegenVisualEffectStyle.DivineRefresh;
    public float Level1IntervalSeconds = 30.0f;
    public float Level2IntervalSeconds = 40.0f;
    public float Level3IntervalSeconds = 50.0f;
    public float Level4IntervalSeconds = 60.0f;
    public float Level5IntervalSeconds = 75.0f;
    public float Level6IntervalSeconds = 90.0f;
    public float Level7IntervalSeconds = 120.0f;
    public float Level8IntervalSeconds = 150.0f;
    public float Level9IntervalSeconds = 180.0f;
    public float Level10IntervalSeconds = 210.0f;
    public int GenericResourceRestoreAmount = 1;
    public float GenericTier1IntervalSeconds = 30.0f;
    public float GenericTier2IntervalSeconds = 40.0f;
    public float GenericTier3IntervalSeconds = 50.0f;
    public float GenericTier4IntervalSeconds = 60.0f;
    public float GenericTier5IntervalSeconds = 75.0f;
    public float GenericTier6IntervalSeconds = 90.0f;
    public bool EnableKineticistBurnRegen = false;
    public float KineticistBurnRestoreIntervalSeconds = 60.0f;
    public int KineticistBurnFloor = 0;

    public float GetIntervalSecondsForSpellLevel(int spellLevel)
    {
        switch (spellLevel)
        {
            case 1:
                return Level1IntervalSeconds;
            case 2:
                return Level2IntervalSeconds;
            case 3:
                return Level3IntervalSeconds;
            case 4:
                return Level4IntervalSeconds;
            case 5:
                return Level5IntervalSeconds;
            case 6:
                return Level6IntervalSeconds;
            case 7:
                return Level7IntervalSeconds;
            case 8:
                return Level8IntervalSeconds;
            case 9:
                return Level9IntervalSeconds;
            case 10:
                return Level10IntervalSeconds;
            default:
                return spellLevel > 10 ? Level10IntervalSeconds : 0f;
        }
    }

    public float GetIntervalSecondsForGenericResourceMax(int maxAmount)
    {
        switch (GetGenericResourceTier(maxAmount))
        {
            case 1:
                return GenericTier1IntervalSeconds;
            case 2:
                return GenericTier2IntervalSeconds;
            case 3:
                return GenericTier3IntervalSeconds;
            case 4:
                return GenericTier4IntervalSeconds;
            case 5:
                return GenericTier5IntervalSeconds;
            case 6:
                return GenericTier6IntervalSeconds;
            default:
                return 0f;
        }
    }

    public int GetGenericResourceTier(int maxAmount)
    {
        if (maxAmount >= 10)
        {
            return 1;
        }

        if (maxAmount >= 8)
        {
            return 2;
        }

        if (maxAmount >= 6)
        {
            return 3;
        }

        if (maxAmount >= 4)
        {
            return 4;
        }

        if (maxAmount >= 2)
        {
            return 5;
        }

        if (maxAmount >= 1)
        {
            return 6;
        }

        return 0;
    }
}

public enum ResourceRegenVisualEffectStyle
{
    DivineRefresh = 0,
    ArcaneRefresh = 1
}

public enum LogLevel
{
    None = -1,
    Error = 0,
    Info = 1,
    Verbose = 2
}
