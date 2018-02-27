using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMove : BaseInputState
{
    private Machine _selectedMachine = null;
    private bool _isDragging = false;
    private Point _initialTile;
    private Point _deltaTile;

    public InputStateMove(Machine machine)
    {
        _selectedMachine = machine;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_selectedMachine != null)
        {
            UiManager.GetInstance().EnableMoveOptionsPanel();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        
        UiManager.GetInstance().DisableMoveOptionsPanel();
    }

    public override void Update()
    {
        base.Update();

        if (_isDragging && _selectedMachine != null)
        {
            Vector3 size = _selectedMachine._size.ToVector3();
            size[1] = 0f;
            GDK.DrawAABB((_selectedMachine._position + _selectedMachine.GetOppositeCornerFromPosition()).ToVector3() / 2.0f + new Vector3(0.5f, 0f, 0.5f) + _deltaTile.ToVector3(), new Vector3(size[0] * 0.5f, 1f, size[2] * 0.5f), Color.green);
        }
    }

    protected override bool OnCursorClickInternal(Point tile)
    {
        return true;
    }

    protected override bool OnCursorPressInternal(Point tile)
    {
        if (_selectedMachine != null &&_selectedMachine.OccupiesPoint(tile))
        {
            _isDragging = true;
            _initialTile = tile;
            _deltaTile = new Point(0,0);

            return true;
        }

        return false;
    }

    protected override bool OnCursorReleaseInternal(Point tile)
    {
        if (_isDragging)
        {
            _isDragging = false;
            return true;
        }

        return true;
    }

    protected override bool OnDragInternal(Point tile, Vector3 dragAmountWorld)
    {
        if (_isDragging && _selectedMachine != null)
        {
            _deltaTile = tile - _initialTile;
            return true;
        }

        return false;
    }

    protected override bool OnButtonClickedInternal(ButtonType button)
    {
        if (button == ButtonType.Cancel)
        {
            TerminateState();
            return true;
        }

        return false;
    }
}
