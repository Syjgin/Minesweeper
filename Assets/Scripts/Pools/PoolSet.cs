using System.Collections.Generic;
using Config;
using UnityEngine;

namespace Pools
{
    public class PoolSet
    {
        private readonly Dictionary<PrefabType, IPool> _pools = new();

        public void RegisterPool(PrefabType prefabType, IPool pool)
        {
            _pools[prefabType] = pool;
        }
        
        public bool TryGetPool<T>(PrefabType prefabType, out ObjectPool<T> pool) where T : Behaviour
        {
            if (!_pools.TryGetValue(prefabType, out IPool poolInstance))
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