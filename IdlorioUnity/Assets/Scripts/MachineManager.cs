using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineManager
{
    public List<Conveyor> m_conveyors = new List<Conveyor>();

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
        _instance.m_conveyors.Remove(c);
    }

    public class PlacementCheckResult
    {
        public List<Machine> _intersectingMachines = new List<Machine>();
        public List<Conveyor> _intersectingConveyors = new List<Conveyor>();
        public List<Point> _intersectingPoints = new List<Point>();
    }

    public static PlacementCheckResult GetIntersections(GridTransform transform)
    {
        PlacementCheckResult result = new PlacementCheckResult();

        List<Point> occupiedPoints = transform.GetOccupiedTiles().ToList();

        foreach (Machine m1 in GetInstance().m_machines)
        {
            List<Point> intersections = m1.GetOccupiedTiles().Intersect(occupiedPoints).ToList();

            if (intersections.Count > 0)
            {
                result._intersectingMachines.Add(m1);
                result._intersectingPoints.AddRange(intersections);
            }
        }

        foreach (Conveyor c in GetInstance().m_conveyors)
        {
            List<Point> intersections = c.GetOccupiedTiles().Intersect(occupiedPoints).ToList();

            if (intersections.Count > 0)
            {
                result._intersectingConveyors.Add(c);
                result._intersectingPoints.AddRange(intersections);
            }
        }

        return result;
    }

    public static Conveyor ConnectMachines(MachineConnector output, MachineConnector input)
    {
        if (output != null
            && input != null)
        {
            Conveyor c = new Conveyor(output, input);

            output._conveyor = c;
            input._conveyor = c;

            Map map = new Map();
            map._conveyors = _instance.m_conveyors;
            map.Machines = _instance.m_machines;
            map.BuildingInputs = _instance.m_machines.SelectMany(x => x._inputs).ToList();
            map.BuildingOutputs = _instance.m_machines.SelectMany(x => x._outputs).ToList();
            map.Machines = _instance.m_machines;
            map.Height = 20;
            map.Width = 40;

            _instance.m_conveyors.Add(c);

            if (Autorouter.Autoroute(map, c))
            {
                return c;
            }
            else
            {
                DisconnectMachines(c);
                return null;
            }
        }
        else
        {
            return null;
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

        foreach(Conveyor c in m_conveyors)
        {
            c.DrawDebug(1 / 60.0f);
        }

        // Highlight mouse cursor tile
        Point mouseTile = InputManager.GetPointerTile();
        GDK.DrawAABB(new Vector3(mouseTile.X + 0.5f, 0f, mouseTile.Y + 0.5f), new Vector3(0.45f, 0.1f, 0.45f), GDK.FadeColor(Color.yellow, 0.35f));

        /*
        Machine machineBeingPlaced = new Machine();
        machineBeingPlaced._size = new Point(3, 2);
        machineBeingPlaced._position = mouseTile;

        machineBeingPlaced.DrawDebug();

        PlacementCheckResult result = GetIntersections(machineBeingPlaced);

        foreach(Point p in result._intersectingPoints)
        {
            GDK.DrawAABB(new Vector3(p.X + 0.5f, 2f, p.Y + 0.5f), new Vector3(0.45f, 0.1f, 0.45f), GDK.FadeColor(Color.red, 0.5f));
        }*/
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
            if (m.OccupiesTile(p))
            {
                return m;
            }
        }

        return null;
    }
}
