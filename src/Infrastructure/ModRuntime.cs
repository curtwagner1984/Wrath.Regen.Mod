namespace WrathRegenMod;

internal sealed class ModRuntime
{
    public ModRuntime(ModSettings settings, ModLogger logger)
    {
        Settings = settings;
        Logger = logger;
    }

    public ModSettings Settings { get; }

    public ModLogger Logger { get; }

    public bool IsModEnabled { get; private set; }

    public bool IsGameplayEnabled => IsModEnabled && Settings.General.Enabled;

    public void SetModEnabled(bool value)
    {
        IsModEnabled = value;
    }
}
