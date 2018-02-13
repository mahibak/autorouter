using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Idlorio
{
    public class BuildingInputOutput
    {
        public Building Building;
        public Point Position;
        public Direction Direction;

        public Map Map
        {
            get
            {
                return Building?.Map;
            }
        }

        public Tile Tile
        {
            get
            {
                return Map?.Tiles[Position.X, Position.Y];
            }
        }

        public BuildingInputOutput(Building Building, Point Position, bool IsInput)
        {
            this.Building = Building;
            this.Position = Position;
            this.IsInput = IsInput;
        }

        public bool IsInput
        {
            get;
            private set;
        }
    }
}
