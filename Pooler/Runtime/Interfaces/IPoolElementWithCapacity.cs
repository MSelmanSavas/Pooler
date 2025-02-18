namespace Pooler
{
    public interface IPoolElementWithCapacity : IPoolElement
    {
        void SetCapacity(int Capacity);
        int GetCapacity();
        void AddOnCapacityChanged(System.Action<IPoolElementWithCapacity, int, int> action);
        void RemoveOnCapacityChanged(System.Action<IPoolElementWithCapacity, int, int> action);
    }
}