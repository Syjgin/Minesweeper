using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class Field : MonoBehaviour, IPointerClickHandler
    {
        public Action<Vector2Int> OnCellClick;
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);
        [SerializeField] private Vector2 _startPosition = Vector2.zero;
        private int _currentIndex;
        private float _cellWidth;
        
        public int TotalCells => _gridSize.x * _gridSize.y;

        public void Clear()
        {
            _currentIndex = 0;
        }

        public void Init(int size, float cellWidth, Vector3 offset)
        {
            _gridSize = new Vector2Int(size, size);
            _cellWidth = cellWidth;
            transform.position = new Vector3(-offset.x, -offset.y, 0);
            transform.localScale = new Vector3(size, size, 1);
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
            var x = _startPosition.x + gridPosition.x * _cellWidth;
            var y = _startPosition.y + gridPosition.y * _cellWidth;
            cell.transform.SetParent(transform);
            cell.transform.localPosition = new Vector3(x, y, 0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}
