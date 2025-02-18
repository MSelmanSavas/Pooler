using System.Collections;
using System.Collections.Generic;
using Pooler.Data;
using UnityEngine;

namespace Pooler
{
    public class BasePoolObject : IPoolElement
    {
        private bool _isUsable = true;

        public bool IsInPool()
        {
            return _isUsable;
        }

        public void ReturnToPool()
        {
            _isUsable = true;
        }

        public void GetFromPool()
        {
            _isUsable = false;
        }

        protected BasicPoolElementDataHandler _poolHandler = new();
        public BasePoolElementDataHandler GetPoolDataHandler() => _poolHandler;
    }
}
