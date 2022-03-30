namespace Cache.Common
{
    public interface ICache<P> where P : class
    {
        bool Access(P page);
    }
}
