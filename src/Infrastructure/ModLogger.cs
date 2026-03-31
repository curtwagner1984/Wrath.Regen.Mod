using Kingmaker.PubSubSystem;
using UnityModManagerNet;

namespace WrathRegenMod;

internal sealed class ModLogger
{
    private readonly UnityModManager.ModEntry modEntry;
    private readonly ModSettings settings;

    public ModLogger(UnityModManager.ModEntry modEntry, ModSettings settings)
    {
        this.modEntry = modEntry;
        this.settings = settings;
    }

    public void Log(string message)
    {
        modEntry.Logger.Log(message);

        if (!settings.MirrorModLogsToGameLog)
        {
            return;
        }

        EventBus.RaiseEvent<ILogMessageUIHandler>(handler => handler.HandleLogMessage(message));
    }

    public void Verbose(string message)
    {
        if (!settings.LogVerbose)
        {
            return;
        }

        Log(message);
    }
}
