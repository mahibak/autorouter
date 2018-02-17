using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManagerTester : MonoBehaviour
{
    private Vector3 _nextPosition = new Vector3(5, 0, 5);

    Machine CreateMachine(int ins, int outs)
    {
        Machine m = new Machine();
        m._sizeX = 1;
        m._sizeY = System.Math.Max(ins, outs);
        m._inputSlots = new MachineConnector[ins];

        for (int i = 0; i < m._inputSlots.Length; i++)
        {
            m._inputSlots[i] = new MachineConnector();
            m._inputSlots[i]._localDir = Direction.Left;
            m._inputSlots[i]._localX = 0;
            m._inputSlots[i]._localY = i;
        }

        m._outputSlots = new MachineConnector[outs];
        for (int i = 0; i < m._outputSlots.Length; i++)
        {
            m._outputSlots[i] = new MachineConnector();
            m._outputSlots[i]._localDir = Direction.Right;
            m._outputSlots[i]._localX = 0;
            m._outputSlots[i]._localY = i;
        }
        MachineManager.CreateInstance();
        MachineManager.RegisterMachine(m);
        return m;
    }

    private void OnEnable ()
    {
        Machine m1 = CreateMachine(0, 2);
        m1._maximumItemsPerSecondProduction = 10;
        m1._position = new Vector3(0, 0, 1);

        Machine m2 = CreateMachine(1, 1);
        m2._maximumItemsPerSecondProduction = 2;
        m2._position = new Vector3(2, 0, 0);

        Machine m3 = CreateMachine(1, 2);
        m3._maximumItemsPerSecondProduction = 7;
        m3._position = new Vector3(2, 0, 2);

        Machine m4 = CreateMachine(2, 0);
        m4._maximumItemsPerSecondProduction = 2;
        m4._position = new Vector3(4, 0, 0);

        Machine m5 = CreateMachine(1, 1);
        m5._maximumItemsPerSecondProduction = 5;
        m5._maximumItemsPerSecondFromStorage = System.Single.PositiveInfinity;
        m5._storageCapacity = System.Single.PositiveInfinity;
        m5._itemsInStorage = 1000;
        m5._position = new Vector3(4, 0, 3);

        Machine m6 = CreateMachine(1, 0);
        m6._maximumItemsPerSecondProduction = 6;
        m6._position = new Vector3(6, 0, 3);
        
        MachineManager.ConnectMachines(m1, 0, m2, 0);
        MachineManager.ConnectMachines(m1, 1, m3, 0);
        MachineManager.ConnectMachines(m2, 0, m4, 0);
        MachineManager.ConnectMachines(m3, 0, m4, 1);
        MachineManager.ConnectMachines(m3, 1, m5, 0);
        MachineManager.ConnectMachines(m5, 0, m6, 0);
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
