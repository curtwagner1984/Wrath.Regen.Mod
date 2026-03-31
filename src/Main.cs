using HarmonyLib;
using UnityModManagerNet;

namespace WrathRegenMod;

public static class Main
{
    private static UnityModManager.ModEntry modEntry;
    private static Harmony harmony;
    private static ModSettings settings;

    public static bool Load(UnityModManager.ModEntry entry)
    {
        modEntry = entry;
        settings = UnityModManager.ModSettings.Load<ModSettings>(entry);
        harmony = new Harmony(entry.Info.Id);

        entry.OnToggle = OnToggle;
        entry.OnGUI = OnGUI;
        entry.OnSaveGUI = OnSaveGUI;
        entry.OnUpdate = OnUpdate;

        modEntry.Logger.Log("Wrath Regen Mod loaded.");
        return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry entry, bool value)
    {
        if (value)
        {
            harmony.PatchAll();
            modEntry.Logger.Log("Wrath Regen Mod enabled.");
        }
        else
        {
            harmony.UnpatchAll(entry.Info.Id);
            modEntry.Logger.Log("Wrath Regen Mod disabled.");
        }

        return true;
    }

    private static void OnGUI(UnityModManager.ModEntry entry)
    {
        settings.Enabled = UnityEngine.GUILayout.Toggle(
            settings.Enabled,
            "Enable experimental regeneration controller");

        settings.LogVerbose = UnityEngine.GUILayout.Toggle(
            settings.LogVerbose,
            "Verbose logging");

        UnityEngine.GUILayout.Label("This scaffold is intentionally tiny. Confirm loading first, then add game logic.");
    }

    private static void OnSaveGUI(UnityModManager.ModEntry entry)
    {
        settings.Save(entry);
    }

    private static void OnUpdate(UnityModManager.ModEntry entry, float deltaTime)
    {
        if (!entry.Enabled || !settings.Enabled)
        {
            return;
        }

        RegenController.Tick(entry, settings, deltaTime);
    }
}
