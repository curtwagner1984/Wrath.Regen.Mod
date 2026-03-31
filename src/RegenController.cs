using UnityModManagerNet;

namespace WrathRegenMod;

internal static class RegenController
{
    private static float elapsedSeconds;
    private static bool loggedReadyMessage;

    public static void Tick(UnityModManager.ModEntry entry, ModSettings settings, float deltaTime)
    {
        if (!loggedReadyMessage)
        {
            entry.Logger.Log("RegenController is running. Next step: replace this stub with real party/resource logic.");
            loggedReadyMessage = true;
        }

        elapsedSeconds += deltaTime;
        if (elapsedSeconds < settings.TickIntervalSeconds)
        {
            return;
        }

        elapsedSeconds = 0f;

        if (settings.LogVerbose)
        {
            entry.Logger.Log("Tick fired. Hook game state inspection here.");
        }
    }
}
