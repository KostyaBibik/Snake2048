using UnityEngine.Pool;

namespace Infrastructure.Pools
{
    public interface IEntityPool<T> where T : class
    {
        int CountInactive { get; }

        T Get();

        PooledEntity<T> Get(out T v);

        void Release(T element);

        void Clear();
    }
}