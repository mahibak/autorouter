using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Idlorio
{
    public class BuildingInput : BuildingInputOutput
    {
        public BuildingInput(Building Building, Point Position) : base(Building, Position, true)
        {

        }
    }
}
