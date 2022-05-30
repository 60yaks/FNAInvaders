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
    public IEnumerable<EntityViewModel> Entities { get; }
}

[AddINotifyPropertyChangedInterface]
public class SystemViewModel<T>
{
    private readonly ISystem<T> _system;
    public SystemViewModel(ISystem<T> system, string name = null)
    {
        _system = system;

        Name = name ?? GetFriendlyName(system.GetType());

        if (system is IEnumerable<ISystem<T>> enumerable)
        {
            Children = enumerable.Select(s => new SystemViewModel<T>(s)).ToArray();
        }
    }

    public string Name { get; }

    public bool Enabled
    {
        get => _system.IsEnabled;
        set => _system.IsEnabled = value;
    }

    public IEnumerable<SystemViewModel<T>> Children { get; }

    private static string GetFriendlyName(Type type)
    {
        if (type == typeof(int))
            return "int";
        else if (type == typeof(short))
            return "short";
        else if (type == typeof(byte))
            return "byte";
        else if (type == typeof(bool))
            return "bool";
        else if (type == typeof(long))
            return "long";
        else if (type == typeof(float))
            return "float";
        else if (type == typeof(double))
            return "double";
        else if (type == typeof(decimal))
            return "decimal";
        else if (type == typeof(string))
            return "string";
        else if (type.IsGenericType)
            return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x)).ToArray()) + ">";
        else
            return type.Name;
    }
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
