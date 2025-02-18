namespace Pooler.Config.Data
{
    [System.Serializable]
    public class ClassPoolConfigData : BasePoolConfigData
    {
        public int Capacity;

        public static ClassPoolConfigData s_DefaultData = new()
        {
            Capacity = 5,
        };

        public override BasePoolConfigData GetDefaultConfigData() => s_DefaultData;
    }
}