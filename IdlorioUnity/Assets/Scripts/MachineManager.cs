﻿using System.Collections;
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

    public static void DisconnectMachines(Conveyor c)
    {
        c._input._conveyor = null;
        c._output._conveyor = null;
        _instance._conveyors.Remove(c);
    }

    public static bool ConnectMachines(MachineConnector output, MachineConnector input)
    {
        if (output != null
            && input != null)
        {
            Conveyor c = new Conveyor(output, input);

            output._conveyor = c;
            input._conveyor = c;

            Map map = new Map();
            map._conveyors = _instance._conveyors;
            map.Machines = _instance.m_machines;
            map.BuildingInputs = _instance.m_machines.SelectMany(x => x._inputs).ToList();
            map.BuildingOutputs = _instance.m_machines.SelectMany(x => x._outputs).ToList();
            map.Machines = _instance.m_machines;
            map.Height = 20;
            map.Width = 40;

            _instance._conveyors.Add(c);

            if (Autorouter.Autoroute(map, c))
            {
                return true;
            }
            else
            {
                DisconnectMachines(c);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static void UpdateRecipes()
    {
        MachineComputations.UpdateRecipes(MachineManager.GetInstance().m_machines);
        MachineComputations.UpdateRates(MachineManager.GetInstance().m_machines);
    }
    
    public void Update()
    {
        SimulationRunner.GetInstance().Update(m_machines);

        foreach (Machine m in m_machines)
        {
            m.DrawDebug();
        }

        foreach(Conveyor c in _conveyors)
        {
            c.DrawDebug(1 / 60.0f);
        }

        // Highlight mouse cursor tile
        Point mouseTile = InputManager.GetPointerTile();
        GDK.DrawAABB(new Vector3(mouseTile.X + 0.5f, 0f, mouseTile.Y + 0.5f), new Vector3(0.45f, 0.1f, 0.45f), GDK.FadeColor(Color.yellow, 0.5f));
    }

    public static void Reset()
    {
        CreateInstance();
        _instance.m_machines.Clear();
    }

    public Machine GetMachineAtPoint(Point p)
    {
        foreach (Machine m in m_machines)
        {
            if (m.OccupiesPoint(p))
            {
                return m;
            }
        }

        return null;
    }
}
