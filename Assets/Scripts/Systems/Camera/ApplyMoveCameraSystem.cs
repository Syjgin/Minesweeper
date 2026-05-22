using Bootstrap;
using Components;
using Config;
using Leopotam.EcsLite;
using Pools;
using View;

namespace Systems.Camera
{
    public class ApplyMoveCameraSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsPool<CameraComponent> _ecsCameraPool;
        private EcsPool<Dirty> _dirtyPool;
        private EcsFilter _cameraFilter;
        private PoolSet _poolSet;

        public void Init(IEcsSystems systems)
        {
            _cameraFilter = systems.GetWorld().Filter<CameraComponent>().Inc<Dirty>().End();
            _ecsCameraPool = systems.GetWorld().GetPool<CameraComponent>();
            _dirtyPool = systems.GetWorld().GetPool<Dirty>();
            _poolSet = systems.GetShared<SharedData>().PoolSet;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _cameraFilter)
            {
                ref var cameraComponent = ref _ecsCameraPool.Get(entity);
                if (!_poolSet.TryGetPool<MainCamera>(PrefabType.Camera, out var cameraPool))
                    return;
                if(!cameraPool.TryGetObjectByEntity(entity, out var cameraObject))
                    return;
                cameraObject.transform.position = cameraComponent.Position;
                _dirtyPool.Del(entity);
            }
        }
    }
}