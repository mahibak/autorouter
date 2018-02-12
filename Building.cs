using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    public class Building
    {
        Map map;

        public Building(Map map)
        {
            this.map = map;
            Size = new System.Drawing.Point(2, 2);
        }

        public System.Drawing.Point Size;
        public System.Drawing.Point Position; //Top left corner

        public IEnumerable<Tile> GetTiles()
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    yield return map.tiles[Position.X + x, Position.Y - y];
        }

        public bool IsIntersectingThings()
        {
            return !GetTiles().All(x => x.Net == null && x.Building == null);
        }
    }
}
