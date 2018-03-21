using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMove : BaseInputState
{
    private Machine _selectedMachine = null;
    private Point _initialTile;
    private Point _currentTile;
    private bool _isPositionValid = true;

    public InputStateMove(Machine machine)
    {
        _selectedMachine = machine;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_selectedMachine != null)
        {
            UiManager.SetMoveOptionsPanelEnabled(true);
            UiManager.SetMoveConfirmButtonEnabled(true);
            _initialTile = _selectedMachine.GetCenterTile();
            _currentTile = _initialTile;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        
        UiManager.SetMoveOptionsPanelEnabled(false);
    }

    public override void Update()
    {
        base.Update();

        if (_currentTile != _initialTile && _selectedMachine != null)
        {
            Vector3 size = _selectedMachine._size.ToVector3();
            size[1] = 0f;
            Color color = _isPositionValid ? Color.green : Color.red;
            GDK.DrawAABB(_currentTile.ToVector3() + (_selectedMachine._position - _selectedMachine.GetOppositeCornerFromPosition()).ToVector3() / 2.0f + new Vector3(0.5f, 0f, 0.5f), new Vector3(size[0] * 0.5f, 1f, size[2] * 0.5f), color);
        }
    }

    protected override bool OnCursorClickInternal(Point tile)
    {
        return true;
    }

    protected override bool OnCursorPressInternal(Point tile)
    {
        SetPosition(tile);
        return true;
    }

    protected override bool OnDragInternal(Point tile, Vector3 dragAmountWorld)
    {
        SetPosition(tile);
        return true;
    }

    private void SetPosition(Point tile)
    {
        _currentTile = tile;

        Machine m = new Machine();
        m._position = _currentTile + _selectedMachine._position - _selectedMachine.GetOppositeCornerFromPosition();
        m._size = _selectedMachine._size;
        m._rotation = _selectedMachine._rotation;

        MachineManager.PlacementCheckResult intersections = MachineManager.GetIntersections(m);

        if (intersections._intersectingMachines.Count == 0
            || (intersections._intersectingMachines.Count == 1 && intersections._intersectingMachines[0] == _selectedMachine && _currentTile != _initialTile))
        {
            _isPositionValid = true;
            UiManager.SetMoveConfirmButtonEnabled(true);
        }
        else
        {
            _isPositionValid = false;
            UiManager.SetMoveConfirmButtonEnabled(false);
        }
    }

    protected override bool OnButtonClickedInternal(ButtonType button)
    {
        if (button == ButtonType.Cancel)
        {
            TerminateState();
            return true;
        }
        if (button == ButtonType.Confirm)
        {
            DoMove();
            TerminateState();
            return true;
        }

        return false;
    }

    private void DoMove()
    {
        _selectedMachine._position = _currentTile + _selectedMachine._position - _selectedMachine.GetOppositeCornerFromPosition();
    }
}
