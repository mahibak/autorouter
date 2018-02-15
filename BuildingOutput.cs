using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Idlorio
{
    public class MachineOutput : MachineInputOutput
    {
        public MachineOutput(Machine Machine, Point Position) : base(Machine, Position, false)
        {
        }

        public MachineInput MachineInput
        {
            get
            {
                if (Net != null)
                    return Net.MachineInput;
                else if (FirstTileOut.MachineInput != null)
                    return FirstTileOut.MachineInput;
                else
                    return null;
            }
        }
    }
}
