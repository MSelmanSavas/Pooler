using System;
using System.Collections;
using System.Collections.Generic;
using Pooler.Config.Data;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pooler
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewScriptablePoolConfigContainer", menuName = "Pooler/Config/Scriptable Pool Config Container", order = 0)]
    public class ScriptablePoolConfigContainer : ScriptableObject
    {
        public Dictionary<System.Type, BasePoolConfigData> TypeToConfigDatas = new();

        public bool AddNewConfigForType(System.Type type, BasePoolConfigData data)
        {
            if (TypeToConfigDatas.ContainsKey(type))
                return false;

            TypeToConfigDatas.Add(type, data);
            return true;
        }

#if UNITY_EDITOR

        private void Awake()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnValidate()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        public bool AddNewConfigForType(IPool pool, BasePoolConfigData data)
        {
            if (TypeToConfigDatas.ContainsKey(pool.GetType()))
                return false;

            TypeToConfigDatas.Add(pool.GetType(), data);

            return true;
        }

        public bool AddOrUpdateConfigForType(System.Type type, BasePoolConfigData data)
        {
            if (!TypeToConfigDatas.ContainsKey(type))
                return AddNewConfigForType(type, data);

            TypeToConfigDatas[type] = data;

            return true;
        }

        public bool AddOrUpdateConfigForType(IPool pool, BasePoolConfigData data)
        {
            if (!TypeToConfigDatas.ContainsKey(pool.GetType()))
                return AddNewConfigForType(pool.GetType(), data);

            TypeToConfigDatas[pool.GetType()] = data;

            return true;
        }

        void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (playModeState != PlayModeStateChange.ExitingPlayMode)
                return;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}

