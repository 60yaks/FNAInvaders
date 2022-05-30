using System.Collections;
using DefaultEcs.System;

namespace FNAInvaders.Systems;

/// <summary>
/// Represents a collection of <see cref="ISystem{T}"/> to update sequentially.
/// </summary>
/// <typeparam name="T">The type of the object used as state to update the systems.</typeparam>
public sealed class EnumerableSequentialSystem<T> : ISystem<T>, IEnumerable<ISystem<T>>
{
    private readonly ISystem<T>[] _systems;

    /// <summary>
    /// Initialises a new instance of the <see cref="EnumerableSequentialSystem{T}"/> class.
    /// </summary>
    /// <param name="systems">The <see cref="ISystem{T}"/> instances.</param>
    /// <exception cref="ArgumentNullException"><paramref name="systems"/> is null.</exception>
    public EnumerableSequentialSystem(IEnumerable<ISystem<T>> systems)
    {
        _systems = systems.CheckArgumentNullException(nameof(systems)).Where(s => s != null).ToArray();
        IsEnabled = true;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="EnumerableSequentialSystem{T}"/> class.
    /// </summary>
    /// <param name="systems">The <see cref="ISystem{T}"/> instances.</param>
    /// <exception cref="ArgumentNullException"><paramref name="systems"/> is null.</exception>
    public EnumerableSequentialSystem(params ISystem<T>[] systems)
        : this(systems as IEnumerable<ISystem<T>>)
    { }

    /// <summary>
    /// Gets or sets whether the current <see cref="EnumerableSequentialSystem{T}"/> instance should update or not.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Updates all the systems once sequentially.
    /// </summary>
    /// <param name="state">The state to use.</param>
    public void Update(T state)
    {
        if (IsEnabled)
        {
            foreach (ISystem<T> system in _systems)
            {
                system.Update(state);
            }
        }
    }

    /// <summary>
    /// Disposes all the inner <see cref="ISystem{T}"/> instances.
    /// </summary>
    public void Dispose()
    {
        for (int i = _systems.Length - 1; i >= 0; --i)
        {
            _systems[i].Dispose();
        }
    }

    public IEnumerator<ISystem<T>> GetEnumerator()
    {
        return ((IEnumerable<ISystem<T>>)_systems).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _systems.GetEnumerator();
    }
}
