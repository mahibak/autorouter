using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineManager
{
    public List<Conveyor> _conveyors = new List<Conveyor>();

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

    float timeSeconds = 0;
    float recalculationNeededTime = 0;

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
            source._outputSlots[sourceSlot]._otherMachine = destination;
            source._outputSlots[sourceSlot]._otherConnector = destination._inputSlots[destSlot];

            destination._inputSlots[destSlot]._otherMachine = source;
            destination._inputSlots[destSlot]._otherConnector = source._outputSlots[sourceSlot];


            Map map = new Map();
            map._conveyors = _instance._conveyors;
            map.Machines = _instance.m_machines;
            map.BuildingInputs = _instance.m_machines.SelectMany(x => x._inputSlots).ToList();
            map.BuildingOutputs = _instance.m_machines.SelectMany(x => x._outputSlots).ToList();
            map.Machines = _instance.m_machines;
            map.Height = 20;
            map.Width = 40;

            Conveyor c = new Conveyor();
            c._start = source._outputSlots[sourceSlot]._local + source._position + source._outputSlots[sourceSlot]._localDir;
            c._end = destination._inputSlots[destSlot]._local + destination._position + destination._inputSlots[destSlot]._localDir;

            Autorouter.Autoroute(map, c, source._outputSlots[sourceSlot]._local + source._position, destination._inputSlots[destSlot]._local + destination._position);
            _instance._conveyors.Add(c);
            
            ProductionSpeedComputation.UpdateProductionSpeed(_instance.m_machines);
            _instance.recalculationNeededTime = _instance.timeSeconds + _instance.m_machines.Min(x => x.GetSecondsBeforeRecalculationNeeded());
        }
    }

    void UpdateMachines(float dt)
    {
        foreach (Machine m in m_machines)
        {
            m._itemsInStorage += m._itemsPerSecondToStorage * dt;
            if (m._itemsInStorage <= 0.001)
                m._itemsInStorage = 0;
        }
    }

    public void Update()
    {
        float desiredDt = 1 / 60.0f;

        while(desiredDt > 0)
        {
            if(timeSeconds + desiredDt > recalculationNeededTime)
            {
                float possibleDt = recalculationNeededTime - timeSeconds;
                UpdateMachines(possibleDt);
                desiredDt -= possibleDt;
                timeSeconds += possibleDt;
                ProductionSpeedComputation.UpdateProductionSpeed(_instance.m_machines);
                _instance.recalculationNeededTime = _instance.timeSeconds + _instance.m_machines.Min(x => x.GetSecondsBeforeRecalculationNeeded());
            }
            else
            {
                UpdateMachines(desiredDt);
                timeSeconds += desiredDt;
                break;
            }
        }

        foreach (Machine m in m_machines)
        {
            m.DrawDebug();
        }

        foreach(Conveyor c in _conveyors)
        {
            c.DrawDebug(1 / 60.0f);
        }
    }

    public static void Reset()
    {
        CreateInstance();
        _instance.m_machines.Clear();
    }
}
