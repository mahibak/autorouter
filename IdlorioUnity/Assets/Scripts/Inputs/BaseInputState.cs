using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInputState
{
    private BaseInputState _substate = null;
    protected BaseInputState GetSubstate() { return _substate; }
    protected bool HasSubstate() { return _substate != null; }

    private bool _requestingTermination = false;
    protected void TerminateState() { _requestingTermination = true; }

    public virtual void OnEnter() { }
    
    public virtual void OnExit()
    {
        if (_substate != null)
        {
            _substate.OnExit();
        }
    }

    protected virtual void OnSubstatePreExit() { }
    protected virtual void OnSubstatePostExit() { }

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
        if (_substate != null && _substate.OnCursorClick(tile))
        {
            return true;
        }
        return OnCursorClickInternal(tile);
    }

    public bool OnCursorHold(Point tile)
    {
        if (_substate != null && _substate.OnCursorHold(tile))
        {
            return true;
        }
        return OnCursorHoldInternal(tile);
    }

    public bool OnCursorPress(Point tile)
    {
        if (_substate != null && _substate.OnCursorPress(tile))
        {
            return true;
        }
        return OnCursorPressInternal(tile);
    }

    public bool OnCursorRelease(Point tile)
    {
        if (_substate != null && _substate.OnCursorRelease(tile))
        {
            return true;
        }
        return OnCursorReleaseInternal(tile);
    }

    public bool OnDrag(Point tile, Vector3 dragAmountWorld)
    {
        if (_substate != null && _substate.OnDrag(tile, dragAmountWorld))
        {
            return true;
        }
        return OnDragInternal(tile, dragAmountWorld);
    }

    public bool OnButtonClick(ButtonType button)
    {
        if (_substate != null && _substate.OnButtonClick(button))
        {
            return true;
        }
        return OnButtonClickedInternal(button);
    }

    // Input virtuals return whether the input was handled by the child state. If it was, parent states don't handle it.
    protected virtual bool OnCursorClickInternal(Point tile) { return false; }
    protected virtual bool OnCursorHoldInternal(Point tile) { return false; }
    protected virtual bool OnCursorPressInternal(Point tile) { return false; }
    protected virtual bool OnCursorReleaseInternal(Point tile) { return false; }
    protected virtual bool OnDragInternal(Point tile, Vector3 dragAmountWorld) { return false; }
    protected virtual bool OnButtonClickedInternal(ButtonType button) { return false; }

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
            OnSubstatePreExit();
            _substate.OnExit();
            _substate = null;
            OnSubstatePostExit();
        }
    }
}
