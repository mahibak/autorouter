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

        if (_selectedMachine != null)
        {
            _selectedMachine._isSelected = true;
            UiManager.GetInstance().EnableMachineSelectedPanel(_selectedMachine.GetMachineInfoString());
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_selectedMachine != null)
        {
            _selectedMachine._isSelected = false;
            UiManager.GetInstance().DisableMachineSelectedPanel();
        }
    }

    public override void Update()
    {
        base.Update();

        if (_selectedMachine == null)
        {
            RequestTermination();
        }
    }

    protected override bool OnCursorClickInternal(Point tile)
    {
        Machine machine = MachineManager.GetInstance().GetMachineAtPoint(tile);

        if (machine == _selectedMachine)
        {
            RequestTermination();
            return true;
        }

        return false;
    }
}
