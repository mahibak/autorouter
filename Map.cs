using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    public class Map
    {
        public Tile[,] Tiles;
        public List<Net> Nets = new List<Net>();
        public List<Building> Buildings = new List<Building>();

        public int Width
        {
            get; private set;
        }

        public int Height
        {
            get; private set;
        }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            Tiles = new Tile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tiles[x, y] = new Tile(x, y);
                }
            }
        }

        public void Add(Building building)
        {
            Buildings.Add(building);
            building.GetTiles().Foreach(x => x.Building = building);
        }

        public void Remove(Building building)
        {
            Buildings.Remove(building);
            building.GetTiles().Foreach(x => x.Building = null);
        }

        public void RemoveNet(Net net)
        {
            while(net.Tiles.Count > 0)
            {
                net.Tiles[0].Net = null;
                net.Tiles.RemoveAt(0);
            }

            if(Nets.Contains(net))
                Nets.Remove(net);
        }

        public bool IsInMap(Point p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
        }
    }
}
