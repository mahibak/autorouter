using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    public class Tile
    {
        public readonly int X;
        public readonly int Y;
        public Net Net = null;
        public Building Building = null;
        public BuildingInput BuildingInput = null;
        public BuildingOutput BuildingOutput = null;

        public bool IsNetTip
        {
            get
            {
                return Net != null && (Net.Start == this || Net.End == this);
            }
        }

        public Tile(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
