namespace Pooler.Request.Data
{
    [System.Serializable]
    public class CollectionPoolRequestData : BasePoolRequestData
    {
        public int Capacity;

        public static readonly CollectionPoolRequestData DefaultData = new()
        {
            Capacity = 20
        };
    }
}