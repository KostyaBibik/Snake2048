using System;

namespace UnityEngine.Pool
{
    public struct PooledEntity<T> : IDisposable where T : class
    {
        private readonly T m_ToReturn;
        private readonly IObjectPool<T> m_Pool;

        internal PooledEntity(T value, IObjectPool<T> pool)
        {
            this.m_ToReturn = value;
            this.m_Pool = pool;
        }

        void IDisposable.Dispose() => this.m_Pool.Release(this.m_ToReturn);
    }
}