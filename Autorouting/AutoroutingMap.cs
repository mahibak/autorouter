using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio.Autorouting
{
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
            int removals = Math.Max(0, net.Tiles.Count - 2);

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

        public int RipupNet(Net net)
        {
            return RipupNet(tiles[net.Start.X, net.Start.Y].Net);
        }

        public AutoroutingMap(Idlorio.Map map)
        {
            Width = map.Width;
            Height = map.Height;

            tiles = new AutoroutingTile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tiles[x, y] = new AutoroutingTile(x, y);
                }
            }

            foreach (Net n in map.Nets)
            {
                AutoroutingNet autoroutingNet = new AutoroutingNet();
                foreach (Tile t in n.Tiles)
                {
                    autoroutingNet.Tiles.Add(tiles[t.X, t.Y]);
                    tiles[t.X, t.Y].Net = autoroutingNet;
                }
                autoroutingNet.Start = tiles[n.Start.X, n.Start.Y];
                autoroutingNet.End = tiles[n.End.X, n.End.Y];
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
}
