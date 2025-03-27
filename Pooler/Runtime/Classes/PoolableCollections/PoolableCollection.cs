using System;
using Pooler.Data;

namespace Pooler
{
    [Serializable]
    public abstract class PoolableCollection<T> : IPoolElementWithCapacity
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        protected BasicPoolElementDataHandler _poolHandler = new();

        public IPoolElementDataHandler GetPoolDataHandler() => _poolHandler;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public T Collection;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
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
