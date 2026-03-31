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

    public void Error(string message)
    {
        modEntry.Logger.Error(message);
        MirrorToGameLog($"[Error] {message}");
    }

    public void Info(string message)
    {
        if (!ShouldLog(LogLevel.Info))
        {
            return;
        }

        modEntry.Logger.Log(message);
        MirrorToGameLog(message);
    }

    public void Verbose(string message)
    {
        if (!ShouldLog(LogLevel.Verbose))
        {
            return;
        }

        modEntry.Logger.Log($"[Verbose] {message}");
        MirrorToGameLog($"[Verbose] {message}");
    }

    private bool ShouldLog(LogLevel level)
    {
        return settings.General.LogLevel >= level;
    }

    private void MirrorToGameLog(string message)
    {
        if (!settings.General.MirrorModLogsToGameLog)
        {
            return;
        }

        EventBus.RaiseEvent<ILogMessageUIHandler>(handler => handler.HandleLogMessage(message));
    }
}
