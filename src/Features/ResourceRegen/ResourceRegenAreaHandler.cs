using Kingmaker.PubSubSystem;

namespace WrathRegenMod;

internal sealed class ResourceRegenAreaHandler : IAreaHandler
{
    private readonly HealthRegenController healthRegenController;
    private readonly ResourceRegenController resourceRegenController;

    public ResourceRegenAreaHandler(HealthRegenController healthRegenController, ResourceRegenController resourceRegenController)
    {
        this.healthRegenController = healthRegenController;
        this.resourceRegenController = resourceRegenController;
    }

    public void OnAreaDidLoad()
    {
        healthRegenController.ResetState();
        resourceRegenController.ResetState();
    }

    public void OnAreaBeginUnloading()
    {
    }
}
