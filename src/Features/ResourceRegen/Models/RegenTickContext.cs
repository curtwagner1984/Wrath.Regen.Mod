namespace WrathRegenMod;

internal readonly struct RegenTickContext
{
    public RegenTickContext(ModLogger logger, ModSettings settings, float elapsedSeconds)
    {
        Logger = logger;
        Settings = settings;
        ElapsedSeconds = elapsedSeconds;
    }

    public ModLogger Logger { get; }

    public ModSettings Settings { get; }

    public float ElapsedSeconds { get; }
}
