using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMachineSelected : BaseInputState
{
    private Machine _selectedMachine = null;

    public InputStateMachineSelected(Machine machine)
    {
        _selectedMachine = machine;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _selectedMachine._isSelected = true;
    }

    public override void OnExit()
    {
        base.OnExit();

        _selectedMachine._isSelected = false;
    }
}
