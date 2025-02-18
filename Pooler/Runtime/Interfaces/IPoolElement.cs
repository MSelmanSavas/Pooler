using Pooler.Data;

namespace Pooler
{
    public interface IPoolElement
    {
        public BasePoolElementDataHandler GetPoolDataHandler();
    }
}
