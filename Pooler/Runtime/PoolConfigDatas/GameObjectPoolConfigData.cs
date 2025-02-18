using UnityEngine;

namespace Pooler.Config.Data
{
    [System.Serializable]
    public class GameObjectPoolConfigData : BasePoolConfigData
    {
        public GameObject Prefab;
        public int Capacity;

        public override BasePoolConfigData GetDefaultConfigData() => s_DefaultData;

        public static GameObjectPoolConfigData s_DefaultData = new()
        {
            Prefab = null,
            Capacity = 50,
        };
    }
}