using Checkers;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ColorType _currentTurn;
    [SerializeField] private EventSystem _eventSystem;

    private ChipComponent _selectedChip;
    private CellComponent[] _cells;
    private ChipComponent[] _chips;

    private void Awake()
    {
        _cells = FindObjectsOfType<CellComponent>();
        
        _chips = FindObjectsOfType<ChipComponent>();
        
        if (_chips?.Length > 0)
        {
            foreach (var chip in _chips)
            {
                chip.OnClickEventHandler += SelectComponent;
                chip.OnChipMove += SwitchEventSystemStatus;
            }
        }
        
        if (_cells?.Length > 0)
        {
            foreach (var cell in _cells)
            {
                cell.OnClickEventHandler += SelectComponent;
            }
        }
    }

    private void SelectComponent(BaseClickComponent component) // меняем выделенные фишки
    {
        if (component is ChipComponent chip && chip.GetColor == _currentTurn)
        {
            if (_selectedChip == null)
            {
                _selectedChip = chip;
            }
            
            if (_selectedChip == chip)
            {
                chip.Select();
                if (!chip.IsSelected)
                {
                    _selectedChip = null;
                }
            }
            else
            {
                _selectedChip.Select();
                _selectedChip = chip;
                chip.Select();
            }
        }
        else if(component is CellComponent cell && cell.IsEmptyToMove && _selectedChip != null)
        {
            SwitchEventSystemStatus();
            _selectedChip.MoveOnNewCell(cell);
            _selectedChip = null;
            NextTurn();
        }
    }

    private void NextTurn()
    {
        _currentTurn = _currentTurn == ColorType.Black ? ColorType.White : ColorType.Black;
    }

    private void SwitchEventSystemStatus()
    {
        _eventSystem.enabled = !_eventSystem.enabled;
    }
}