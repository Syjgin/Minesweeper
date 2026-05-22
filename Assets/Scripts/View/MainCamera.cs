using UnityEngine;

namespace View
{
    public class MainCamera : MonoBehaviour
    {
        [field: SerializeField] public Camera Camera { get; private set; }
    }
}