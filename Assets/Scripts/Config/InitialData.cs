using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "InitialData", menuName = "Configs/Initial Data", order = 2)]
    public class InitialData : ScriptableObject
    {
        [field: SerializeField] public int GridSize { get; private set; } = 8;
        [field: SerializeField] public int MinesCount { get; private set; } = 10;
        [field: SerializeField] public float InitialCameraOrthoSize { get; private set; } = -10;
        [field: SerializeField] public float CellSize { get; private set; } = 1f;
        [field: SerializeField] public float MinCameraOrthoSize { get; private set; }
        [field: SerializeField] public float MaxCameraOrthoSize { get; private set; }
    }
}