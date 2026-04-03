using Kingmaker.PubSubSystem;

namespace WrathRegenMod;

internal sealed class ResourceRegenAreaHandler : IAreaHandler
{
    private readonly ResourceRegenController resourceRegenController;

    public ResourceRegenAreaHandler(ResourceRegenController resourceRegenController)
    {
        this.resourceRegenController = resourceRegenController;
    }

    public void OnAreaDidLoad()
    {
        resourceRegenController.ResetAllStrategies();
    }

    public void OnAreaBeginUnloading()
    {
    }
}
