using System;
using System.Globalization;
using UnityEngine;

namespace WrathRegenMod;

internal static class SettingsRenderer
{
    private enum SettingsTab
    {
        General,
        HealthRegen,
        ResourceRegen
    }

    private static readonly string[] TabLabels =
    {
        "General",
        "Health Regen",
        "Resource Regen"
    };

    private static readonly string[] LogLevelLabels =
    {
        "None",
        "Error",
        "Info",
        "Verbose"
    };

    private static readonly string[] ResourceRegenVisualEffectLabels =
    {
        "Divine Refresh",
        "Arcane Refresh"
    };

    private static SettingsTab currentTab;
    private static string healthTickIntervalText;
    private static string healthPerTickText;
    private static string resourceTickIntervalText;
    private static string resourceLevel1IntervalText;
    private static string resourceLevel2IntervalText;
    private static string resourceLevel3IntervalText;
    private static string resourceLevel4IntervalText;
    private static string resourceLevel5IntervalText;
    private static string resourceLevel6IntervalText;
    private static string resourceLevel7IntervalText;
    private static string resourceLevel8IntervalText;
    private static string resourceLevel9IntervalText;
    private static string resourceLevel10IntervalText;
    private static string resourceGenericRestoreAmountText;
    private static string resourceGenericTier1IntervalText;
    private static string resourceGenericTier2IntervalText;
    private static string resourceGenericTier3IntervalText;
    private static string resourceGenericTier4IntervalText;
    private static string resourceGenericTier5IntervalText;
    private static string resourceGenericTier6IntervalText;
    private static string kineticistBurnRestoreIntervalText;
    private static string kineticistBurnFloorText;

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
            case SettingsTab.HealthRegen:
                DrawHealthRegenTab(settings);
                break;
            case SettingsTab.ResourceRegen:
                DrawResourceRegenTab(settings);
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

    private static void DrawResourceRegenTab(ModSettings settings)
    {
        Section("Resource Prototype");

        settings.ResourceRegen.Enabled = Toggle(
            settings.ResourceRegen.Enabled,
            "Enable resource regeneration prototype",
            "Turns on the first resource regeneration controller. The current implementation targets spontaneous spellbooks like Oracle.");

        settings.ResourceRegen.OnlyRegenOutOfCombat = Toggle(
            settings.ResourceRegen.OnlyRegenOutOfCombat,
            "Only regenerate out of combat",
            "Prevents spell-slot restoration while the player party is flagged as being in combat.");

        settings.ResourceRegen.EnableSpontaneousSpellbookRegen = Toggle(
            settings.ResourceRegen.EnableSpontaneousSpellbookRegen,
            "Enable spontaneous spellbook regeneration",
            "Restores one spontaneous spell slot at configured levels when the timer for that level completes.");

        settings.ResourceRegen.EnablePreparedSpellbookRegen = Toggle(
            settings.ResourceRegen.EnablePreparedSpellbookRegen,
            "Enable prepared spellbook regeneration",
            "Restores one spent prepared slot at configured levels when the timer for that level completes. The current policy restores slots in order.");

        settings.ResourceRegen.EnableGenericAbilityResourceRegen = Toggle(
            settings.ResourceRegen.EnableGenericAbilityResourceRegen,
            "Enable generic ability resource regeneration",
            "Restores one generic resource charge when the timer for that resource tier completes. Tiers are assigned from the resource's runtime max amount.");

        settings.ResourceRegen.ShowVisualEffects = Toggle(
            settings.ResourceRegen.ShowVisualEffects,
            "Show visual effects when resources regenerate",
            "Plays a short built-in visual effect on the unit after a spell slot is restored. This applies only to resource regeneration, not health regeneration.");

        settings.ResourceRegen.VisualEffectStyle = ResourceRegenVisualEffectSelection(
            "Visual effect style",
            settings.ResourceRegen.VisualEffectStyle,
            ResourceRegenVisualEffectLabels,
            "Chooses which built-in spell-style effect to reuse for resource regeneration. Divine Refresh is the recommended subtle default.");

        settings.ResourceRegen.TickIntervalSeconds = FloatField(
            "Controller tick interval (seconds)",
            resourceTickIntervalText,
            value => resourceTickIntervalText = value,
            settings.ResourceRegen.TickIntervalSeconds,
            0.1f,
            60f,
            "How often the resource controller scans party units. Allowed range: 0.1 to 60 seconds.");

        DrawSpellLevelIntervalField(
            settings,
            1,
            resourceLevel1IntervalText,
            value => resourceLevel1IntervalText = value,
            settings.ResourceRegen.Level1IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            2,
            resourceLevel2IntervalText,
            value => resourceLevel2IntervalText = value,
            settings.ResourceRegen.Level2IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            3,
            resourceLevel3IntervalText,
            value => resourceLevel3IntervalText = value,
            settings.ResourceRegen.Level3IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            4,
            resourceLevel4IntervalText,
            value => resourceLevel4IntervalText = value,
            settings.ResourceRegen.Level4IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            5,
            resourceLevel5IntervalText,
            value => resourceLevel5IntervalText = value,
            settings.ResourceRegen.Level5IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            6,
            resourceLevel6IntervalText,
            value => resourceLevel6IntervalText = value,
            settings.ResourceRegen.Level6IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            7,
            resourceLevel7IntervalText,
            value => resourceLevel7IntervalText = value,
            settings.ResourceRegen.Level7IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            8,
            resourceLevel8IntervalText,
            value => resourceLevel8IntervalText = value,
            settings.ResourceRegen.Level8IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            9,
            resourceLevel9IntervalText,
            value => resourceLevel9IntervalText = value,
            settings.ResourceRegen.Level9IntervalSeconds);
        DrawSpellLevelIntervalField(
            settings,
            10,
            resourceLevel10IntervalText,
            value => resourceLevel10IntervalText = value,
            settings.ResourceRegen.Level10IntervalSeconds);

        GUILayout.Space(8f);
        Section("Generic Ability Resources");

        settings.ResourceRegen.GenericResourceRestoreAmount = IntField(
            "Generic resource restored per tick",
            resourceGenericRestoreAmountText,
            value => resourceGenericRestoreAmountText = value,
            settings.ResourceRegen.GenericResourceRestoreAmount,
            1,
            100,
            "How many generic resource charges to restore whenever a resource tier timer completes.");

        DrawGenericResourceTierField(
            "Tier 1 interval (max uses 10+)",
            resourceGenericTier1IntervalText,
            value => resourceGenericTier1IntervalText = value,
            settings.ResourceRegen.GenericTier1IntervalSeconds,
            updatedValue => settings.ResourceRegen.GenericTier1IntervalSeconds = updatedValue);
        DrawGenericResourceTierField(
            "Tier 2 interval (max uses 8-9)",
            resourceGenericTier2IntervalText,
            value => resourceGenericTier2IntervalText = value,
            settings.ResourceRegen.GenericTier2IntervalSeconds,
            updatedValue => settings.ResourceRegen.GenericTier2IntervalSeconds = updatedValue);
        DrawGenericResourceTierField(
            "Tier 3 interval (max uses 6-7)",
            resourceGenericTier3IntervalText,
            value => resourceGenericTier3IntervalText = value,
            settings.ResourceRegen.GenericTier3IntervalSeconds,
            updatedValue => settings.ResourceRegen.GenericTier3IntervalSeconds = updatedValue);
        DrawGenericResourceTierField(
            "Tier 4 interval (max uses 4-5)",
            resourceGenericTier4IntervalText,
            value => resourceGenericTier4IntervalText = value,
            settings.ResourceRegen.GenericTier4IntervalSeconds,
            updatedValue => settings.ResourceRegen.GenericTier4IntervalSeconds = updatedValue);
        DrawGenericResourceTierField(
            "Tier 5 interval (max uses 2-3)",
            resourceGenericTier5IntervalText,
            value => resourceGenericTier5IntervalText = value,
            settings.ResourceRegen.GenericTier5IntervalSeconds,
            updatedValue => settings.ResourceRegen.GenericTier5IntervalSeconds = updatedValue);
        DrawGenericResourceTierField(
            "Tier 6 interval (max uses 1)",
            resourceGenericTier6IntervalText,
            value => resourceGenericTier6IntervalText = value,
            settings.ResourceRegen.GenericTier6IntervalSeconds,
            updatedValue => settings.ResourceRegen.GenericTier6IntervalSeconds = updatedValue);

        GUILayout.Space(8f);
        Section("Kineticist Burn");

        settings.ResourceRegen.EnableKineticistBurnRegen = Toggle(
            settings.ResourceRegen.EnableKineticistBurnRegen,
            "Enable Kineticist burn regeneration",
            "Restores Kineticist burn through the dedicated HealBurn path. This is separate from generic ability resources.");

        settings.ResourceRegen.KineticistBurnRestoreIntervalSeconds = FloatField(
            "Seconds per 1 burn restored",
            kineticistBurnRestoreIntervalText,
            value => kineticistBurnRestoreIntervalText = value,
            settings.ResourceRegen.KineticistBurnRestoreIntervalSeconds,
            0f,
            3600f,
            "How long it takes to heal 1 point of accepted Kineticist burn. Set to 0 to disable the Kineticist burn strategy.");

        settings.ResourceRegen.KineticistBurnFloor = IntField(
            "Restore burn until floor",
            kineticistBurnFloorText,
            value => kineticistBurnFloorText = value,
            settings.ResourceRegen.KineticistBurnFloor,
            0,
            100,
            "Kineticist burn regeneration stops once the unit reaches this accepted burn floor.");
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
            var toolbarIndex = (int)value + 1;
            var selected = GUILayout.Toolbar(toolbarIndex, labels, GUILayout.MinWidth(240f));
            return (LogLevel)(selected - 1);
        }
    }

    private static ResourceRegenVisualEffectStyle ResourceRegenVisualEffectSelection(
        string label,
        ResourceRegenVisualEffectStyle value,
        string[] labels,
        string tooltip)
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(new GUIContent(label, tooltip), GUILayout.Width(240f));
            return (ResourceRegenVisualEffectStyle)GUILayout.Toolbar((int)value, labels, GUILayout.MinWidth(240f));
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

    private static void DrawSpellLevelIntervalField(
        ModSettings settings,
        int spellLevel,
        string textValue,
        Action<string> setTextValue,
        float currentValue)
    {
        var updatedValue = FloatField(
            $"Level {spellLevel} restore interval (seconds)",
            textValue,
            setTextValue,
            currentValue,
            0f,
            3600f,
            $"How long it takes to restore one level {spellLevel} spontaneous slot. Set to 0 to disable regeneration at this spell level.");

        switch (spellLevel)
        {
            case 1:
                settings.ResourceRegen.Level1IntervalSeconds = updatedValue;
                break;
            case 2:
                settings.ResourceRegen.Level2IntervalSeconds = updatedValue;
                break;
            case 3:
                settings.ResourceRegen.Level3IntervalSeconds = updatedValue;
                break;
            case 4:
                settings.ResourceRegen.Level4IntervalSeconds = updatedValue;
                break;
            case 5:
                settings.ResourceRegen.Level5IntervalSeconds = updatedValue;
                break;
            case 6:
                settings.ResourceRegen.Level6IntervalSeconds = updatedValue;
                break;
            case 7:
                settings.ResourceRegen.Level7IntervalSeconds = updatedValue;
                break;
            case 8:
                settings.ResourceRegen.Level8IntervalSeconds = updatedValue;
                break;
            case 9:
                settings.ResourceRegen.Level9IntervalSeconds = updatedValue;
                break;
            case 10:
                settings.ResourceRegen.Level10IntervalSeconds = updatedValue;
                break;
        }
    }

    private static void DrawGenericResourceTierField(
        string label,
        string textValue,
        Action<string> setTextValue,
        float currentValue,
        Action<float> applyValue)
    {
        var updatedValue = FloatField(
            label,
            textValue,
            setTextValue,
            currentValue,
            0f,
            3600f,
            "How long it takes to restore one generic resource charge for pools that fall into this max-usage tier. Set to 0 to disable this tier.");

        applyValue(updatedValue);
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
            case SettingsTab.HealthRegen:
                return "Health Regen contains the current gameplay prototype. This is the tab that controls party, pet, summon, and undead handling.";
            case SettingsTab.ResourceRegen:
                return "Resource Regen contains the spell and ability prototype. It currently supports spontaneous spellbooks, ordered prepared-slot restoration, generic ability-resource regeneration, and fixed-floor Kineticist burn healing.";
            default:
                return string.Empty;
        }
    }

    private static void EnsureTextCache(ModSettings settings)
    {
        healthTickIntervalText ??= settings.HealthRegen.TickIntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        healthPerTickText ??= settings.HealthRegen.HealthPerTick.ToString(CultureInfo.InvariantCulture);
        resourceTickIntervalText ??= settings.ResourceRegen.TickIntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel1IntervalText ??= settings.ResourceRegen.Level1IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel2IntervalText ??= settings.ResourceRegen.Level2IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel3IntervalText ??= settings.ResourceRegen.Level3IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel4IntervalText ??= settings.ResourceRegen.Level4IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel5IntervalText ??= settings.ResourceRegen.Level5IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel6IntervalText ??= settings.ResourceRegen.Level6IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel7IntervalText ??= settings.ResourceRegen.Level7IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel8IntervalText ??= settings.ResourceRegen.Level8IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel9IntervalText ??= settings.ResourceRegen.Level9IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceLevel10IntervalText ??= settings.ResourceRegen.Level10IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceGenericRestoreAmountText ??= settings.ResourceRegen.GenericResourceRestoreAmount.ToString(CultureInfo.InvariantCulture);
        resourceGenericTier1IntervalText ??= settings.ResourceRegen.GenericTier1IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceGenericTier2IntervalText ??= settings.ResourceRegen.GenericTier2IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceGenericTier3IntervalText ??= settings.ResourceRegen.GenericTier3IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceGenericTier4IntervalText ??= settings.ResourceRegen.GenericTier4IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceGenericTier5IntervalText ??= settings.ResourceRegen.GenericTier5IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        resourceGenericTier6IntervalText ??= settings.ResourceRegen.GenericTier6IntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        kineticistBurnRestoreIntervalText ??= settings.ResourceRegen.KineticistBurnRestoreIntervalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        kineticistBurnFloorText ??= settings.ResourceRegen.KineticistBurnFloor.ToString(CultureInfo.InvariantCulture);
    }
}
