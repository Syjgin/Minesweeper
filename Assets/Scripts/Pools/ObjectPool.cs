using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
    public class ObjectPool<T> : IPool  where T : Behaviour
    {
        private readonly GameObject _prefab;
        private readonly int _initialSize;
        private readonly List<T> _pooledObjects;
        private readonly Dictionary<int, T> _activeObjects;
        private readonly Dictionary<int, int> _activeEntities;
        private readonly Transform _parent;

        public ObjectPool(GameObject prefab, int initialSize = 10, Transform parent = null)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            _parent = parent;
            _pooledObjects = new List<T>();
            _activeObjects = new Dictionary<int, T>();
            _activeEntities = new Dictionary<int, int>();
            InitializePool();
        }
        
        public T CreateObject(int entity = -1)
        {
            T obj;

            if (_pooledObjects.Count > 0)
            {
                int lastIndex = _pooledObjects.Count - 1;
                obj = _pooledObjects[lastIndex];
                _pooledObjects.RemoveAt(lastIndex);
            }
            else
            {
                CreatePooledObject();
                int lastIndex = _pooledObjects.Count - 1;
                obj = _pooledObjects[lastIndex];
                _pooledObjects.RemoveAt(lastIndex);
            }

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
                _pooledObjects.Add(obj);
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

        private void InitializePool()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                CreatePooledObject();
            }
        }

        private void CreatePooledObject()
        {
            GameObject instance = null;
            instance = _parent != null ? Object.Instantiate(_prefab, _parent) : Object.Instantiate(_prefab);
            instance.SetActive(false);
            T component = instance.GetComponent<T>();
            _pooledObjects.Add(component);
        }
    }
}
