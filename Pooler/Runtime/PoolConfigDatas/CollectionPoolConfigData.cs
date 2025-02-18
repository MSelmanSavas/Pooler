using System.Collections.Generic;

namespace Pooler.Config.Data
{
    [System.Serializable]
    public class CollectionPoolConfigData : BasePoolConfigData
    {
        public List<CapacityAndCount> CollectionCapacitiesAndCounts = new();

        [System.Serializable]
        public class CapacityAndCount
        {
            public int Capacity;
            public int Count;
        }

        public static int s_NonAvailableRequestCapacityCreateCount = 5;
        public override BasePoolConfigData GetDefaultConfigData() => s_DefaultData;

        public static CollectionPoolConfigData s_DefaultData = new()
        {
            CollectionCapacitiesAndCounts = new()
            {
                new CapacityAndCount()
                {
                    Count = 20,
                    Capacity = 20,
                },
                // new CapacityAndCount()
                // {
                //     Count = 20,
                //     Capacity = 50,
                // },
                // new CapacityAndCount()
                // {
                //     Count = 20,
                //     Capacity = 100,
                // }
            }
        };
    }
}