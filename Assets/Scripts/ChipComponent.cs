using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] private float moveTime = 1;
        [SerializeField] private float moveSpeed = 1;
        [SerializeField] private float moveHeight = 1;

        public event Action OnMoveFinished;
        protected override void Start()
        {
            base.Start();
            if (Physics.Raycast ( transform.position, Vector3.down, out var hit, 5))
            {
                var cell = hit.collider.gameObject.GetComponent<CellComponent>(); // делаем пары для шашек и клеток
                if (cell != null)
                {
                    Pair = cell;
                    cell.Pair = this;
                }
            }

            OnFocusEventHandler += ChangeMaterialOnHover;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, false);
        }
        
        public void Select() // меняем цвет выделенной шашки
        {
            if (IsSelected)
            {
                IsSelected = false;
                RemoveAdditionalMaterial(2);
                ShowAvailableNeighboursToWalk();
            }
            else
            {
                IsSelected = true;
                AddAdditionalMaterial(selectMaterial, 2);
                ShowAvailableNeighboursToWalk();
            }
        }

        private void PairChipWithCell()
        {
            if(Pair != null)
            {
                Pair.Pair = null;
            }
            if (Physics.Raycast ( transform.position, Vector3.down, out var hit,5))
            {
                var cell = hit.collider.gameObject.GetComponent<CellComponent>(); // делаем пары для шашек и клеток
                if (cell != null)
                {
                    Pair = cell;
                    cell.Pair = this;
                }
            }
        }

        public void MoveOnNewCell(CellComponent cell)
        {
            Select();
            var newPosition = cell.transform.position;
            StartCoroutine(MoveChipOnNewCell(newPosition)); // корутина движения

        }

        private IEnumerator MoveChipOnNewCell (Vector3 newPosition)
        {
            var startPos = transform.position;
            var lerpTime = 0f;

            newPosition = newPosition + new Vector3(0, startPos.y, 0);
            while(lerpTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPos, newPosition, lerpTime / moveTime);
                lerpTime += moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = newPosition;
            PairChipWithCell();
            OnMoveFinished?.Invoke(); // проверяем чтоб евент не был null и вызываем его
        }

        public void ChangeMaterialOnEnable()
        {

        }
        private void ShowAvailableNeighboursToWalk()
        {
            if (Pair is CellComponent cell)
            {
                if (GetColor == ColorType.White)
                {
                    if (cell.TryGetNeighbor(NeighborType.TopLeft, out var leftCell) && leftCell.IsEmpty)
                    {
                        leftCell.ChangeMaterialIfSelectable();
                    }
                    if (cell.TryGetNeighbor(NeighborType.TopRight, out var rightCell) && rightCell.IsEmpty)
                    {
                        rightCell.ChangeMaterialIfSelectable();
                    }
                }
                else
                {
                    if (cell.TryGetNeighbor(NeighborType.BottomLeft, out var leftCell) && leftCell.IsEmpty)
                    {
                        leftCell.ChangeMaterialIfSelectable();
                    }
                    if (cell.TryGetNeighbor(NeighborType.BottomRight, out var rightCell) && rightCell.IsEmpty)
                    {
                        rightCell.ChangeMaterialIfSelectable();
                    }
                }
            }
        }
    }
}
