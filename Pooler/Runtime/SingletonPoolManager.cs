#if USEFUL_DATA_TYPES_ENABLED

using System;
using System.Collections.Generic;
using Pooler.Config.Data;
using Pooler.Request.Data;
using UnityEngine;
using UsefulDataTypes;

namespace Pooler
{
    public class SingletonPoolManager : SingletonMonoBehaviour<PoolManager>
    {
        [Serializable]
        protected class ObjectPoolsDictionary : Dictionary<Type, IPool> { }

        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        protected ObjectPoolsDictionary ObjectPools = new ObjectPoolsDictionary();

#if UNITY_EDITOR
        [SerializeField]
        bool _autoUpdateConfig = true;
#endif

        [field: SerializeField]
        public ScriptablePoolConfigContainer PoolConfigContainer { get; private set; }

        protected override void AwakeInternal()
        {
            ObjectPools = new ObjectPoolsDictionary();
            LoadFromConfig();
        }

        protected internal T1 GetPoolElementInternal<T1, T2>(T2 requestData) where T1 : class, IPoolElement, new() where T2 : BasePoolRequestData
        {
            if (!ObjectPools.ContainsKey(typeof(T1)))
            {
                if (!PoolTypeResolver.TryGetPoolTypeFromPoolElementType(typeof(T1), out Type poolType))
                    return null;

                TryAddNewPool(poolType, null);
            }

            return ObjectPools[typeof(T1)].GetPoolObj<T1, T2>(requestData);
        }

        public static T1 GetPoolElement<T1, T2>(T2 requestData)
        where T1 : class, IPoolElement, new() where T2 : BasePoolRequestData
        => Instance.GetPoolElementInternal<T1, T2>(requestData);


        protected internal bool TryGetPoolElementInternal<T1, T2>(T2 requestData, out T1 poolElement) where T1 : class, IPoolElement, new() where T2 : BasePoolRequestData
        {
            try
            {
                if (!ObjectPools.ContainsKey(typeof(T1)))
                {
                    if (!PoolTypeResolver.TryGetPoolTypeFromPoolElementType(typeof(T1), out Type poolType))
                    {
                        poolElement = null;
                        return false;
                    }

                    TryAddNewPool(poolType, null);
                }

                poolElement = ObjectPools[typeof(T1)].GetPoolObj<T1, T2>(requestData);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error while trying to retrive type : {typeof(T1)} from Pooler! Returning false! Error : {e}");
                poolElement = null;
                return false;
            }
        }

        public static bool TryGetPoolElement<T1, T2>(T2 requestData, out T1 poolElement) where T1 : class, IPoolElement, new() where T2 : BasePoolRequestData => Instance.TryGetPoolElementInternal<T1, T2>(requestData, out poolElement);

        protected internal bool TryReturnElementToPoolInternal<T>(T poolElement) where T : class, IPoolElement
        {
            if (poolElement == null)
                return false;

            try
            {
                if (!ObjectPools.ContainsKey(typeof(T)))
                {
                    Debug.LogError($"There is no pool for {poolElement} with type : {poolElement.GetType()}! Cannot return to pool...");
                    return false;
                }

                return ObjectPools[typeof(T)].ReturnPoolObj(poolElement);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error while trying to return to pool with element {poolElement} of type : {typeof(T)} from Pooler! Error : {e}");
                poolElement = null;
                return false;
            }
        }

        public static bool TryReturnElementToPool<T>(T poolElement) where T : class, IPoolElement => Instance.TryReturnElementToPoolInternal(poolElement);

        /// <summary>
        /// Tries to add a new pool by type and config data.
        /// </summary>
        /// <param name="poolType"></param>
        /// <param name="configData"></param>
        /// <param name="updateConfigData">ONLY USED FOR EDITOR</param>
        /// <returns></returns>
        /// 
        protected internal bool TryAddNewPoolInternal(Type poolType, BasePoolConfigData configData = null, bool updateConfigData = true)
        {
            try
            {
                if (!typeof(IPool).IsAssignableFrom(poolType))
                    return false;

                Type genericType = poolType.GetGenericArguments()[0];

                if (ObjectPools.ContainsKey(genericType))
                    return false;

                IPool pool = Activator.CreateInstance(poolType) as IPool;
                ObjectPools.Add(genericType, pool);
                pool.Initialize(configData);

#if UNITY_EDITOR
                if (_autoUpdateConfig)
                {
                    pool.OnCapacityChange += UpdateConfigData;

                    if (updateConfigData)
                    {
                        UpdateConfigData(pool);
                    }
                }
# endif

                return true;
            }
            catch (Exception e)
            {

#if LOGGER_ENABLED
                UnityLogger.LogErrorWithTag($"Error while trying to add new pool with type : {poolType}! Error : {e}");
#else
                Debug.LogError($"Error while trying to add new pool with type : {poolType}! Error : {e}");
#endif
                return false;
            }
        }


        /// <summary>
        /// Tries to add a new pool by type and config data.
        /// </summary>
        /// <param name="poolType"></param>
        /// <param name="configData"></param>
        /// <param name="updateConfigData">ONLY USED FOR EDITOR</param>
        /// <returns></returns>
        public static bool TryAddNewPool(Type poolType, BasePoolConfigData configData = null, bool updateConfigData = true) => Instance.TryAddNewPoolInternal(poolType, configData, updateConfigData);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="configData"></param>
        /// <param name="updateConfigData">ONLY USED FOR EDITOR</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryAddNewPool<T>(BasePoolConfigData configData = null, bool updateConfigData = true) where T : IPool => TryAddNewPool(typeof(T), configData, updateConfigData);

        void LoadFromConfig()
        {
            if (PoolConfigContainer == null)
            {

#if LOGGER_ENABLED
                UnityLogger.LogErrorWithTag($"No {nameof(PoolConfigContainer)} is attached! Cannot load from on on {GetType()}");
#else
                Debug.LogError($"No {nameof(PoolConfigContainer)} is attached! Cannot load from on on {this.GetType()}");
#endif
                return;
            }

            foreach (var typeToConfig in PoolConfigContainer.TypeToConfigDatas)
            {
                TryAddNewPool(typeToConfig.Key, typeToConfig.Value, false);
            }
        }

#if UNITY_EDITOR
        static void UpdateConfigData(IPool pool)
        {
            Instance.PoolConfigContainer?.AddOrUpdateConfigForType(pool, pool.GetPoolAsConfigData());
        }

        static void UpdateConfigData(IPool pool, BasePoolConfigData data)
        {
            Instance.PoolConfigContainer?.AddOrUpdateConfigForType(pool, data);
        }
# endif
    }
}

#endif