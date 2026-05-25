using UnityEngine;

namespace View
{
    public struct MouseClickData
    {
        public readonly Vector2Int Position;
        public readonly bool IsLeftButton;

        public MouseClickData(Vector2Int position, bool isLeftButton)
        {
            Position = position;
            IsLeftButton = isLeftButton;
        }
    }
}