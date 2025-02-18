using UnityEngine;

namespace Pooler
{
    internal interface IGameObjectPool : IPool
    {
        T GetPoolObj<T>(GameObject Prefab) where T : MonoBehaviour;
    }
}
