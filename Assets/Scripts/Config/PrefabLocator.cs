using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "PrefabLocator", menuName = "Configs/Prefab Locator", order = 1)]
    public class PrefabLocator : ScriptableObject
    {
        [System.Serializable]
        private struct PrefabEntry
        {
            public PrefabType Type;
            public GameObject Prefab;
        }

        [Header("Prefabs")]
        [SerializeField] private List<PrefabEntry> _entries;

        private Dictionary<PrefabType, GameObject> _prefabCache;

        public void InitializeCache()
        {
            _prefabCache = new Dictionary<PrefabType, GameObject>();
            foreach (var prefabEntry in _entries)
            {
                _prefabCache.Add(prefabEntry.Type, prefabEntry.Prefab);
            }
        }

        public GameObject GetPrefabByType(PrefabType id)
        {
            return _prefabCache[id];
        }
    }
}