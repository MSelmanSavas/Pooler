using System;
using System.Collections.Generic;
using Pooler.Data;
using UsefulDataTypes;

namespace Pooler
{
    [Serializable]
    public abstract class PoolableCollection<T> : IPoolElementWithCapacity
    {
        [Sirenix.OdinInspector.ShowInInspector]
        protected BasicPoolElementDataHandler _poolHandler = new();
        public BasePoolElementDataHandler GetPoolDataHandler() => _poolHandler;

        [Sirenix.OdinInspector.ShowInInspector]
        public T Collection;

        [Sirenix.OdinInspector.ShowInInspector]
        protected Action<IPoolElementWithCapacity, int, int> _onCapacityChanged;

        public PoolableCollection() { }
        public PoolableCollection(int capacity)
        {

        }

        public abstract void SetCapacity(int Capacity);

        public abstract int GetCapacity();

        public virtual void AddOnCapacityChanged(Action<IPoolElementWithCapacity, int, int> action)
        {
            _onCapacityChanged += action;
        }

        public virtual void RemoveOnCapacityChanged(Action<IPoolElementWithCapacity, int, int> action)
        {
            _onCapacityChanged = action;
        }
    }
}
