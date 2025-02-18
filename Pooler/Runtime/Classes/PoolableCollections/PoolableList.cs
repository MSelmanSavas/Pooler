using System;
using System.Collections.Generic;
using Pooler;

[Serializable]
public class PoolableList<T> : PoolableCollection<List<T>>
{
    public PoolableList()
    {
        Collection = new();
        _poolHandler.OnReturnToPool += ClearCollection;
    }

    public PoolableList(int capacity) : base(capacity)
    {
        Collection = new(capacity);
        _poolHandler.OnReturnToPool += ClearCollection;
    }

    [Sirenix.OdinInspector.ShowInInspector]
    public override int GetCapacity() => Collection.Capacity;

    [Sirenix.OdinInspector.ShowInInspector]
    public override void SetCapacity(int Capacity)
    {
        int oldCapacity = Collection.Capacity;
        Collection.Capacity = Capacity;
        _onCapacityChanged?.Invoke(this, oldCapacity, Collection.Capacity);
    }

    public void ClearCollection()
    {
        Collection.Clear();
    }

    public void Add(T item)
    {
        if (Collection.Count >= Collection.Capacity)
        {
            int oldCapacity = Collection.Capacity;
            Collection.Add(item);
            _onCapacityChanged?.Invoke(this, oldCapacity, Collection.Capacity);
            return;
        }

        Collection.Add(item);
    }

    public bool Remove(T item) => Collection.Remove(item);
}