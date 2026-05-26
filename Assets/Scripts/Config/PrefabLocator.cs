using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using View;

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
        private Dictionary<Type, GameObject> _typeCache;

        private static readonly Dictionary<PrefabType, Type> PrefabTypeToComponentType = new()
        {
            { PrefabType.Cell, typeof(CellView) },
            { PrefabType.NewGameWindow, typeof(NewGameWindow) },
            { PrefabType.MainUi, typeof(MainUi) },
            { PrefabType.PauseWindow, typeof(PauseWindow) },
            { PrefabType.Field, typeof(FieldView) },
            { PrefabType.Camera, typeof(MainCamera) },
            { PrefabType.GameOverWindow, typeof(GameOverWindow) },
            { PrefabType.WinWindow, typeof(WinWindow) },
        };

        public void InitializeCache()
        {
            _prefabCache = new Dictionary<PrefabType, GameObject>();
            _typeCache = new Dictionary<Type, GameObject>();
            foreach (var prefabEntry in _entries)
            {
                _prefabCache.Add(prefabEntry.Type, prefabEntry.Prefab);
                if (PrefabTypeToComponentType.TryGetValue(prefabEntry.Type, out var componentType))
                {
                    _typeCache[componentType] = prefabEntry.Prefab;
                }
            }
        }

        public GameObject GetPrefabByType(Type componentType)
        {
            return _typeCache[componentType];
        }

        public GameObject GetPrefabByType<T>() where T : Behaviour
        {
            return GetPrefabByType(typeof(T));
        }
    }
}