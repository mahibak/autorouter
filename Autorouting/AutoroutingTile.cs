using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio.Autorouting
{
    public class AutoroutingTile
    {
        public readonly int X;
        public readonly int Y;
        public AutoroutingNet Net;
        public bool Praticable = true;

        public AutoroutingTile(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
