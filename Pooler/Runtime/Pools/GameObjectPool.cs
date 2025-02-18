using System.Collections.Generic;
using UnityEngine;
using Pooler.Config.Data;
using Pooler.Request.Data;
using System;
using System.Collections;

namespace Pooler
{
    [Obsolete("Do not use, will be refactored later", true)]
    internal class GameObjectPool<T> : IGameObjectPool where T : class, IPoolElement, new()
    {
        public List<IPoolElement> PoolElements = new List<IPoolElement>();

        public int CreateNewPoolElement()
        {
            T temp = new T();
            PoolElements.Add(temp as IPoolElement);
            return PoolElements.Count - 1;
        }

        public T1 GetPoolObj<T1, T2>(T2 requestData)
           where T1 : class, IPoolElement
           where T2 : BasePoolRequestData
        {
            for (int i = 0; i < PoolElements.Count; i++)
            {
                if (PoolElements[i].GetPoolDataHandler().IsInPool())
                {
                    PoolElements[i].GetPoolDataHandler().GetFromPool();
                    return PoolElements[i] as T1;
                }
            }

            int Index = CreateNewPoolElement();
            PoolElements[Index].GetPoolDataHandler().GetFromPool();
            return PoolElements[Index] as T1;
        }

        public int CreateNewPoolElement(GameObject Prefab)
        {
            GameObject gameObject = MonoBehaviour.Instantiate(Prefab, Vector3.zero, Quaternion.identity, PoolManager.Instance.transform);
            PoolElements.Add(gameObject.GetComponent<IPoolElement>());
            return PoolElements.Count - 1;
        }

        public bool ReturnPoolObj<T1>(T1 poolElement) where T1 : IPoolElement
        {
            try
            {
                poolElement.GetPoolDataHandler().ReturnToPool();
                return true;
            }
            catch (System.Exception e)
            {
                UnityLogger.LogErrorWithTag($"Error while trying to return : {poolElement} to pool :{this.GetType()}! Error : {e}");
                return false;
            }
        }
        public Action<IPool> OnCapacityChange { get; set; }

        T IGameObjectPool.GetPoolObj<T>(GameObject Prefab)
        {
            for (int i = 0; i < PoolElements.Count; i++)
            {
                if (PoolElements[i].GetPoolDataHandler().IsInPool())
                {
                    PoolElements[i].GetPoolDataHandler().GetFromPool();
                    return PoolElements[i] as T;
                }
            }

            int Index = CreateNewPoolElement(Prefab);
            PoolElements[Index].GetPoolDataHandler().GetFromPool();
            return PoolElements[Index] as T;
        }

        public Type GetConfigDataType() => typeof(GameObjectPoolConfigData);

        public Type GetRequestDataType() => typeof(GameObjectPoolRequestData);

        public bool Initialize(BasePoolConfigData configData)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetPoolEnumerator()
        {
            throw new NotImplementedException();
        }

        public BasePoolConfigData GetPoolAsConfigData()
        {
            throw new NotImplementedException();
        }
    }

}
