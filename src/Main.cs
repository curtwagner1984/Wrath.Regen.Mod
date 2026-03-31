using HarmonyLib;
using UnityModManagerNet;

namespace WrathRegenMod;

public static class Main
{
    private static UnityModManager.ModEntry modEntry;
    private static Harmony harmony;
    private static ModSettings settings;
    private static SettingsSnapshot lastSavedSnapshot;

    public static bool Load(UnityModManager.ModEntry entry)
    {
        modEntry = entry;
        settings = UnityModManager.ModSettings.Load<ModSettings>(entry);
        harmony = new Harmony(entry.Info.Id);
        lastSavedSnapshot = SettingsSnapshot.Capture(settings);

        entry.OnToggle = OnToggle;
        entry.OnGUI = OnGUI;
        entry.OnSaveGUI = OnSaveGUI;
        entry.OnUpdate = OnUpdate;

        GetLogger().Info("Wrath Regen Mod loaded.");
        return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry entry, bool value)
    {
        if (value)
        {
            harmony.PatchAll();
            GetLogger().Info("Wrath Regen Mod enabled.");
        }
        else
        {
            harmony.UnpatchAll(entry.Info.Id);
            GetLogger().Info("Wrath Regen Mod disabled.");
        }

        return true;
    }

    private static void OnGUI(UnityModManager.ModEntry entry)
    {
        SettingsRenderer.Draw(settings);
    }

    private static void OnSaveGUI(UnityModManager.ModEntry entry)
    {
        var logger = GetLogger();
        var currentSnapshot = SettingsSnapshot.Capture(settings);

        settings.Save(entry);
        foreach (var change in currentSnapshot.DescribeChanges(lastSavedSnapshot))
        {
            logger.Info($"Configuration changed: {change}");
        }

        lastSavedSnapshot = currentSnapshot;
    }

    private static void OnUpdate(UnityModManager.ModEntry entry, float deltaTime)
    {
        if (!entry.Enabled || !settings.General.Enabled)
        {
            return;
        }

        var logger = GetLogger();
        try
        {
            PartyProbeController.Tick(logger, settings, deltaTime);
            HealthRegenController.Tick(logger, settings, deltaTime);
            ResourceRegenController.Tick(logger, settings, deltaTime);
        }
        catch (System.Exception ex)
        {
            logger.Error($"Unhandled exception in update loop: {ex}");
        }
    }

    private static ModLogger GetLogger()
    {
        return new ModLogger(modEntry, settings);
    }
}
