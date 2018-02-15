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
        public BuildingInput(Machine Building, Point Position) : base(Building, Position, true)
        {

        }

        public BuildingOutput BuildingOutput
        {
            get
            {
                if (Net != null)
                    return Net.BuildingOutput;
                else if (FirstTileOut.BuildingOutput != null)
                    return FirstTileOut.BuildingOutput;
                else
                    return null;
            }
        }
    }
}
