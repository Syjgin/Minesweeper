using Bootstrap;
using Components;
using Events;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using UnityEngine;
using UnityEngine.InputSystem;
using View;

namespace Systems.Camera
{
    public class CameraDragSystem : IEcsInitSystem, IEcsRunSystem
    {
        
        private EcsFilter _cameraFilter;
        private EcsPool<CameraComponent> _cameraPool;
        private EcsPool<DirtyComponent> _dirtyPool;
        private EcsPool<FieldComponent>  _fieldCharacteristicsPool;
        private EcsFilter _fieldCharacteristicsFilter;
        private ReadOnlySettings _readOnlySettings;
        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();
            var world = systems.GetWorld();
            _cameraFilter = world.Filter<CameraComponent>().End();
            _cameraPool = world.GetPool<CameraComponent>();
            _dirtyPool = world.GetPool<DirtyComponent>();
            _fieldCharacteristicsPool = world.GetPool<FieldComponent>();
            _fieldCharacteristicsFilter = world.Filter<FieldComponent>().End();
            _eventsBus = sharedData.EventsBus;
            _readOnlySettings = sharedData.ReadOnlySettings;
        }

        public void Run(IEcsSystems systems)
        {
            if(!_eventsBus.HasEventSingleton<FieldDragEvent>(out var fieldDragEvent))
                return;
            var maxOffset = 0f;
            foreach (var entity in _fieldCharacteristicsFilter)
            {
                ref var characteristics = ref _fieldCharacteristicsPool.Get(entity);
                maxOffset = characteristics.MaxOffset;
                break;
            }
            var resultDelta = -1 * fieldDragEvent.Delta * _readOnlySettings.DragSensibility;
            foreach (var entity in _cameraFilter)
            {
                ref var cameraComponent = ref _cameraPool.Get(entity);
                if(Mathf.Abs(cameraComponent.Position.x + resultDelta.x) > maxOffset)
                    break;
                if(Mathf.Abs(cameraComponent.Position.y + resultDelta.y) > maxOffset)
                    break;
                cameraComponent.Drag(resultDelta);
                _dirtyPool.Add(entity);
                break;
            }
        }
    }
}