using UnityEngine;

namespace Components
{
    public struct CellCoordinateComponent
    {
        public Vector2Int Coordinates {get; private set;}

        public CellCoordinateComponent Init(Vector2Int coordinates)
        {
            Coordinates = coordinates;
            return this;
        }
    }
}