using DefaultEcs;
using DefaultEcs.Serialization;
using DefaultEcs.System;
using PropertyChanged;

namespace FNAInvaders.Editor;

[AddINotifyPropertyChangedInterface]
public class EditorViewModel<T>
{
    public EditorViewModel(ISystem<T> updateSystem, ISystem<T> drawSystem)
    {
        var systems = new SystemViewModel<T>[2];
        systems[0] = new SystemViewModel<T>(updateSystem, "UpdateSystemGroup");
        systems[1] = new SystemViewModel<T>(drawSystem, "DrawSystemGroup");
        Systems = systems;
    }

    public IEnumerable<SystemViewModel<T>> Systems { get; }

    public SystemViewModel<T> SelectedSystem { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class SystemViewModel<T>
{
    private readonly ISystem<T> _system;
    public SystemViewModel(ISystem<T> system, string name = null)
    {
        _system = system;

        Name = name ?? system.GetType().FriendlyName();

        if (system is IEnumerable<ISystem<T>> enumerable)
        {
            Children = enumerable.Select(s => new SystemViewModel<T>(s)).ToArray();
        }

        if (system is AEntitySetSystem<T> entitySystem)
        {
            entitySystem.Set.EntityAdded += (in Entity _) => RefreshEntities(entitySystem.Set);
            entitySystem.Set.EntityRemoved += (in Entity _) => RefreshEntities(entitySystem.Set);
            RefreshEntities(entitySystem.Set);
        }
    }

    public string Name { get; }
    public IEnumerable<SystemViewModel<T>> Children { get; }
    public IEnumerable<EntityViewModel> Entities { get; set; }
    public EntityViewModel SelectedEntity { get; set; }

    public bool Enabled
    {
        get => _system.IsEnabled;
        set => _system.IsEnabled = value;
    }

    private void RefreshEntities(EntitySet entitySet)
    {
        var entities = entitySet.GetEntities();
        var viewModels = new EntityViewModel[entitySet.Count];
        for (var i = 0; i < entitySet.Count; i++)
        {
            viewModels[i] = new(entities[i]);
        }
        Entities = viewModels;
    }
}

[AddINotifyPropertyChangedInterface]
public class EntityViewModel : IComponentReader
{
    private readonly List<ComponentViewModel> _components = new();

    public EntityViewModel(Entity entity)
    {
        Entity = entity;
        Entity.ReadAllComponents(this);
    }

    public Entity Entity { get; }
    public string Name => Entity.ToString();
    public IEnumerable<ComponentViewModel> Components => _components;
    public bool Enabled
    {
        get => Entity.IsEnabled();
        set
        {
            if (value) Entity.Enable();
            else Entity.Disable();
        }
    }

    public void OnRead<T>(in T component, in Entity componentOwner) =>
        _components.Add(new ComponentViewModel
        {
            Name = component.GetType().FriendlyName()
        });
}

[AddINotifyPropertyChangedInterface]
public class ComponentViewModel
{
    public string Name { get; set; }
}
