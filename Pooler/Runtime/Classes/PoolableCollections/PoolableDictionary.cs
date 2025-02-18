using System;
using System.Collections.Generic;
using Pooler;

[Serializable]
public class PoolableDictionary<T1, T2> : PoolableCollection<Dictionary<T1, T2>>
{
    int _capacity = 0;

    public PoolableDictionary()
    {
        Collection = new();
        _poolHandler.OnReturnToPool += ClearCollection;
    }
    public PoolableDictionary(int capacity) : base(capacity)
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

    public bool TryAdd(T1 key, T2 item)
    {
        if (Collection.ContainsKey(key))
            return false;

        if (Collection.Count >= GetCapacity())
        {
            int oldCapacity = GetCapacity();
            SetCapacity(oldCapacity * 2);
            Collection.Add(key, item);
            _onCapacityChanged?.Invoke(this, oldCapacity, GetCapacity());
            return true;
        }

        Collection.Add(key, item);

        return true;
    }

    public bool Remove(T1 item) => Collection.Remove(item);
}