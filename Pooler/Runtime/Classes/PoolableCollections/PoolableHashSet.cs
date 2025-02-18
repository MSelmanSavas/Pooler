using System;
using System.Collections.Generic;
using Pooler;

[Serializable]
public class PoolableHashSet<T> : PoolableCollection<HashSet<T>>
{
    int _capacity = 0;

    public PoolableHashSet()
    {
        Collection = new();
        _poolHandler.OnReturnToPool += ClearCollection;
    }
    public PoolableHashSet(int capacity) : base(capacity)
    {
        Collection = new(capacity);
        _capacity = capacity;
        _poolHandler.OnReturnToPool += ClearCollection;
    }

    [Sirenix.OdinInspector.ShowInInspector]
    public override int GetCapacity() => _capacity;

    public override void SetCapacity(int Capacity)
    {
        int oldCapacity = _capacity;
        _capacity = Collection.EnsureCapacity(Capacity);
        _onCapacityChanged?.Invoke(this, oldCapacity, _capacity);
    }

    public void ClearCollection()
    {
        Collection.Clear();
    }

    public bool Add(T item)
    {
        if (Collection.Contains(item))
            return false;

        if (Collection.Count >= GetCapacity())
        {
            int oldCapacity = GetCapacity();
            SetCapacity(oldCapacity * 2);
            Collection.Add(item);
            _onCapacityChanged?.Invoke(this, oldCapacity, GetCapacity());
            return true;
        }

        return Collection.Add(item);
    }

    public bool Remove(T item) => Collection.Remove(item);
}