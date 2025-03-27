#if USEFUL_DATA_TYPES_ENABLED
using System;
using Pooler;
using UsefulDataTypes;

[Serializable]
public class PoolablePreAllocatedQueue<T> : PoolableCollection<PreAllocatedQueue<T>>
{
    int _capacity = 0;

    public PoolablePreAllocatedQueue()
    {
        Collection = new();
        Collection.OnCapacityChange += OnCapacityChange;
        _poolHandler.OnReturnToPool += ClearCollection;
    }

    public PoolablePreAllocatedQueue(int capacity) : base(capacity)
    {
        Collection = new(capacity);
        Collection.OnCapacityChange += OnCapacityChange;
        _capacity = capacity;
        _poolHandler.OnReturnToPool += ClearCollection;
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif

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


    public void Enquque(T value) => Collection.Enqueue(value);
    public void Dequeue() => Collection.Dequeue();
}
#endif
