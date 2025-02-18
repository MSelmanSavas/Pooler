namespace Pooler.Config.Data
{
    [System.Serializable]
    public abstract class BasePoolConfigData
    {
        public const int CONFIG_CAPACITY_OFFSET = 5;
        public abstract BasePoolConfigData GetDefaultConfigData();
    }
}