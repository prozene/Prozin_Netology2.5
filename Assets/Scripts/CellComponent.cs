using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        public bool IsEmpty => Pair == null;
        public bool IsEmptyToMove => !_isSelectable && IsEmpty;
        
        private bool _isSelectable = true;

        protected override void Start()
        {
            _neighbors = new Dictionary<NeighborType, CellComponent>();
            base.Start();

            AddNeighbor(NeighborType.TopLeft, new Vector3(1,0,1));
            AddNeighbor(NeighborType.TopRight, new Vector3(1, 0, -1));
            AddNeighbor(NeighborType.BottomLeft, new Vector3(-1, 0, 1));
            AddNeighbor(NeighborType.BottomRight, new Vector3(-1, 0, -1));

            OnFocusEventHandler += ChangeMaterialOnHover;
        }

        private void AddNeighbor(NeighborType neighborType, Vector3 direction) // собираем список соседей для клеток
        {
            if (Physics.Raycast(transform.position, direction, out var hit, 5))
            {
                var cell = hit.collider.GetComponent<CellComponent>();
                if (cell != null)
                {
                    _neighbors.Add(neighborType, cell);
                }
            }
        }

        private Dictionary<NeighborType, CellComponent> _neighbors;

        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <param name="component">Сосед</param>
        /// <returns>Наличие соседа</returns>
        public bool TryGetNeighbor(NeighborType type, out CellComponent component) => _neighbors.TryGetValue(type, out component);

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }

        public void ChangeMaterialIfSelectable()
        {
            if (_isSelectable)
            {
                _isSelectable = false;
                AddAdditionalMaterial(selectMaterial, 2);
            }
            else
            {
                _isSelectable = true;
                RemoveAdditionalMaterial(2);
            }
        }
    }

    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight
    }
}