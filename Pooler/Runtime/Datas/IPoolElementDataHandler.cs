namespace Pooler.Data
{
    public interface  IPoolElementDataHandler
    {
        public abstract bool IsInPool();
        public abstract void GetFromPool();
        public abstract void ReturnToPool();
    }
}