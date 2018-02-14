using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Idlorio
{
    public class BuildingOutput : BuildingInputOutput
    {
        public BuildingOutput(Building Building, Point Position) : base(Building, Position, false)
        {
        }

        public BuildingInput BuildingInput
        {
            get
            {
                if (Net != null)
                    return Net.BuildingInput;
                else if (FirstTileOut.BuildingInput != null)
                    return FirstTileOut.BuildingInput;
                else
                    return null;
            }
        }
    }
}
