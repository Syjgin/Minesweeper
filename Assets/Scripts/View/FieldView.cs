using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class FieldView : MonoBehaviour, IPointerClickHandler
    {
        public Action<Vector2Int> OnCellClick;
        private Vector2Int _gridSize;
        private int _currentIndex;
        private float _cellWidth;
        private Camera _camera;
        private int _totalCells;

        public void Clear()
        {
            _currentIndex = 0;
        }

        public void Init(int size, float cellWidth, Vector3 offset, Camera camera)
        {
            _camera = camera;
            _gridSize = new Vector2Int(size, size);
            _cellWidth = cellWidth;
            transform.position = new Vector3(-offset.x, -offset.y, 0);
            transform.localScale = new Vector3(size, size, 1);
            _totalCells = _gridSize.x * _gridSize.y;
        }
        
        public void AddCell(CellView view)
        {
            SetCellPosition(view, GetNextPosition());
        }

        private Vector2Int GetNextPosition()
        {
            while (_currentIndex < _totalCells)
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
            var x = gridPosition.x * _cellWidth;
            var y = gridPosition.y * _cellWidth;
            cell.transform.SetParent(transform);
            cell.transform.localPosition = new Vector3(x, y, 0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var worldPoint = _camera.ScreenToWorldPoint(eventData.position);
            
            var localPoint = transform.InverseTransformPoint(worldPoint);
            
            var x = Mathf.FloorToInt(localPoint.x / _cellWidth);
            var y = Mathf.FloorToInt(localPoint.y / _cellWidth);
            
            if (x >= 0 && x < _gridSize.x && y >= 0 && y < _gridSize.y)
            {
                Debug.Log($"{x}, {y}");
                OnCellClick?.Invoke(new Vector2Int(x, y));  
            }
        }
    }
}
