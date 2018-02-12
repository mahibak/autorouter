using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio.Autorouting
{
    public class AutoroutingNet
    {
        public List<AutoroutingTile> Tiles = new List<AutoroutingTile>();
        public AutoroutingTile Start = null;
        public AutoroutingTile End = null;
        
        public AutoroutingNet GetSameNetIn(AutoroutingMap map)
        {
            return map.tiles[Start.X, Start.Y].Net;
        }
    }
}
