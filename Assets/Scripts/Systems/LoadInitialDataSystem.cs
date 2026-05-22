using Bootstrap;
using Components;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using UnityEngine;
using View;

namespace Systems
{
    public class LoadInitialDataSystem : IEcsInitSystem
    {
        private EcsPool<CameraComponent> _ecsCameraPool;
        private PoolSet _poolSet;
        private EcsWorld _world;
        private SharedData _sharedData;
        
        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            var eventsBus = _sharedData.EventsBus;
            _world = systems.GetWorld();
            _ecsCameraPool = _world.GetPool<CameraComponent>();
            _poolSet = _sharedData.PoolSet;
            CreateCamera();
            eventsBus.NewEventSingleton<StartNewGameEvent>() = _sharedData.InitialData;
        }
        
        private void CreateCamera()
        {
            if(!_poolSet.TryGetPool<MainCamera>(PrefabType.Camera, out var cameraPool))
                return;
            var cameraEntity = _world.NewEntity();
            _ecsCameraPool.Add(cameraEntity);
            cameraPool.CreateObject(cameraEntity);
        }
    }
}