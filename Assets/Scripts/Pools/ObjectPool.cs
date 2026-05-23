using System.Collections.Generic;
using Config;
using UnityEngine;

namespace Pools
{
    public class ObjectPool<T> : IPool  where T : Behaviour 
    {
        public PrefabType PrefabType { get; }
        private readonly GameObject _prefab;
        private readonly Dictionary<int, T> _activeObjects;
        private readonly Dictionary<int, int> _activeEntities;
        private readonly UnityEngine.Pool.ObjectPool<T> _pool;
        private readonly Transform _parent;

        public ObjectPool(PrefabLocator locator, PrefabType type, int initialSize = 10, Transform parent = null)
        {
            PrefabType = type;
            _prefab = locator.GetPrefabByType(PrefabType);
            _parent = parent;
            _pool = new UnityEngine.Pool.ObjectPool<T>(CreateObject, OnGetObject, OnReleaseObject, OnDestroyObject, true, initialSize);
            _activeObjects = new Dictionary<int, T>();
            _activeEntities = new Dictionary<int, int>();
        }
        
        public T CreateObject(int entity)
        {
            T obj = _pool.Get();
            obj.gameObject.SetActive(true);
            _activeObjects.Add(entity, obj);
            _activeEntities.Add(obj.GetInstanceID(), entity);
            return obj;
        }

        public void ReturnObject(int entity)
        {
            if (_activeObjects.TryGetValue(entity, out var obj))
            {
                obj.gameObject.SetActive(false);
                _activeObjects.Remove(entity);
                _activeEntities.Remove(obj.GetInstanceID());
                _pool.Release(obj);
            }
        }

        public bool TryGetObjectByEntity(int entity, out T result)
        {
            return _activeObjects.TryGetValue(entity, out result);
        }

        public bool TryGetEntityByInstanceID(int instanceID, out int result)
        {
            return _activeEntities.TryGetValue(instanceID, out result);
        }
        
        private void OnDestroyObject(T obj)
        {
            Object.Destroy(obj);   
        }

        private void OnReleaseObject(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void OnGetObject(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        private T CreateObject()
        {
            return  Object.Instantiate(_prefab, _parent).GetComponent<T>();
        }
    }
}
