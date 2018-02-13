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
        public List<Building> Buildings = new List<Building>();
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
        }

        public void Add(Building building)
        {
            Buildings.Add(building);
            building.GetTiles().Foreach(x => x.Building = building);
            BuildingInputs.AddRange(building.Inputs);
            BuildingOutputs.AddRange(building.Outputs);
            building.Inputs.ForEach(x => Tiles[x.Position.X, x.Position.Y].BuildingInput = x);
            building.Outputs.ForEach(x => Tiles[x.Position.X, x.Position.Y].BuildingOutput = x);
        }

        public void Remove(Building building)
        {
            Buildings.Remove(building);
            building.GetTiles().Foreach(x => x.Building = null);
            building.Inputs.ForEach(x => { BuildingInputs.Remove(x); Tiles[x.Position.X, x.Position.Y].BuildingInput = null; });
            building.Outputs.ForEach(x => { BuildingOutputs.Remove(x); Tiles[x.Position.X, x.Position.Y].BuildingOutput = null; });
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

            net.Input = input;
            net.Output = output;

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

            if(net.Input != null)
                net.Input.Net = null;
            if (net.Output != null)
                net.Output.Net = null;

            net.Input = null;
            net.Output = null;
        }
    }
}
