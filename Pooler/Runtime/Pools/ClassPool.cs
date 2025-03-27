using System;
using System.Collections;
using System.Collections.Generic;
using Pooler.Config.Data;
using Pooler.Request.Data;
using UnityEngine;

namespace Pooler
{
    internal class ClassPool<T> : IPool where T : class, IPoolElement, new()
    {
        ClassPoolConfigData _configData;

        ClassPoolRequestData _requestdata;

        public List<IPoolElement> PoolElements = new List<IPoolElement>();
        public Action<IPool> OnCapacityChange { get; set; }
        public IEnumerator GetPoolEnumerator() => PoolElements.GetEnumerator();

        public int CreateNewPoolElement()
        {
            T temp = new T();
            PoolElements.Add(temp);
            return PoolElements.Count - 1;
        }

        public T1 GetPoolObj<T1, T2>(T2 requestData)
           where T1 : class, IPoolElement
           where T2 : BasePoolRequestData
        {
            if (requestData == null || !CheckRequestDataType(requestData))
            {
#if LOGGER_ENABLED
                UnityLogger.LogWarningWithTag($"Given Request Data Type : {requestData.GetType()} does not matches with required Request Data Type : {GetRequestDataType()} on pool : {GetType()}! Using default request parameters...");
#else
                Debug.LogWarning($"Given Request Data Type : {requestData.GetType()} does not matches with required Request Data Type : {GetRequestDataType()} on pool : {GetType()}! Using default request parameters...");
#endif
            }

            _requestdata = requestData as ClassPoolRequestData;

            for (int i = 0; i < PoolElements.Count; i++)
            {
                if (PoolElements[i].GetPoolDataHandler().IsInPool())
                {
                    PoolElements[i].GetPoolDataHandler().GetFromPool();
                    return PoolElements[i] as T1;
                }
            }

            int index = CreateNewPoolElement();
            PoolElements[index].GetPoolDataHandler().GetFromPool();
            OnCapacityChange?.Invoke(this);
            return PoolElements[index] as T1;
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
#if LOGGER_ENABLED
                UnityLogger.LogErrorWithTag($"Error while trying to return : {poolElement} to pool :{this.GetType()}! Error : {e}");
#else
                Debug.LogError($"Error while trying to return : {poolElement} to pool :{this.GetType()}! Error : {e}");
#endif
                return false;
            }
        }

        public Type GetConfigDataType() => typeof(ClassPoolConfigData);

        public Type GetRequestDataType() => typeof(ClassPoolRequestData);

        public bool Initialize()
        {
            _configData = ClassPoolConfigData.s_DefaultData;
            return Initialize(_configData);
        }

        public bool Initialize(BasePoolConfigData configData)
        {
            if (configData == null || configData.GetType() != GetConfigDataType())
            {
#if LOGGER_ENABLED
                UnityLogger.LogWarningWithTag($"Given Config Data Type : {configData?.GetType()} does not matches with required Config Data Type : {GetConfigDataType()} on pool : {GetType()}! Using default config parameters...");
#else
                Debug.LogWarning($"Given Config Data Type : {configData?.GetType()} does not matches with required Config Data Type : {GetConfigDataType()} on pool : {GetType()}! Using default config parameters...");
#endif

                _configData = ClassPoolConfigData.s_DefaultData;
            }
            else
                _configData = configData as ClassPoolConfigData;

            PoolElements.Clear();

            for (int i = 0; i < _configData.Capacity; i++)
                PoolElements.Add(new T());

            return true;
        }

        public BasePoolConfigData GetPoolAsConfigData()
        {
            ClassPoolConfigData classPoolConfigData = new()
            {
                Capacity = PoolElements.Count
            };

            return classPoolConfigData;
        }

        public bool CheckRequestDataType(object requestData)
        {
            if(requestData.GetType() != GetRequestDataType())
            {
                return false;
            }

            return true;
        }
    }

}
