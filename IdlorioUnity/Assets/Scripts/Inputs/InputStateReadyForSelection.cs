using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateReadyForSelection : BaseInputState
{
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnCursorClick(Point tile)
    {
        Machine machine = MachineManager.GetInstance().GetMachineAtPoint(tile);
            
        if (machine != null)
        {
            SetSubstate(new InputStateMachineSelected(machine));
        }
        
    }
}
