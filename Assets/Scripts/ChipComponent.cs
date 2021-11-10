using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] private float moveTime = 2;
        [SerializeField] private float moveSpeed = 1;
        [SerializeField] private float moveHeight = 1;

        public event Action OnChipMove;

        private bool _isColorNotChangedToEatable = true;
        
        protected override void Start()
        {
            base.Start();
            PairChipWithCell();
            OnFocusEventHandler += ChangeMaterialOnHover;
        }

        private void PairChipWithCell()
        {
            if (Pair != null)
            {
                Pair.Pair = null;
            }
            if (Physics.Raycast ( transform.position, Vector3.down, out var hit, 5))
            {
                var cell = hit.collider.gameObject.GetComponent<CellComponent>(); // делаем пары для шашек и клеток
                if (cell != null)
                {
                    Pair = cell;
                    cell.Pair = this;
                }
            }
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

        public void MoveOnNewCell(CellComponent cell)
        {
            Select();
            var newPosition = cell.transform.position;
            StartCoroutine(MoveChipOnNewCell(newPosition));
        }

        private IEnumerator MoveChipOnNewCell(Vector3 newPosition)
        {
            var lerpTime = 0f;
            var startPos = transform.position;
            newPosition = new Vector3(newPosition.x, startPos.y + moveHeight, newPosition.z);
            while (lerpTime < moveTime / 2)
            {
                transform.position = Vector3.Lerp(startPos, newPosition, lerpTime / moveTime);
                lerpTime += moveSpeed * Time.deltaTime;
                yield return null;
            }

            TryEatEnemyChip();
            lerpTime = 0f;
            newPosition = new Vector3(newPosition.x, startPos.y, newPosition.z);
            startPos = transform.position;
            while (lerpTime < moveTime / 2)
            {
                transform.position = Vector3.Lerp(startPos, newPosition, lerpTime / moveTime);
                lerpTime += moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = newPosition;
            PairChipWithCell();
            OnChipMove?.Invoke();
        }

        private bool TryEatEnemyChip()
        {
            if(Physics.Raycast(transform.position, Vector3.down, out var hitChip, 5))
            {
                var chip = hitChip.collider.GetComponent<ChipComponent>();
                if (chip != null)
                {
                    chip.DestroyChip();
                    return true;
                }
            }

            return false;
        }

        public void DestroyChip()
        {
            Pair.Pair = null;
            Pair = null;
            gameObject.SetActive(false);
        }

        public void ChangeMaterialOnEatable()
        {
            if (_isColorNotChangedToEatable)
            {
                _isColorNotChangedToEatable = false;
                AddAdditionalMaterial(eatableMaterial, 3);
            }
            else
            {
                _isColorNotChangedToEatable = true;
                RemoveAdditionalMaterial(3);
            }
        }

        // shit, it smells
        private void ShowAvailableNeighboursToWalk()
        {
            if (Pair is CellComponent cell)
            {
                if (GetColor == ColorType.White)
                {
                    if (cell.TryGetNeighbor(NeighborType.TopLeft, out var leftCell))
                    {
                        if (leftCell.IsEmpty)
                        {
                            leftCell.ChangeMaterialIfSelectable();
                        }
                        else
                        {
                            if (leftCell.Pair.GetColor != GetColor && 
                                leftCell.TryGetNeighbor(NeighborType.TopLeft, out var leftOverEnemy) && 
                                leftOverEnemy.IsEmpty)
                            {
                                (leftCell.Pair as ChipComponent)?.ChangeMaterialOnEatable();
                                leftOverEnemy.ChangeMaterialIfSelectable();
                            }
                        }
                    }
                    if (cell.TryGetNeighbor(NeighborType.TopRight, out var rightCell))
                    {
                        if (rightCell.IsEmpty)
                        {
                            rightCell.ChangeMaterialIfSelectable();
                        }
                        else
                        {
                            if (rightCell.Pair.GetColor != GetColor && 
                                rightCell.TryGetNeighbor(NeighborType.TopRight, out var rightOverEnemy) && 
                                rightOverEnemy.IsEmpty)
                            {
                                (rightCell.Pair as ChipComponent)?.ChangeMaterialOnEatable();
                                rightOverEnemy.ChangeMaterialIfSelectable();
                            }
                        }
                    }
                }
                else
                {
                    if (cell.TryGetNeighbor(NeighborType.BottomLeft, out var leftCell))
                    {
                        if (leftCell.IsEmpty)
                        {
                            leftCell.ChangeMaterialIfSelectable();
                        }
                        else
                        {
                            if (leftCell.Pair.GetColor != GetColor && 
                                leftCell.TryGetNeighbor(NeighborType.BottomLeft, out var leftOverEnemy) && 
                                leftOverEnemy.IsEmpty)
                            {
                                (leftCell.Pair as ChipComponent)?.ChangeMaterialOnEatable();
                                leftOverEnemy.ChangeMaterialIfSelectable();
                            }
                        }
                    }
                    if (cell.TryGetNeighbor(NeighborType.BottomRight, out var rightCell))
                    {
                        if (rightCell.IsEmpty)
                        {
                            rightCell.ChangeMaterialIfSelectable();
                        }
                        else
                        {
                            if (rightCell.Pair.GetColor != GetColor && 
                                rightCell.TryGetNeighbor(NeighborType.BottomRight, out var rightOverEnemy) && 
                                rightOverEnemy.IsEmpty)
                            {
                                (rightCell.Pair as ChipComponent)?.ChangeMaterialOnEatable();
                                rightOverEnemy.ChangeMaterialIfSelectable();
                            }
                        }
                    }
                }
            }
        }
    }
}
