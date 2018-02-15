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
        public Machine Building;
        public Point Position;
        public Direction Direction;
        public Net Net;

        public Tile FirstTileOut
        {
            get
            {
                Point point = (Position + Direction);
                return Map?.Tiles[point.X, point.Y];
            }
        }

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

        public BuildingInputOutput(Machine Building, Point Position, bool IsInput)
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
