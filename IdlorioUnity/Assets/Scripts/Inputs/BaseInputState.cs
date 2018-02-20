using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInputState
{
    private BaseInputState _substate = null;
    protected BaseInputState GetSubstate() { return _substate; }
    protected bool HasSubstate() { return _substate != null; }

    protected bool _requestingTermination = false;

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

    public virtual void OnCursorClick(Point tile)
    {
        if (_substate != null)
        {
            _substate.OnCursorClick(tile);
        }
    }

    public virtual void OnCursorHold(Point tile)
    {
        if (_substate != null)
        {
            _substate.OnCursorHold(tile);
        }
    }

    public virtual void OnDrag(Point tile, Vector3 dragAmountWorld)
    {
        if (_substate != null)
        {
            _substate.OnDrag(tile, dragAmountWorld);
        }
    }

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
