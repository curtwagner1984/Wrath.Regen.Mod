using Kingmaker.PubSubSystem;

namespace WrathRegenMod;

internal sealed class ResourceRegenAreaHandler : IAreaHandler
{
    public void OnAreaDidLoad()
    {
        ResourceRegenController.ResetAllStrategies();
    }

    public void OnAreaBeginUnloading()
    {
    }
}
