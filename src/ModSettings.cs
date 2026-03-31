using UnityModManagerNet;

namespace WrathRegenMod;

public sealed class ModSettings : UnityModManager.ModSettings
{
    public bool Enabled = true;
    public bool LogVerbose = false;
    public float TickIntervalSeconds = 5.0f;

    public override void Save(UnityModManager.ModEntry entry)
    {
        Save(this, entry);
    }
}
