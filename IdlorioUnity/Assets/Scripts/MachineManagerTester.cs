using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManagerTester : MonoBehaviour
{
    private Point _nextPosition = new Point(5, 5);

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
            m._inputSlots[i]._local.X = 0;
            m._inputSlots[i]._local.Y = i;
        }

        m._outputSlots = new MachineConnector[outs];
        for (int i = 0; i < m._outputSlots.Length; i++)
        {
            m._outputSlots[i] = new MachineConnector();
            m._outputSlots[i]._localDir = Direction.Right;
            m._outputSlots[i]._local.X = 0;
            m._outputSlots[i]._local.Y = i;
        }
        MachineManager.CreateInstance();
        MachineManager.RegisterMachine(m);
        return m;
    }

    private void OnEnable ()
    {
        Machine m0 = CreateMachine(0, 1);
        m0._maximumItemsPerSecondFromStorage = System.Single.PositiveInfinity;
        m0._position = new Point(0, 4);
        m0._storageMode = Machine.StorageModes.Out;
        m0._storageCapacity = System.Single.PositiveInfinity;
        m0._itemsInStorage = System.Single.PositiveInfinity;
        m0._itemInStorage = new Item();
        m0._itemInStorage._baseItem = BaseItems.MixedOre;

        Machine m1 = CreateMachine(1, 2);
        m1._maximumItemsPerSecondProduction = 10;
        m1._position = new Point(4, 4);
        m1._addedProperty = Properties.Centrifuged;

        Machine m2 = CreateMachine(1, 1);
        m2._maximumItemsPerSecondProduction = 2;
        m2._position = new Point(12, 0);
        m2._addedProperty = Properties.Heated;

        Machine m3 = CreateMachine(1, 2);
        m3._maximumItemsPerSecondProduction = 7;
        m3._position = new Point(12, 8);
        m3._addedProperty = Properties.Heated;

        Machine m4 = CreateMachine(2, 0);
        m4._maximumItemsPerSecondProduction = 2;
        m4._position = new Point(20, 0);
        m4._storageCapacity = System.Single.PositiveInfinity;
        m4._storageMode = Machine.StorageModes.In;
        m4._addedProperty = Properties.Heated;

        Machine m5 = CreateMachine(1, 1);
        m5._maximumItemsPerSecondProduction = 5;
        m5._maximumItemsPerSecondFromStorage = System.Single.PositiveInfinity;
        m5._storageCapacity = System.Single.PositiveInfinity;
        m5._itemsInStorage = 5;
        m5._storageMode = Machine.StorageModes.Out;
        m5._position = new Point(20, 12);
        m5._addedProperty = Properties.Stretched;

        Machine m6 = CreateMachine(1, 0);
        m6._maximumItemsPerSecondProduction = 6;
        m6._position = new Point(28, 12);
        m6._storageCapacity = System.Single.PositiveInfinity;
        m6._storageMode = Machine.StorageModes.In;
        m6._addedProperty = Properties.Cut;

        MachineManager.ConnectMachines(m0, 0, m1, 0);
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
        _nextPosition.X += 8;
        MachineManager.RegisterMachine(machine);
    }

    private void BeginNextRow()
    {
        _nextPosition.X = 5;
        _nextPosition.Y += 5;
    }
}
