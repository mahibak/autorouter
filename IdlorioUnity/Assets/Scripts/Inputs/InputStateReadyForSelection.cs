using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateReadyForSelection : BaseInputState
{
    public override void OnEnter()
    {
        base.OnEnter();
    }
    
    protected override bool OnCursorClickInternal(Point tile)
    {
        Machine machine = MachineManager.GetInstance().GetMachineAtPoint(tile);
            
        if (machine != null)
        {
            SetSubstate(new InputStateMachineSelected(machine));
            return true;
        }

        return false;
    }
}
