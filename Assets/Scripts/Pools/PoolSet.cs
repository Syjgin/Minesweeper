using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
    public class PoolSet
    {
        private readonly Dictionary<Type, IPool> _pools = new();

        public void RegisterPool(IPool pool)
        {
            _pools[pool.ComponentType] = pool;
        }
        
        public bool TryGetPool<T>(out ObjectPool<T> pool) where T : Behaviour
        {
            if (!_pools.TryGetValue(typeof(T), out IPool poolInstance))
            {
                pool = null;
                return false;
            }

            if (poolInstance is ObjectPool<T> objectPool)
            {
                pool = objectPool;
                return true;
            }
            pool = null;
            return false;
        }
    }
}