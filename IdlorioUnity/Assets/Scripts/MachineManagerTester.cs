using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManagerTester : MonoBehaviour
{
    private Point _nextPosition = new Point(5, 5);

    Machine CreateMachine(int ins, int outs)
    {
        Machine m = new Machine();
        m._size.X = 1;
        m._size.Y = System.Math.Max(ins, outs);
        m._inputs = new MachineConnector[ins];

        for (int i = 0; i < m._inputs.Length; i++)
        {
            m._inputs[i] = new MachineConnector();
            m._inputs[i]._localDir = Direction.Left;
            m._inputs[i]._local.X = 0;
            m._inputs[i]._local.Y = i;
            m._inputs[i]._input = true;
            m._inputs[i]._thisMachine = m;
        }

        m._outputs = new MachineConnector[outs];
        for (int i = 0; i < m._outputs.Length; i++)
        {
            m._outputs[i] = new MachineConnector();
            m._outputs[i]._localDir = Direction.Right;
            m._outputs[i]._local.X = 0;
            m._outputs[i]._local.Y = i;
            m._outputs[i]._input = false;
            m._outputs[i]._thisMachine = m;
        }
        MachineManager.CreateInstance();
        MachineManager.RegisterMachine(m);
        return m;
    }

    private void OnEnable ()
    {
        Machine m0 = CreateMachine(0, 1);
        m0._maximumItemsPerSecond = System.Single.PositiveInfinity;
        m0._position = new Point(0, 4);
        m0._storageMode = Machine.StorageModes.Out;
        m0._storageCapacity = System.Single.PositiveInfinity;
        m0._itemsInStorage = System.Single.PositiveInfinity;
        m0._itemInStorage = new Item();
        m0._itemInStorage._baseItem = BaseItems.MixedOre;

        Machine m1 = CreateMachine(1, 2);
        m1._maximumItemsPerSecond = 10;
        m1._position = new Point(4, 4);
        m1._addedProperty = Properties.Centrifuged;

        Machine m2 = CreateMachine(1, 1);
        m2._maximumItemsPerSecond = 2;
        m2._position = new Point(12, 0);
        m2._addedProperty = Properties.Heated;

        Machine m3 = CreateMachine(1, 2);
        m3._maximumItemsPerSecond = 7;
        m3._position = new Point(12, 8);
        m3._addedProperty = Properties.Heated;
        m3._rotation = Rotation.CW180;

        Machine m4 = CreateMachine(2, 1);
        m4._maximumItemsPerSecond = 2;
        m4._position = new Point(20, 0);
        m4._addedProperty = Properties.Heated;

        Machine m5 = CreateMachine(1, 1);
        m5._maximumItemsPerSecond = 5;
        m5._position = new Point(20, 12);
        m5._addedProperty = Properties.Stretched;

        Machine m6 = CreateMachine(1, 1);
        m6._maximumItemsPerSecond = 6;
        m6._position = new Point(28, 12);
        m6._addedProperty = Properties.Cut;

        Machine m7 = CreateMachine(1, 0);
        m7._position = new Point(24, 0);
        m7._maximumItemsPerSecond = 1e6f;
        m7._storageCapacity = System.Single.PositiveInfinity;
        m7._storageMode = Machine.StorageModes.In;

        Machine m8 = CreateMachine(1, 0);
        m8._position = new Point(32, 12);
        m8._maximumItemsPerSecond = 1e6f;
        m8._storageCapacity = System.Single.PositiveInfinity;
        m8._storageMode = Machine.StorageModes.In;

        MachineManager.ConnectMachines(m0._outputs[0], m1._inputs[0]);
        MachineManager.ConnectMachines(m1._outputs[0], m2._inputs[0]);
        MachineManager.ConnectMachines(m1._outputs[1], m3._inputs[0]);
        MachineManager.ConnectMachines(m2._outputs[0], m4._inputs[0]);
        MachineManager.ConnectMachines(m3._outputs[0], m4._inputs[1]);
        MachineManager.ConnectMachines(m3._outputs[1], m5._inputs[0]);
        MachineManager.ConnectMachines(m5._outputs[0], m6._inputs[0]);
        MachineManager.ConnectMachines(m4._outputs[0], m7._inputs[0]);
        MachineManager.ConnectMachines(m6._outputs[0], m8._inputs[0]);

        MachineManager.UpdateRecipes();
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
