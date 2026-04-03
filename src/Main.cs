using HarmonyLib;
using Kingmaker;
using Kingmaker.GameModes;
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
    private static ResourceRegenAreaHandler areaHandler;

    public static bool Load(UnityModManager.ModEntry entry)
    {
        modEntry = entry;
        settings = UnityModManager.ModSettings.Load<ModSettings>(entry);
        logger = new ModLogger(modEntry, settings);
        harmony = new Harmony(entry.Info.Id);
        lastSavedSnapshot = SettingsSnapshot.Capture(settings);

        entry.OnToggle = OnToggle;
        entry.OnGUI = OnGUI;
        entry.OnSaveGUI = OnSaveGUI;
        entry.OnUpdate = OnUpdate;

        logger.Info("Wrath Regen Mod loaded.");
        return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry entry, bool value)
    {
        if (value)
        {
            harmony.PatchAll();
            areaHandler = new ResourceRegenAreaHandler();
            EventBus.Subscribe(areaHandler);
            logger.Info("Wrath Regen Mod enabled.");
        }
        else
        {
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

    private static void OnUpdate(UnityModManager.ModEntry entry, float deltaTime)
    {
        if (!entry.Enabled || !settings.General.Enabled)
        {
            return;
        }

        if (!Game.HasInstance || Game.Instance.Player == null)
        {
            return;
        }

        if (Game.Instance.IsPaused || Game.Instance.IsFakePause)
        {
            return;
        }

        if (Game.Instance.CurrentMode != GameModeType.Default)
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
            if (logger.IsError)
                logger.Error($"Unhandled exception in update loop: {ex}");
        }
    }

    private static ModLogger GetLogger()
    {
        return logger;
    }
}
