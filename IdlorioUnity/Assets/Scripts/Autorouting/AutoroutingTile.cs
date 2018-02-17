using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AutoroutingTile
{
    public readonly int X;
    public readonly int Y;
    public AutoroutingNet Net;
    public float Cost = 1;

    public AutoroutingTile(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override string ToString()
    {
        return String.Format("({0}, {1}) {2}{3}", X, Y, Cost, Net != null ? " Net" : "");
    }
}
