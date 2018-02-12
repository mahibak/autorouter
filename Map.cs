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
        public Tile[,] tiles;
        public List<Net> Nets = new List<Net>();
        
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

            tiles = new Tile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tiles[x, y] = new Tile(x, y);
                }
            }
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


        /*
         public bool IsInMap(Point p)
         {
             return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
         }

         public Point GetNetEnd(int netId)
         {
             for (int y = Height - 1; y >= 0; y--)
             {
                 for (int x = Width - 1; x >= 0; x--)
                 {
                     if (tiles[x, y].netId == netId && new Point(x, y).GetOrthogonalNeighbors().Where(z => IsInMap(z) && tiles[z.X, z.Y].netId == netId).Count() == 1)
                         return new Point(x, y);
                 }
             }

             return new Point(-1, -1);
         }

         public Tile GetTile(Point p)
         {
             return tiles[p.X, p.Y];
         }*/
    }
}
