namespace Pooler.Data
{
    [System.Serializable]
    public abstract class BasePoolElementDataHandler
    {
        public abstract bool IsInPool();
        public abstract void GetFromPool();
        public abstract void ReturnToPool();
    }
}