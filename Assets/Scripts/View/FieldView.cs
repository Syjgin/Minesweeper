using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class FieldView : MonoBehaviour, IPointerUpHandler, IDragHandler, IPointerDownHandler
    {
        public Action<MouseClickData> OnCellClickAction;
        public Action<Vector2> OnDragAction;
        [SerializeField] private BoxCollider2D _collider;
        private Vector2Int _gridSize;
        private int _currentIndex;
        private float _cellWidth;
        private Camera _camera;
        private int _totalCells;
        private float _multiplier;
        private bool _wasDragged;

        public void Init(int size, float cellWidth, float multiplier, Camera camera)
        {
            _currentIndex = 0;
            _multiplier = multiplier;
            _camera = camera;
            _gridSize = new Vector2Int(size, size);
            _cellWidth = cellWidth;
            transform.position = new Vector3(-multiplier, -multiplier, 0);
            transform.localScale = Vector3.one;

            _totalCells = _gridSize.x * _gridSize.y;

            _collider.size = new Vector2(size * _cellWidth, size * _cellWidth);
            _collider.offset = new Vector2(_cellWidth * _multiplier, _cellWidth * _multiplier);
        }

        public void AddCell(CellView view)
        {
            SetCellPosition(view, GetNextPosition());
        }

        private Vector2Int GetNextPosition()
        {
            while (_currentIndex < _totalCells)
            {
                var x = _currentIndex / _gridSize.x;
                var y = _currentIndex % _gridSize.x;
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
            cell.transform.localScale = Vector3.one;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_wasDragged)
            {
                _wasDragged = false;
                return;
            }
            
            var worldPoint = _camera.ScreenToWorldPoint(eventData.position);

            var localPoint = transform.InverseTransformPoint(worldPoint);

            var x = Mathf.RoundToInt(localPoint.x / _cellWidth);
            var y = Mathf.RoundToInt(localPoint.y / _cellWidth);

            if (x >= 0 && x < _gridSize.x && y >= 0 && y < _gridSize.y)
            {
                bool isLeftButton = eventData.button == PointerEventData.InputButton.Left;
                OnCellClickAction?.Invoke(new MouseClickData(new Vector2Int(x, y), isLeftButton));
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _wasDragged = true;
            OnDragAction?.Invoke(eventData.delta);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}