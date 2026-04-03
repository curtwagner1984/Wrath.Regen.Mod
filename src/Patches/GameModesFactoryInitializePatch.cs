using HarmonyLib;
using Kingmaker.GameModes;

namespace WrathRegenMod;

[HarmonyPatch(typeof(GameModesFactory), "Initialize")]
internal static class GameModesFactoryInitializePatch
{
    private static void Postfix()
    {
        Main.TryRegisterControllers();
    }
}

[HarmonyPatch(typeof(GameModesFactory), "Reset")]
internal static class GameModesFactoryResetPatch
{
    private static void Postfix()
    {
        Main.ResetControllerRegistration();
    }
}
