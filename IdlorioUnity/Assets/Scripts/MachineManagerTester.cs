using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManagerTester : MonoBehaviour
{
    private Vector3 _nextPosition = new Vector3(5, 0, 5);

    private void OnEnable ()
    {
        Machine m1 = new Machine();
        Machine m2 = new Machine();
        m2._sizeX = 2;
        m2._sizeY = 3;
        m2.InitializeTestSlots();
        Machine m3 = new Machine();
        AddMachine(m1);
        AddMachine(m2);
        AddMachine(m3);

        MachineManager.ConnectMachines(m1, 0, m2, 0);
        MachineManager.ConnectMachines(m2, 0, m3, 0);
    }

    private void AddMachine(Machine machine)
    {
        machine._position = _nextPosition;
        _nextPosition[0] += 8;
        MachineManager.RegisterMachine(machine);
    }

    private void BeginNextRow()
    {
        _nextPosition[0] = 5;
        _nextPosition[2] += 5;
    }
}
