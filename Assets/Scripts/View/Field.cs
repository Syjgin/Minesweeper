using System.Collections.Generic;
using UnityEngine;

namespace View
{
    public class Field : MonoBehaviour
    {
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);
        [SerializeField] private float _cellSpacing = 1f;
        [SerializeField] private Vector2 _startPosition = Vector2.zero;
        
        private int _currentIndex;
        
        public int TotalCells => _gridSize.x * _gridSize.y;

        public void Clear()
        {
            _currentIndex = 0;
        }

        public void Init(int size)
        {
            _gridSize = new Vector2Int(size, size);
        }
        
        public void AddCell(CellView view)
        {
            SetCellPosition(view, GetNextPosition());
        }

        private Vector2Int GetNextPosition()
        {
            while (_currentIndex < TotalCells)
            {
                var x = _currentIndex % _gridSize.x;
                var y = _currentIndex / _gridSize.x;
                _currentIndex++;
                
                return new Vector2Int(x, y);
            }
            return Vector2Int.zero;
        }

        private void SetCellPosition(CellView cell, Vector2Int gridPosition)
        {
            var width = GetCellWidth(cell);
            var x = _startPosition.x + gridPosition.x * (_cellSpacing + width);
            var y = _startPosition.y + gridPosition.y * (_cellSpacing + width);
            cell.transform.SetParent(transform);
            cell.transform.localPosition = new Vector3(x, y, 0);
        }

        private float GetCellWidth(CellView cell)
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
