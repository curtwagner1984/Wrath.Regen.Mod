using HarmonyLib;
using Kingmaker.PubSubSystem;
using UnityModManagerNet;

namespace WrathRegenMod;

public static class Main
{
    private static UnityModManager.ModEntry modEntry;
    private static Harmony harmony;
    private static ModSettings settings;
    private static SettingsSnapshot lastSavedSnapshot;
    private static ModLogger logger;
    private static ModRuntime runtime;
    private static HealthRegenController healthRegenController;
    private static ResourceRegenController resourceRegenController;
    private static ResourceRegenAreaHandler areaHandler;

    public static bool Load(UnityModManager.ModEntry entry)
    {
        modEntry = entry;
        settings = UnityModManager.ModSettings.Load<ModSettings>(entry);
        logger = new ModLogger(modEntry, settings);
        runtime = new ModRuntime(settings, logger);
        runtime.SetModEnabled(entry.Enabled);
        healthRegenController = new HealthRegenController(runtime);
        resourceRegenController = new ResourceRegenController(runtime);
        harmony = new Harmony(entry.Info.Id);
        lastSavedSnapshot = SettingsSnapshot.Capture(settings);

        entry.OnToggle = OnToggle;
        entry.OnGUI = OnGUI;
        entry.OnSaveGUI = OnSaveGUI;

        GameModeControllerRegistrar.RegisterDefault(healthRegenController);
        GameModeControllerRegistrar.RegisterDefault(resourceRegenController);

        logger.Info("Wrath Regen Mod loaded.");
        return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry entry, bool value)
    {
        if (value)
        {
            runtime.SetModEnabled(true);
            harmony.PatchAll();
            areaHandler = new ResourceRegenAreaHandler(resourceRegenController);
            EventBus.Subscribe(areaHandler);
            logger.Info("Wrath Regen Mod enabled.");
        }
        else
        {
            runtime.SetModEnabled(false);
            healthRegenController.Deactivate();
            resourceRegenController.Deactivate();
            harmony.UnpatchAll(entry.Info.Id);
            if (areaHandler != null)
            {
                EventBus.Unsubscribe(areaHandler);
                areaHandler = null;
            }
            logger.Info("Wrath Regen Mod disabled.");
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

    private static ModLogger GetLogger()
    {
        return logger;
    }
}
