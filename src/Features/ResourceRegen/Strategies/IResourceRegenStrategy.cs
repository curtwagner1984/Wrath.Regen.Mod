using Kingmaker.EntitySystem.Entities;

namespace WrathRegenMod;

internal interface IResourceRegenStrategy
{
    string Name { get; }

    void Tick(UnitEntityData unit, RegenTickContext context);
}
