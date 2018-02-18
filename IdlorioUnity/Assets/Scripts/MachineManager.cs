using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManager
{
    private static MachineManager _instance;
    public static void CreateInstance()
    {
        if (_instance == null)
        {
            _instance = new MachineManager();
        }
    }
    public static MachineManager GetInstance()
    {
        return _instance;
    }

    private MachineManager() { }

    private List<Machine> m_machines = new List<Machine>();

    public static void RegisterMachine(Machine machine)
    {
        if (_instance.m_machines.Contains(machine))
        {
            Debug.Assert(false, "Registered the same machine twice.");
        }
        else
        {
            _instance.m_machines.Add(machine);
        }
    }

    public static void UnregisterMachine(Machine machine)
    {
        if (!_instance.m_machines.Remove(machine))
        {
            Debug.Assert(false, "Unregistered a machine that wasn't registered.");
        }
    }

    public static void ConnectMachines(Machine source, int sourceSlot, Machine destination, int destSlot)
    {
        if (source != null
            && destination != null
            && source._outputSlots.Length > sourceSlot
            && destination._inputSlots.Length > destSlot
            && source._outputSlots[sourceSlot]._otherMachine == null
            && destination._inputSlots[destSlot]._otherMachine == null)
        {
            // TODO : Probably sharing way too much info
            source._outputSlots[sourceSlot]._thisMachine = source;
            source._outputSlots[sourceSlot]._otherMachine = destination;
            source._outputSlots[sourceSlot]._otherConnector = destination._inputSlots[destSlot];

            destination._inputSlots[destSlot]._thisMachine = destination;
            destination._inputSlots[destSlot]._otherMachine = source;
            destination._inputSlots[destSlot]._otherConnector = source._outputSlots[sourceSlot];
        }
    }

    public void Update()
    {
        foreach (Machine m in m_machines)
        {
            m.DrawDebug();
        }
    }
}
