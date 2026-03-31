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

        UnityEngine.GUILayout.Space(8f);
        UnityEngine.GUILayout.Label("Diagnostics");

        settings.EnablePartyDiagnostics = UnityEngine.GUILayout.Toggle(
            settings.EnablePartyDiagnostics,
            "Enable party diagnostics probe");

        settings.LogVerbose = UnityEngine.GUILayout.Toggle(
            settings.LogVerbose,
            "Verbose logging");

        settings.MirrorModLogsToGameLog = UnityEngine.GUILayout.Toggle(
            settings.MirrorModLogsToGameLog,
            "Mirror mod messages to the in-game event log");

        UnityEngine.GUILayout.Space(8f);
        UnityEngine.GUILayout.Label("Health Prototype");

        settings.EnableHealthRegenPrototype = UnityEngine.GUILayout.Toggle(
            settings.EnableHealthRegenPrototype,
            "Enable health regeneration prototype");

        settings.OnlyRegenOutOfCombat = UnityEngine.GUILayout.Toggle(
            settings.OnlyRegenOutOfCombat,
            "Only regenerate out of combat");

        settings.IncludePetsInHealthRegen = UnityEngine.GUILayout.Toggle(
            settings.IncludePetsInHealthRegen,
            "Include pets in health regeneration");

        settings.IncludeSummonsInHealthRegen = UnityEngine.GUILayout.Toggle(
            settings.IncludeSummonsInHealthRegen,
            "Include summons in health regeneration");

        settings.ShowHealingInGameLog = UnityEngine.GUILayout.Toggle(
            settings.ShowHealingInGameLog,
            "Show prototype healing in the in-game event log");

        UnityEngine.GUILayout.Label($"Tick interval: {settings.TickIntervalSeconds:0.0} seconds");
        settings.TickIntervalSeconds = (float)System.Math.Round(
            UnityEngine.GUILayout.HorizontalSlider(settings.TickIntervalSeconds, 1f, 30f),
            1);

        UnityEngine.GUILayout.Label($"Health restored per tick: {settings.HealthRegenAmountPerTick}");
        settings.HealthRegenAmountPerTick = UnityEngine.Mathf.RoundToInt(
            UnityEngine.GUILayout.HorizontalSlider(settings.HealthRegenAmountPerTick, 1f, 10f));

        UnityEngine.GUILayout.Space(8f);
        UnityEngine.GUILayout.Label("Health regen uses Wrath's built-in healing and damage rules. Living units receive healing; undead use the negative-energy path.");
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

        var logger = new ModLogger(entry, settings);
        PartyProbeController.Tick(logger, settings, deltaTime);
        HealthRegenController.Tick(logger, settings, deltaTime);
    }
}
