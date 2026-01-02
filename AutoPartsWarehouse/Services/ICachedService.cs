namespace AutoPartsWarehouse.Services
{
    public interface ICachedService<T> where T : class
    {
        void AddEntitiesToCache(string cacheKey, int rowsNumber = 20);
        IEnumerable<T> GetEntities(string cacheKey);
    }
}