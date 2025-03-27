using Pooler.Data;

namespace Pooler
{
    public interface IPoolElement
    {
        public IPoolElementDataHandler GetPoolDataHandler();
    }
}
