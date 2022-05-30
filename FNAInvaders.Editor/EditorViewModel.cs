using DefaultEcs;
using DefaultEcs.Serialization;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using PropertyChanged;

namespace FNAInvaders.Editor;

[AddINotifyPropertyChangedInterface]
public record EditorViewModel(World World, ISystem<GameTime> MainSystem)
{
    public IEnumerable<ISystem<GameTime>> Systems => (IEnumerable<ISystem<GameTime>>)MainSystem;
    public IEnumerable<EntityViewModel> Entities => World.Select(e => new EntityViewModel(e));
    public EntityViewModel SelectedEntity { get; set; }
}

public class EntityViewModel : IComponentReader
{
    private readonly List<string> _components = new();

    public EntityViewModel(Entity entity)
    {
        Entity = entity;
        Entity.ReadAllComponents(this);
    }

    public Entity Entity { get; }
    public override string ToString() => Entity.ToString();
    public IEnumerable<string> Components => _components;
    public bool Enabled
    {
        get => Entity.IsEnabled();
        set
        {
            if (value) Entity.Enable();
            else Entity.Disable();
        }
    }

    public void OnRead<T>(in T component, in Entity componentOwner) => _components.Add(component.ToString());
}
