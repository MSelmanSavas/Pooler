using System;
using Pooler;
using UsefulDataTypes;

[Serializable]
public class PoolablePreAllocatedStack<T> : PoolableCollection<PreAllocatedStack<T>>
{
    int _capacity = 0;

    public PoolablePreAllocatedStack()
    {
        Collection = new();
        Collection.OnCapacityChange += OnCapacityChange;
        _poolHandler.OnReturnToPool += ClearCollection;
    }

    public PoolablePreAllocatedStack(int capacity) : base(capacity)
    {
        Collection = new(capacity);
        _capacity = capacity;
        Collection.OnCapacityChange += OnCapacityChange;
        _poolHandler.OnReturnToPool += ClearCollection;
    }

    [Sirenix.OdinInspector.ShowInInspector]
    public override int GetCapacity() => _capacity;
    public override void SetCapacity(int Capacity)
    {
        int oldCapacity = Collection.Capacity;
        Collection.Capacity = Capacity;
        _onCapacityChanged?.Invoke(this, oldCapacity, Collection.Capacity);
    }

    void OnCapacityChange(int oldCapacity, int newCapacity)
    {
        _onCapacityChanged?.Invoke(this, oldCapacity, Collection.Capacity);
    }

    public void ClearCollection()
    {
        Collection.Clear();
    }

    public void Push(T value) => Collection.Push(value);
    public void Pop() => Collection.Pop();
}