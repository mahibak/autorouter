using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMain : BaseInputState
{
    public override void OnEnter()
    {
        base.OnEnter();

        SetSubstate(new InputStateReadyForSelection());
    }
}
