using System.Collections.Generic;
using UnityEngine;

namespace Systems.Cells
{
    public static class CoordinateUtils
    {
        public static void FillNeighbourCoordinates(Vector2Int currentCoordinates,
            HashSet<Vector2Int> neighbourCoordinates, int gridSize)
        {
            neighbourCoordinates.Clear();
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    
                    var newX = currentCoordinates.x + i;
                    var newY = currentCoordinates.y + j;
                    
                    if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
                    {
                        neighbourCoordinates.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }
    }
}