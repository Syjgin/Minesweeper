using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "InitialData", menuName = "Configs/Initial Data", order = 2)]
    public class InitialData : ScriptableObject
    {
        [field: SerializeField] public int GridSize { get; private set; } = 8;
        [field: SerializeField] public int MaxGridSize { get; private set; } = 50;
        [field: SerializeField] public int MinGridSize { get; private set; } = 5;
        [field: SerializeField] public int MinesCount { get; private set; } = 10;
        [field: SerializeField] public float ScrollWheelSensibility { get; private set; } = 0.1f;
        [field: SerializeField] public float DragSensibility { get; private set; } = 0.1f;
    }
}