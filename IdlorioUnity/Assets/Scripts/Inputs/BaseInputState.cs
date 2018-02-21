using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInputState
{
    private BaseInputState _substate = null;
    protected BaseInputState GetSubstate() { return _substate; }
    protected bool HasSubstate() { return _substate != null; }

    private bool _requestingTermination = false;
    protected void RequestTermination() { _requestingTermination = true; }

    public virtual void OnEnter() { }

    public virtual void OnExit()
    {
        if (_substate != null)
        {
            _substate.OnExit();
        }
    }

    public virtual void Update()
    {
        if (_substate != null)
        {
            if (_substate._requestingTermination)
            {
                TerminateSubstate();
            }
            else
            {
                _substate.Update();
            }
        }
    }

    public bool OnCursorClick(Point tile)
    {
        if (_substate == null || !_substate.OnCursorClick(tile))
        {
            return OnCursorClickInternal(tile);
        }

        return false;
    }

    public bool OnCursorHold(Point tile)
    {
        if (_substate == null || !_substate.OnCursorClick(tile))
        {
            return OnCursorHoldInternal(tile);
        }

        return false;
    }

    public bool OnDrag(Point tile, Vector3 dragAmountWorld)
    {
        if (_substate == null || !_substate.OnCursorClick(tile))
        {
            return OnDragInternal(tile, dragAmountWorld);
        }

        return false;
    }

    // Input virtuals return whether the input was handled by the child state. If it was, parent states don't handle it.
    protected virtual bool OnCursorClickInternal(Point tile) { return false; }
    protected virtual bool OnCursorHoldInternal(Point tile) { return false; }
    protected virtual bool OnDragInternal(Point tile, Vector3 dragAmountWorld) { return false; }

    public void SetSubstate(BaseInputState substate)
    {
        TerminateSubstate();
        _substate = substate;
        _substate.OnEnter();
    }

    public void TerminateSubstate()
    {
        if (_substate != null)
        {
            _substate.OnExit();
            _substate = null;
        }
    }
}
