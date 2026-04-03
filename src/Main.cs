using HarmonyLib;
using Kingmaker.GameModes;
using Kingmaker.PubSubSystem;
using UnityModManagerNet;

namespace WrathRegenMod;

public static class Main
{
    private static UnityModManager.ModEntry _modEntry;
    private static Harmony _harmony;
    private static ModSettings _settings;
    private static SettingsSnapshot _lastSavedSnapshot;
    private static ModLogger _logger;
    private static ModRuntime _runtime;
    private static HealthRegenController _healthRegenController;
    private static ResourceRegenController _resourceRegenController;
    private static ResourceRegenAreaHandler _areaHandler;
    private static bool _controllersRegistered;

    public static bool Load(UnityModManager.ModEntry entry)
    {
        _modEntry = entry;
        _settings = UnityModManager.ModSettings.Load<ModSettings>(entry);
        _logger = new ModLogger(_modEntry, _settings);
        _runtime = new ModRuntime(_settings, _logger);
        _runtime.SetModEnabled(entry.Enabled);
        _healthRegenController = new HealthRegenController(_runtime);
        _resourceRegenController = new ResourceRegenController(_runtime);
        _harmony = new Harmony(entry.Info.Id);
        _lastSavedSnapshot = SettingsSnapshot.Capture(_settings);

        entry.OnToggle = OnToggle;
        entry.OnGUI = OnGUI;
        entry.OnSaveGUI = OnSaveGUI;

        _logger.Info("Wrath Regen Mod loaded.");
        return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry entry, bool value)
    {
        if (value)
        {
            _runtime.SetModEnabled(true);
            _harmony.PatchAll();
            TryRegisterControllers();
            _areaHandler = new ResourceRegenAreaHandler(_resourceRegenController);
            EventBus.Subscribe(_areaHandler);
            _logger.Info("Wrath Regen Mod enabled.");
        }
        else
        {
            _runtime.SetModEnabled(false);
            _healthRegenController.Deactivate();
            _resourceRegenController.Deactivate();
            _harmony.UnpatchAll(entry.Info.Id);
            if (_areaHandler != null)
            {
                EventBus.Unsubscribe(_areaHandler);
                _areaHandler = null;
            }
            _logger.Info("Wrath Regen Mod disabled.");
        }

        return true;
    }

    internal static void TryRegisterControllers()
    {
        if (_controllersRegistered)
        {
            return;
        }

        if (_healthRegenController == null || _resourceRegenController == null)
        {
            return;
        }

        if (GameModesFactory.AllControllers == null || GameModesFactory.AllControllers.Count == 0)
        {
            return;
        }

        GameModesFactory.Register(_healthRegenController, GameModeType.Default);
        GameModesFactory.Register(_resourceRegenController, GameModeType.Default);
        _controllersRegistered = true;
    }

    internal static void ResetControllerRegistration()
    {
        _controllersRegistered = false;
    }

    private static void OnGUI(UnityModManager.ModEntry entry)
    {
        SettingsRenderer.Draw(_settings);
    }

    private static void OnSaveGUI(UnityModManager.ModEntry entry)
    {
        var currentSnapshot = SettingsSnapshot.Capture(_settings);

        _settings.Save(entry);
        foreach (var change in currentSnapshot.DescribeChanges(_lastSavedSnapshot))
        {
            if (_logger.IsInfo)
            {
                _logger.Info($"Configuration changed: {change}");
            }
        }

        _lastSavedSnapshot = currentSnapshot;
    }

}
