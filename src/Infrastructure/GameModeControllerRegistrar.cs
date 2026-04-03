using System;
using HarmonyLib;
using Kingmaker.Controllers;
using Kingmaker.GameModes;

namespace WrathRegenMod;

internal static class GameModeControllerRegistrar
{
    private static readonly Action<IController, GameModeType[]> Register = AccessTools.MethodDelegate<Action<IController, GameModeType[]>>(
        AccessTools.Method(typeof(GameModesFactory), "Register", new[] { typeof(IController), typeof(GameModeType[]) })
        ?? throw new InvalidOperationException("Could not find GameModesFactory.Register(IController, GameModeType[])."));

    public static void RegisterDefault(IController controller)
    {
        Register(controller, new[] { GameModeType.Default });
    }
}
