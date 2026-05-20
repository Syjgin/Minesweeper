using System;
using Config;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bootstrap
{
    public sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] private PrefabLocator _prefabLocator;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private GameObject _uiRoot;
        
        private EcsWorld _world;
        private IEcsSystems _updateRunSystems;

        private void Start()
        {
            _prefabLocator.InitializeCache();
            _world = new EcsWorld();
            
            var eventsBus = new EventsBus();
            
            var poolSet = CreatePrefabPoolSet();
            var sharedData = new SharedData(eventsBus, poolSet, _playerInput);
            _updateRunSystems = new EcsSystems(_world, sharedData);
            
        }
        
        private PoolSet CreatePrefabPoolSet()
        {
            var poolSet = new PoolSet();
            var cellPool = new ObjectPool<SingleMineUi>(_prefabLocator.GetPrefabByType(PrefabType.Cell), 512);
            var createWindowPool = new ObjectPool<CreateLevelWindow>(_prefabLocator.GetPrefabByType(PrefabType.CreateWindow), 1);
            var pauseWindowPool = new ObjectPool<PauseWindow>(_prefabLocator.GetPrefabByType(PrefabType.PauseWindow), 1);
            var uiPool = new ObjectPool<MainUi>(_prefabLocator.GetPrefabByType(PrefabType.MainUi), 1, _uiRoot.transform);
            
            poolSet.RegisterPool(PrefabType.Cell, cellPool);
            poolSet.RegisterPool(PrefabType.CreateWindow, createWindowPool);
            poolSet.RegisterPool(PrefabType.PauseWindow, pauseWindowPool);
            poolSet.RegisterPool(PrefabType.MainUi, uiPool);
            return poolSet;
        }
    }
}