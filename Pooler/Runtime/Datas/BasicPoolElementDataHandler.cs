namespace Pooler.Data
{
    [System.Serializable]
    public class BasicPoolElementDataHandler : BasePoolElementDataHandler
    {
        [Sirenix.OdinInspector.ShowInInspector]
        protected bool _isInPool = true;
        public System.Action OnGetFromPool;
        public System.Action OnReturnToPool;
        public override bool IsInPool() => _isInPool;

        public override void GetFromPool()
        {
            _isInPool = false;
            OnGetFromPool?.Invoke();
        }

        public override void ReturnToPool()
        {
            _isInPool = true;
            OnReturnToPool?.Invoke();
        }
    }
}