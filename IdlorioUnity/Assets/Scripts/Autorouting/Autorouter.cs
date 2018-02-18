using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Map
{
    public int Height;
    public int Width;
    public List<Conveyor> _conveyors = new List<Conveyor>();
    public List<Machine> Machines = new List<Machine>();
    public List<MachineConnector> BuildingInputs = new List<MachineConnector>();
    public List<MachineConnector> BuildingOutputs = new List<MachineConnector>();
}

class Autorouter
{
   /* public static bool Reroute(List<Machine> machines, List<Conveyor> conveyors, Conveyor conveyorToRoute)
    {
        int originalLength = conveyorToRoute.Tiles.Count;
            
        AutoroutingMap map = new AutoroutingMap(originalMap);
        AutoroutingNet netToReroute = map.tiles[conveyorToRoute.Start.X, conveyorToRoute.Start.Y].Net;
        map.RipupNet(netToReroute);
            
        List<Point> newPath = Autoroute(map, netToReroute);

        if (newPath.Count <= originalLength)
        {
            Conveyor newNet = new Conveyor();
            newNet.BuildingOutput = conveyorToRoute.BuildingOutput;
            newNet.BuildingInput = conveyorToRoute.BuildingInput;

            foreach (Point p in newPath)
            {
                newNet.Tiles.Add(originalMap.Tiles[p.X, p.Y]);
            }

            originalMap.Remove(conveyorToRoute);
            originalMap.Add(newNet, newNet.BuildingInput, newNet.BuildingOutput);

            return true;
        }
        else
        {
            return false;
        }
    }*/


    public static bool Autoroute(Map map, Conveyor originalNet, Point conveyorStartFacing, Point conveyorEndFacing)
    {
        var ret = GetAutoroutingSolution(map, originalNet);
        
        if (ret == null)
            return false;

        foreach (var netRouting in ret)
        {
            netRouting.Key.CreateSegmentsForPath(netRouting.Value, conveyorStartFacing, conveyorEndFacing);
        }
        
        return true;
    }

    static List<Conveyor> GetNetsInTheWay(Map originalMap, Conveyor netToRoute, out bool routingComplete)
    {
        AutoroutingMap map = new AutoroutingMap(originalMap);
        AutoroutingNet net = new AutoroutingNet();

        net.Start = map.tiles[netToRoute._start.X, netToRoute._start.Y];
        net.End = map.tiles[netToRoute._end.X, netToRoute._end.Y];
            
        List<AutoroutingNet> netIdsInTheWay = new List<AutoroutingNet>();

        var autorouteResult = Autoroute(map, net, netIdsInTheWay);
        routingComplete = autorouteResult != null;

        List<Conveyor> ret = netIdsInTheWay.Select(x => x.originalConveyor).ToList();

        foreach(Conveyor c in originalMap._conveyors)
        {
            if (c == netToRoute)
                continue;

            if(c.GetOccupiedPoints().Where(x => x == netToRoute._start || x == netToRoute._end).Count() > 0)
            {
                ret.Add(c);
            }
        }

        return ret;
    }

    static Dictionary<Conveyor, List<Point>> GetAutoroutingSolution(Map originalMap, Conveyor originalNet)
    {
        //originalMap.Remove(originalNet);
            
            
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        bool autorouteSuccess = false;
        List<Conveyor> netIdsInTheWay = GetNetsInTheWay(originalMap, originalNet, out autorouteSuccess);
        if(netIdsInTheWay.Contains(null))
        {

        }
        int bestCost = Int32.MaxValue;
        Conveyor[] bestPermutation = null;
        List<List<Point>> bestRouting = null;

        List<Conveyor> netsToPermute = new List<Conveyor>();
        netsToPermute.AddRange(netIdsInTheWay);
        netsToPermute.Add(originalNet);

        Parallel.ForEach<IEnumerable<Conveyor>>(netsToPermute.GetPermutations(), permutation =>
        {
            if (stopwatch.ElapsedMilliseconds >= 50 && (bestCost < Int32.MaxValue || autorouteSuccess))
                return;
            else if (stopwatch.ElapsedMilliseconds >= 500)
                return;

            AutoroutingMap possibleMap = new AutoroutingMap(originalMap);

            AutoroutingNet originalNet2 = new AutoroutingNet();
            originalNet2.Start = possibleMap.tiles[originalNet._start.X, originalNet._start.Y];
            originalNet2.End = possibleMap.tiles[originalNet._end.X, originalNet._end.Y];

            foreach (Conveyor x in netIdsInTheWay)
                possibleMap.RipupNet(x);
            possibleMap.RipupNet(originalNet2);
                
            int cost = 0;

            List<List<Point>> routing = new List<List<Point>>();

            foreach (Conveyor n in permutation)
            {
                List<Point> result = Autoroute(possibleMap, possibleMap.tiles[n._start.X, n._start.Y].Net);

                if (result == null)
                    return; //Bad permutation, can't route everything
                routing.Add(result);
                cost += result.Count;
            }

            if (cost < bestCost)
            {
                bestCost = cost;
                bestRouting = routing;
                bestPermutation = permutation.ToArray();
            }
        });

        Dictionary<Conveyor, List<Point>> ret = new Dictionary<Conveyor, List<Point>>();

        if (bestCost != Int32.MaxValue)
        {
            for (int i = 0; i < bestPermutation.Length; i++)
            {
                if (bestPermutation[i] == originalNet)
                    ret.Add(originalNet, bestRouting[i]);
                else
                    ret.Add(bestPermutation[i], bestRouting[i]);
            }
                
            return ret;
        }
        else
        {
            return null;
        }
    }

    static List<Point> Autoroute(AutoroutingMap map, AutoroutingNet net, List<AutoroutingNet> netIdsInTheWay = null)
    {
        Func<Point, float> costEvaluator = delegate (Point to)
        {
            if (to.X == net.End.X && to.Y == net.End.Y || to.X == net.Start.X && to.Y == net.Start.Y)
                return 1;
            else if (map.tiles[to.X, to.Y].Net != null)
            {
                if (netIdsInTheWay != null && !netIdsInTheWay.Contains(map.tiles[to.X, to.Y].Net))
                    netIdsInTheWay.Add(map.tiles[to.X, to.Y].Net);

                return float.PositiveInfinity;
            }
            else
                return map.tiles[to.X, to.Y].Cost;
        };

        if (net.Start.Net != null && net.Start.Net != net)
            return null;
        if (net.End.Net != null && net.End.Net != net)
            return null;

        net.Start.Net = null;
        net.End.Net = null;

        var aStarResult = CornersReductedAStar.Find(map.Width, map.Height, costEvaluator, net.Start.X, net.Start.Y, net.End.X, net.End.Y);

        if(aStarResult != null)
            foreach (Point p in aStarResult)
                map.tiles[p.X, p.Y].Net = net;

        return aStarResult;
    }
}