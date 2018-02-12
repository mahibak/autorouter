using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Idlorio
{
    public class Building
    {
        Map Map;

        public List<BuildingInput> Inputs = new List<BuildingInput>();
        public List<BuildingOutput> Outputs = new List<BuildingOutput>();

        public Building(Map map, Point position)
        {
            this.Map = map;
            this.Position = position;

            Size = new System.Drawing.Point(Extensions.Random.Next(1, 5), Extensions.Random.Next(1, 5));

            List<Tile> ioPossibilites = GetOneOutFromTheEdges().Randomized();

            int iosToPlace = Math.Max(2, Extensions.Random.Next(0, (int)Math.Ceiling(ioPossibilites.Count / 1.5)));
            int inputsToPlace = Extensions.Random.Next(1, iosToPlace - 1);
            int outputsToPlace = iosToPlace - inputsToPlace;

            for (int i = 0; i < inputsToPlace; i++)
            {
                Tile t = ioPossibilites.First();
                ioPossibilites.RemoveAt(0);
                BuildingInput input = new BuildingInput(this, new System.Drawing.Point(t.X, t.Y));
                Inputs.Add(input);
            }

            for (int i = 0; i < outputsToPlace; i++)
            {
                Tile t = ioPossibilites.First();
                ioPossibilites.RemoveAt(0);
                BuildingOutput output = new BuildingOutput(this, new System.Drawing.Point(t.X, t.Y));
                Outputs.Add(output);
            }
        }
        

        public Point Size;
        public Point Position; //Top left corner

        public IEnumerable<Tile> GetTiles()
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    yield return Map.Tiles[Position.X + x, Position.Y - y];
        }

        public IEnumerable<Tile> GetEdges() //Clockwise from top left
        {
            //Top
            for (int x = 0; x < Size.X; x++)
                yield return Map.Tiles[Position.X + x, Position.Y];

            //Right
            for (int y = 1; y < Size.Y; y++)
                yield return Map.Tiles[Position.X + Size.X - 1, Position.Y - y];

            //Bottom
            if (Size.Y > 1)
                for (int x = Size.X - 2; x >= 0; x--)
                    yield return Map.Tiles[Position.X + x, Position.Y - Size.Y - 1];

            //Left
            if (Size.X > 1)
                for (int y = Size.Y - 2; y >= 0; y--)
                    yield return Map.Tiles[Position.X, Position.Y - y];
        }

        public IEnumerable<Tile> GetOneOutFromTheEdges() //Clockwise from top
        {
            //Top
            if(Position.Y + 1 < Map.Height)
                for (int x = 0; x < Size.X; x++)
                    yield return Map.Tiles[Position.X + x, Position.Y + 1];

            //Right
            if(Position.X + Size.X < Map.Width)
                for (int y = 0; y < Size.Y; y++)
                    yield return Map.Tiles[Position.X + Size.X, Position.Y - y];

            //Bottom
            if(Position.Y - Size.Y >= 0)
                for (int x = Size.X - 1; x >= 0; x--)
                    yield return Map.Tiles[Position.X + x, Position.Y - Size.Y];

            //Left
            if(Position.X - 1 >= 0)
                for (int y = Size.Y - 1; y >= 0; y--)
                    yield return Map.Tiles[Position.X - 1, Position.Y - y];
        }

        public bool IsIntersectingThings()
        {
            if (Position.X + Size.X - 1 >= Map.Width || Position.Y - Size.Y + 1 < 0)
                return true;

            return !GetTiles().All(x => x.Net == null && x.Building == null);
        }
    }
}
