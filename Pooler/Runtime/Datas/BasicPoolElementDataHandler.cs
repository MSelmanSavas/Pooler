namespace Pooler.Data
{
    [System.Serializable]
    public class BasicPoolElementDataHandler : IPoolElementDataHandler
    {
        protected bool isInPool = true;
        
        public System.Action OnGetFromPool;
        public System.Action OnReturnToPool;

        public bool IsInPool() => isInPool;

        public void GetFromPool()
        {
            isInPool = false;
            OnGetFromPool?.Invoke();
        }

        public void ReturnToPool()
        {
            isInPool = true;
            OnReturnToPool?.Invoke();
        }
    }
}