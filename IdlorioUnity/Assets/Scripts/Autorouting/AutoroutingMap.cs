using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AutoroutingMap
{
    public int Width
    {
        get; private set;
    }

    public int Height
    {
        get; private set;
    }

    public AutoroutingTile[,] tiles;

    public List<AutoroutingNet> nets = new List<AutoroutingNet>();

    public int RipupNet(AutoroutingNet net)
    {
        int removals = System.Math.Max(0, net.Tiles.Count - 2);

        while (net.Tiles.Count > 2)
        {
            net.Tiles[1].Net = null;
            net.Tiles.RemoveAt(1);
        }

        net.Start.Net = net;
        net.End.Net = net;

        net.Tiles.Clear();
        net.Tiles.Add(net.Start);
        net.Tiles.Add(net.End);

        return removals;
    }

    public int RipupNet(Conveyor net)
    {
        return RipupNet(tiles[net._start.X, net._start.Y].Net);
    }

    public AutoroutingMap(Map map)
    {
        Width = map.Width;
        Height = map.Height;

        tiles = new AutoroutingTile[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                tiles[x, y] = new AutoroutingTile(x, y);
                tiles[x, y].Cost = 1;
            }
        }

        foreach (Machine m in map.Machines)
        {
            foreach(Point p in m.GetOccupiedPoints())
            {
                tiles[p.X, p.Y].Cost = Single.PositiveInfinity;
            }
        }

        foreach (Conveyor n in map._conveyors)
        {
            AutoroutingNet autoroutingNet = new AutoroutingNet();
            foreach (ConveyorSegment t in n._segments)
            {
                foreach (Point p in t._start.GetPointsTo(t._end))
                {
                    autoroutingNet.Tiles.Add(tiles[p.X, p.Y]);
                    tiles[p.X, p.Y].Net = autoroutingNet;
                }
            }
            autoroutingNet.Start = tiles[n._start.X, n._start.Y];
            autoroutingNet.End = tiles[n._end.X, n._end.Y];
            autoroutingNet.originalConveyor = n;
            nets.Add(autoroutingNet);
        }
    }

    public AutoroutingMap(AutoroutingMap map)
    {
        Width = map.Width;
        Height = map.Height;

        tiles = new AutoroutingTile[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                tiles[x, y] = new AutoroutingTile(x, y);

                tiles[x, y].Cost = map.tiles[x, y].Cost;
            }
        }

        foreach (AutoroutingNet n in map.nets)
        {
            AutoroutingNet autoroutingNet = new AutoroutingNet();
            foreach (AutoroutingTile t in n.Tiles)
            {
                autoroutingNet.Tiles.Add(tiles[t.X, t.Y]);
                tiles[t.X, t.Y].Net = autoroutingNet;
            }
            autoroutingNet.Start = tiles[n.Start.X, n.Start.Y];
            autoroutingNet.End = tiles[n.End.X, n.End.Y];
            nets.Add(autoroutingNet);
        }
    }

    public bool IsInMap(Point p)
    {
        return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
    }
        
    public AutoroutingTile GetTile(Point p)
    {
        return tiles[p.X, p.Y];
    }
}
