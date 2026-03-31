using System;
using System.Globalization;
using UnityEngine;

namespace WrathRegenMod;

internal static class SettingsRenderer
{
    private enum SettingsTab
    {
        General,
        Diagnostics,
        HealthRegen
    }

    private static readonly string[] TabLabels =
    {
        "General",
        "Diagnostics",
        "Health Regen"
    };

    private static readonly string[] LogLevelLabels =
    {
        "Error",
        "Info",
        "Verbose"
    };

    private static SettingsTab currentTab;
    private static string diagnosticsTickIntervalText;
    private static string healthTickIntervalText;
    private static string healthPerTickText;

    public static void Draw(ModSettings settings)
    {
        EnsureTextCache(settings);

        settings.General.Enabled = Toggle(
            settings.General.Enabled,
            "Enable mod",
            "Master switch for all runtime behavior in this mod.");

        GUILayout.Space(8f);
        currentTab = (SettingsTab)GUILayout.Toolbar((int)currentTab, TabLabels);
        GUILayout.Space(8f);

        switch (currentTab)
        {
            case SettingsTab.General:
                DrawGeneralTab(settings);
                break;
            case SettingsTab.Diagnostics:
                DrawDiagnosticsTab(settings);
                break;
            case SettingsTab.HealthRegen:
                DrawHealthRegenTab(settings);
                break;
        }

        DrawTooltipPanel(GetDefaultHelpText());
    }

    private static void DrawGeneralTab(ModSettings settings)
    {
        Section("General");

        settings.General.LogLevel = EnumSelection(
            "Log level",
            settings.General.LogLevel,
            LogLevelLabels,
            "Controls how much this mod writes to the log. Error shows only failures, Info shows lifecycle and setting changes, Verbose adds probe/debug noise.");

        settings.General.MirrorModLogsToGameLog = Toggle(
            settings.General.MirrorModLogsToGameLog,
            "Mirror mod messages to the in-game event log",
            "Copies this mod's own messages into Wrath's in-game event log window.");
    }

    private static void DrawDiagnosticsTab(ModSettings settings)
    {
        Section("Diagnostics");

        settings.Diagnostics.EnablePartyDiagnostics = Toggle(
            settings.Diagnostics.EnablePartyDiagnostics,
            "Enable party diagnostics probe",
            "Logs party composition, HP, and combat state at a fixed interval.");

        settings.Diagnostics.TickIntervalSeconds = FloatField(
            "Probe interval (seconds)",
            diagnosticsTickIntervalText,
            value => diagnosticsTickIntervalText = value,
            settings.Diagnostics.TickIntervalSeconds,
            0.5f,
            3600f,
            "How often the diagnostics probe runs. Allowed range: 0.5 to 3600 seconds.");
    }

    private static void DrawHealthRegenTab(ModSettings settings)
    {
        Section("Health Prototype");

        settings.HealthRegen.Enabled = Toggle(
            settings.HealthRegen.Enabled,
            "Enable health regeneration prototype",
            "Turns on the current HP regeneration prototype.");

        settings.HealthRegen.OnlyRegenOutOfCombat = Toggle(
            settings.HealthRegen.OnlyRegenOutOfCombat,
            "Only regenerate out of combat",
            "Prevents health regeneration while the party is flagged as being in combat.");

        settings.HealthRegen.IncludePets = Toggle(
            settings.HealthRegen.IncludePets,
            "Include pets",
            "Includes animal companions and other real pet units from the PartyAndPets list.");

        settings.HealthRegen.IncludeSummons = Toggle(
            settings.HealthRegen.IncludeSummons,
            "Include summons",
            "Includes active player-side summoned creatures. Disabled by default to avoid surprising behavior.");

        settings.HealthRegen.ShowHealingInGameLog = Toggle(
            settings.HealthRegen.ShowHealingInGameLog,
            "Show healing in the in-game event log",
            "Lets Wrath's own rule system write healing-related entries into the in-game event log.");

        settings.HealthRegen.TickIntervalSeconds = FloatField(
            "Regen tick interval (seconds)",
            healthTickIntervalText,
            value => healthTickIntervalText = value,
            settings.HealthRegen.TickIntervalSeconds,
            0.5f,
            3600f,
            "How often the regeneration logic runs. Allowed range: 0.5 to 3600 seconds.");

        settings.HealthRegen.HealthPerTick = IntField(
            "Health restored per tick",
            healthPerTickText,
            value => healthPerTickText = value,
            settings.HealthRegen.HealthPerTick,
            1,
            10000,
            "Flat HP restored per tick. Living targets use healing; undead use the negative-energy restoration path.");
    }

    private static bool Toggle(bool value, string label, string tooltip)
    {
        return GUILayout.Toggle(value, new GUIContent(label, tooltip));
    }

    private static LogLevel EnumSelection(string label, LogLevel value, string[] labels, string tooltip)
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent(label, tooltip), GUILayout.Width(240f));
            return (LogLevel)GUILayout.Toolbar((int)value, labels, GUILayout.MinWidth(240f));
        }
    }

    private static float FloatField(
        string label,
        string textValue,
        Action<string> setTextValue,
        float currentValue,
        float min,
        float max,
        string tooltip)
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent(label, tooltip), GUILayout.Width(240f));
            var nextText = GUILayout.TextField(textValue, GUILayout.Width(120f));
            if (!string.Equals(nextText, textValue, StringComparison.Ordinal))
            {
                setTextValue(nextText);
            }

            GUILayout.Label($"Range: {min:0.##} - {max:0.##}", GUILayout.Width(140f));
        }

        if (float.TryParse(textValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            return Mathf.Clamp(parsed, min, max);
        }

        return currentValue;
    }

    private static int IntField(
        string label,
        string textValue,
        Action<string> setTextValue,
        int currentValue,
        int min,
        int max,
        string tooltip)
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent(label, tooltip), GUILayout.Width(240f));
            var nextText = GUILayout.TextField(textValue, GUILayout.Width(120f));
            if (!string.Equals(nextText, textValue, StringComparison.Ordinal))
            {
                setTextValue(nextText);
            }

            GUILayout.Label($"Range: {min} - {max}", GUILayout.Width(140f));
        }

        if (int.TryParse(textValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return Mathf.Clamp(parsed, min, max);
        }

        return currentValue;
    }

    private static void Section(string title)
    {
        GUILayout.Label(title);
        GUILayout.Space(4f);
    }

    private static void DrawTooltipPanel(string fallbackText)
    {
        GUILayout.Space(10f);
        var tooltip = string.IsNullOrWhiteSpace(GUI.tooltip) ? fallbackText : GUI.tooltip;
        GUILayout.Label(tooltip);
    }

    private static string GetDefaultHelpText()
    {
        switch (currentTab)
        {
            case SettingsTab.General:
                return "General settings control the mod-wide behavior shared by all features, including how much the mod logs.";
            case SettingsTab.Diagnostics:
                return "Diagnostics settings control whether the party probe runs and how often it samples game state.";
            case SettingsTab.HealthRegen:
                return "Health Regen contains the current gameplay prototype. This is the tab that controls party, pet, summon, and undead handling.";
            default:
                return string.Empty;
        }
    }

    private static void EnsureTextCache(ModSettings settings)
    {
        diagnosticsTickIntervalText ??= settings.Diagnostics.TickIntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        healthTickIntervalText ??= settings.HealthRegen.TickIntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        healthPerTickText ??= settings.HealthRegen.HealthPerTick.ToString(CultureInfo.InvariantCulture);
    }
}
