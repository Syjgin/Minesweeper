using System.Collections.Generic;
using UnityEngine;

namespace View
{
    public class Field : MonoBehaviour
    {
        [SerializeField] private SingleMineView _cellPrefab;
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);
        [SerializeField] private float _cellSpacing = 1f;
        [SerializeField] private Vector2 _startPosition = Vector2.zero;
        
        private readonly HashSet<Vector2Int> _occupiedPositions = new();
        private int _currentIndex;
        
        public int TotalCells => _gridSize.x * _gridSize.y;

        public void Clear()
        {
            _occupiedPositions.Clear();
            _currentIndex = 0;
        }
        
        public void AddCell(SingleMineView view)
        {
            if (_occupiedPositions.Count >= TotalCells)
            {
                return;
            }
            SetCellPosition(view, GetNextPosition());
        }

        private Vector2Int GetNextPosition()
        {
            while (_currentIndex < TotalCells)
            {
                int x = _currentIndex % _gridSize.x;
                int y = _currentIndex / _gridSize.x;
                _currentIndex++;
                
                Vector2Int pos = new Vector2Int(x, y);
                if (!_occupiedPositions.Contains(pos))
                {
                    return pos;
                }
            }
            return Vector2Int.zero;
        }

        private void SetCellPosition(SingleMineView cell, Vector2Int gridPosition)
        {
            var width = GetCellWidth(cell);
            var x = _startPosition.x + gridPosition.x * (_cellSpacing + width);
            var y = _startPosition.y + gridPosition.y * (_cellSpacing + width);
            cell.transform.localPosition = new Vector3(x, y, 0);
        }

        private float GetCellWidth(SingleMineView cell)
        {
            var image = cell.BackgroundImage;
            if (image != null && image.sprite != null)
            {
                return image.sprite.bounds.size.x;
            }
            return 1f;
        }
    }
}
