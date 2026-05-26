using Bootstrap;
using Components;
using Config;
using Leopotam.EcsLite;
using Pools;
using UnityEngine;
using View;

namespace Systems.Camera
{
    public class ApplyMoveCameraSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsPool<CameraComponent> _ecsCameraPool;
        private EcsPool<DirtyComponent> _dirtyPool;
        private EcsFilter _cameraFilter;
        private PoolSet _poolSet;

        public void Init(IEcsSystems systems)
        {
            _cameraFilter = systems.GetWorld().Filter<CameraComponent>().Inc<DirtyComponent>().End();
            _ecsCameraPool = systems.GetWorld().GetPool<CameraComponent>();
            _dirtyPool = systems.GetWorld().GetPool<DirtyComponent>();
            _poolSet = systems.GetShared<SharedData>().PoolSet;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _cameraFilter)
            {
                ref var cameraComponent = ref _ecsCameraPool.Get(entity);
                if (!_poolSet.TryGetPool<MainCamera>(out var cameraPool))
                    return;
                if(!cameraPool.TryGetObjectByEntity(entity, out var cameraObject))
                    return;
                cameraObject.transform.position = new Vector3(cameraComponent.Position.x, cameraComponent.Position.y, Constants.DefaultCameraZ);
                cameraObject.Camera.orthographicSize = cameraComponent.OrthoSize;
                _dirtyPool.Del(entity);
            }
        }
    }
}