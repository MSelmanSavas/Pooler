using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pooler.Config.Data;
using Pooler.Request.Data;
using UnityEngine;

namespace Pooler
{
    [Serializable]
    internal class CollectionPool<T> : IPool where T : class, IPoolElementWithCapacity, new()
    {
        CollectionPoolConfigData _configData;

        CollectionPoolRequestData _requestdata;

        Dictionary<int, List<T>> _poolElements = new();
        public IEnumerator GetPoolEnumerator() => _poolElements.GetEnumerator();

        public Action<IPool> OnCapacityChange { get; set; }

        public T1 GetPoolObj<T1, T2>(T2 requestData)
            where T1 : class, IPoolElement
            where T2 : BasePoolRequestData
        {
            if (!CheckRequestDataType(requestData))
            {
#if LOGGER_ENABLED
                UnityLogger.LogWarningWithTag($"Given Request Data Type : {requestData.GetType()} does not matches with required Request Data Type : {GetRequestDataType()} on pool : {GetType()}! Using default request parameters...");
#else
                Debug.LogWarning($"Given Request Data Type : {requestData.GetType()} does not matches with required Request Data Type : {GetRequestDataType()} on pool : {GetType()}! Using default request parameters...");
#endif
                _requestdata = CollectionPoolRequestData.DefaultData;
            }
            else
                _requestdata = requestData as CollectionPoolRequestData;


            int requestedCapacity = _requestdata.Capacity;

            if (_poolElements.TryGetValue(requestedCapacity, out var poolElements))
            {
                for (int i = 0; i < poolElements.Count; i++)
                {
                    if (poolElements[i].GetPoolDataHandler().IsInPool())
                    {
                        poolElements[i].GetPoolDataHandler().GetFromPool();
                        return poolElements[i] as T1;
                    }
                }
#if LOGGER_ENABLED
                UnityLogger.LogWarningWithTag($"Collection Pool of : {GetType()} does not have any available collection with given capacity : {requestedCapacity}! Creating a new one in given capacity...");
#else
                Debug.LogWarning($"Collection Pool of : {GetType()} does not have any available collection with given capacity : {requestedCapacity}! Creating a new one in given capacity...");
#endif
                var poolElement = poolElements.AddWithReturn(new());
                poolElement.SetCapacity(requestedCapacity);
                poolElement.AddOnCapacityChanged(ChangePoolBucketOnCapacityChange);
                poolElement.GetPoolDataHandler().GetFromPool();

                try
                {
                    OnCapacityChange?.Invoke(this);
                }
                catch (System.Exception e)
                {
#if LOGGER_ENABLED
                    UnityLogger.LogErrorWithTag($"Error calling OnCapacityChange action on pool : {GetType()}! Error : {e}");
#else
                    Debug.LogError($"Error calling OnCapacityChange action on pool : {GetType()}! Error : {e}");
#endif
                }

                return poolElement as T1;
            }
            else
            {
                foreach (var poolElement in _poolElements)
                {
                    if (poolElement.Key >= requestedCapacity)
                    {
                        for (int i = 0; i < poolElement.Value.Count; i++)
                        {
                            if (poolElement.Value[i].GetPoolDataHandler().IsInPool())
                            {
                                poolElement.Value[i].GetPoolDataHandler().GetFromPool();
                                return poolElement.Value[i] as T1;
                            }
                        }
#if LOGGER_ENABLED
                        UnityLogger.LogWarningWithTag($"Collection Pool of : {GetType()} does not have any non used collection on capacity: {requestedCapacity}! Found a bigger pool with capacity : {poolElement.Key} but here is no unused pools in that capacity! Creating a new collection pool with given capacity...");
#else
                        Debug.LogWarning($"Collection Pool of : {GetType()} does not have any non used collection on capacity: {requestedCapacity}! Found a bigger pool with capacity : {poolElement.Key} but here is no unused pools in that capacity! Creating a new collection pool with given capacity...");
#endif
                        var foundElement = poolElement.Value.AddWithReturn(new());
                        foundElement.SetCapacity(poolElement.Key);
                        foundElement.AddOnCapacityChanged(ChangePoolBucketOnCapacityChange);
                        foundElement.GetPoolDataHandler().GetFromPool();

                        try
                        {
                            OnCapacityChange?.Invoke(this);
                        }
                        catch (System.Exception e)
                        {
#if LOGGER_ENABLED
                            UnityLogger.LogErrorWithTag($"Error calling OnCapacityChange action on pool : {GetType()}! Error : {e}");
#else
                            Debug.LogError($"Error calling OnCapacityChange action on pool : {GetType()}! Error : {e}");
#endif
                        }

                        return foundElement as T1;
                    }
                }

#if LOGGER_ENABLED
                UnityLogger.LogWarningWithTag($"Collection Pool of : {GetType()} does not have any available collection pool with given capacity : {requestedCapacity}! Creating a new collection pool with given capacity...");
#else
                Debug.LogWarning($"Collection Pool of : {GetType()} does not have any available collection pool with given capacity : {requestedCapacity}! Creating a new collection pool with given capacity...");
#endif
                var addedNewCollectionList = _poolElements.AddWithReturn(requestedCapacity, new List<T>()).value;

                for (int i = 0; i < CollectionPoolConfigData.s_NonAvailableRequestCapacityCreateCount; i++)
                {
                    var addedCollection = addedNewCollectionList.AddWithReturn(new());
                    addedCollection.SetCapacity(requestedCapacity);
                    addedCollection.AddOnCapacityChanged(ChangePoolBucketOnCapacityChange);
                    addedCollection.GetPoolDataHandler().ReturnToPool();
                }

                OnCapacityChange?.Invoke(this);

                var foundPoolElement = addedNewCollectionList[0];
                foundPoolElement.GetPoolDataHandler().GetFromPool();
                return foundPoolElement as T1;
            }
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

        void ChangePoolBucketOnCapacityChange(IPoolElementWithCapacity poolElement, int oldCapacity, int newCapacity)
        {
            if (_poolElements.ContainsKey(oldCapacity) && _poolElements[oldCapacity].Contains(poolElement as T))
                _poolElements[oldCapacity].Remove(poolElement as T);

            if (!_poolElements.ContainsKey(newCapacity))
                _poolElements.Add(newCapacity, new List<T>());

            _poolElements[newCapacity].Add(poolElement as T);

            try
            {
                OnCapacityChange?.Invoke(this);
            }
            catch (System.Exception e)
            {

#if LOGGER_ENABLED
                UnityLogger.LogErrorWithTag($"Error calling OnCapacityChange action on pool : {GetType()}! Error : {e}");
#else
                Debug.LogError($"Error calling OnCapacityChange action on pool : {GetType()}! Error : {e}");
#endif

            }
        }

        public Type GetConfigDataType() => typeof(CollectionPoolConfigData);
        public Type GetRequestDataType() => typeof(CollectionPoolRequestData);

        public bool Initialize()
        {
            _configData = CollectionPoolConfigData.s_DefaultData;
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
                _configData = CollectionPoolConfigData.s_DefaultData;
            }
            else
                _configData = configData as CollectionPoolConfigData;

            _poolElements.Clear();

            foreach (var collectionCapacityAndCount in _configData.CollectionCapacitiesAndCounts)
            {
                var collectionList = _poolElements.AddWithReturn(collectionCapacityAndCount.Capacity, new List<T>()).value;

                for (int i = 0; i < collectionCapacityAndCount.Count; i++)
                {
                    var addedCollection = collectionList.AddWithReturn(new());
                    addedCollection.SetCapacity(collectionCapacityAndCount.Capacity);
                    addedCollection.AddOnCapacityChanged(ChangePoolBucketOnCapacityChange);
                    addedCollection.GetPoolDataHandler().ReturnToPool();
                }
            }

            return true;
        }

        public BasePoolConfigData GetPoolAsConfigData()
        {
            CollectionPoolConfigData configData = new CollectionPoolConfigData();

            foreach (var poolElement in _poolElements)
            {
                CollectionPoolConfigData.CapacityAndCount capacityAndCount = new CollectionPoolConfigData.CapacityAndCount();

                capacityAndCount.Capacity = poolElement.Key;
                capacityAndCount.Count = poolElement.Value.Count;

                configData.CollectionCapacitiesAndCounts.Add(capacityAndCount);
            }

            configData.CollectionCapacitiesAndCounts.OrderBy(x => x.Capacity);

            return configData;
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
