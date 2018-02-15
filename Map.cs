using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    public class Map
    {
        public Tile[,] Tiles;
        public List<Net> Nets = new List<Net>();
        public List<Machine> Buildings = new List<Machine>();
        public List<BuildingInput> BuildingInputs = new List<BuildingInput>();
        public List<BuildingOutput> BuildingOutputs = new List<BuildingOutput>();

        public int Width
        {
            get; private set;
        }

        public int Height
        {
            get; private set;
        }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            Tiles = new Tile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tiles[x, y] = new Tile(x, y);
                }
            }

            for (int x = 0; x < Width; x += 4)
            {
                Machine b = new Machine(this, new Point(x, 0), new Idlorio.Point(1, 1));
                b.AddInput(b.Position, Direction.Up);
                b._maximumItemsPerSecond = Double.PositiveInfinity;
                Add(b);

                b = new Machine(this, new Point(x, height - 1), new Idlorio.Point(1, 1));
                b.AddOutput(b.Position, Direction.Down);
                b._maximumItemsPerSecond = Double.PositiveInfinity;
                Add(b);
            }
        }

        public void Add(Machine building)
        {
            Buildings.Add(building);
            building.GetTiles().Foreach(x => x.Building = building);
            BuildingInputs.AddRange(building._inputSlots);
            BuildingOutputs.AddRange(building._outputSlots);
            building._inputSlots.ForEach(x => Tiles[x.Position.X, x.Position.Y].BuildingInput = x);
            building._outputSlots.ForEach(x => Tiles[x.Position.X, x.Position.Y].BuildingOutput = x);
        }

        public void Remove(Machine building)
        {
            Buildings.Remove(building);
            building.GetTiles().Foreach(x => x.Building = null);

            foreach(BuildingInput input in building._inputSlots)
            {
                BuildingInputs.Remove(input);
                Tiles[input.Position.X, input.Position.Y].BuildingInput = null;

                if (input.Net != null)
                    Remove(input.Net);
            }
            
            foreach (BuildingOutput output in building._outputSlots)
            {
                BuildingOutputs.Remove(output);
                Tiles[output.Position.X, output.Position.Y].BuildingOutput = null;

                if (output.Net != null)
                    Remove(output.Net);
            }
        }

        public bool IsInMap(Point p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
        }

        public void Add(Net net, BuildingInputOutput io1, BuildingInputOutput io2)
        {
            if (io1.IsInput)
                Add(net, (BuildingInput)io1, (BuildingOutput)io2);
            else
                Add(net, (BuildingInput)io2, (BuildingOutput)io1);
        }

        public void Add(Net net, BuildingInput input, BuildingOutput output)
        {
            foreach (Tile t in net.Tiles)
                t.Net = net;

            net.BuildingInput = input;
            net.BuildingOutput = output;

            input.Net = net;
            output.Net = net;

            Nets.Add(net);
        }

        public void Remove(Net net)
        {
            while (net.Tiles.Count > 0)
            {
                net.Tiles[0].Net = null;
                net.Tiles.RemoveAt(0);
            }

            if (Nets.Contains(net))
                Nets.Remove(net);

            if(net.BuildingInput != null)
                net.BuildingInput.Net = null;
            if (net.BuildingOutput != null)
                net.BuildingOutput.Net = null;

            net.BuildingInput = null;
            net.BuildingOutput = null;
        }
    }
}
