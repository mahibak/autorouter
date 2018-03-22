using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMove : BaseInputState
{
    private Machine _selectedMachine = null;
    private Point _initialTile;
    private GridTransform _targetTransform = new GridTransform();
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
            _targetTransform = new GridTransform(_selectedMachine._gridTransform);
            _targetTransform.SetCenterTile(_initialTile);
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

        if (_targetTransform.GetTile() != _initialTile && _selectedMachine != null)
        {
            _targetTransform.DrawInWorld(_isPositionValid ? Color.green : Color.red);
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
        _targetTransform.SetCenterTile(tile);
        RefreshValidity();
    }

    private void Rotate(bool clockwise)
    {
        _targetTransform.SetRotation((Rotation)(((int)_targetTransform.GetRotation() + (clockwise ? 1 : -1)) % 4));
        RefreshValidity();
    }

    private void RefreshValidity()
    {
        MachineManager.PlacementCheckResult intersections = MachineManager.GetIntersections(_targetTransform);

        if (intersections._intersectingMachines.Count == 0
            || (intersections._intersectingMachines.Count == 1 && intersections._intersectingMachines[0] == _selectedMachine))
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
        switch (button)
        {
            case ButtonType.Cancel:
                TerminateState();
                return true;
            case ButtonType.Confirm:
                DoMove();
                TerminateState();
                return true;
            case ButtonType.RotateCW:
                Rotate(true);
                return true;
            case ButtonType.RotateCCW:
                Rotate(false);
                return true;
        }

        return false;
    }

    private void DoMove()
    {
        _selectedMachine._gridTransform.SetCenterTile(_targetTransform.GetTile());
        _selectedMachine._gridTransform.SetRotation(_targetTransform.GetRotation());
        _selectedMachine.TryRefreshConnectors();
    }
}
