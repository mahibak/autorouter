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
        public Map Map;

        public List<BuildingInput> Inputs = new List<BuildingInput>();
        public List<BuildingOutput> Outputs = new List<BuildingOutput>();

        public double MaximumItemsPerSecond = 0;
        public double DesiredItemsPerSecond = 0;
        public double ItemsPerSecond = 0;

        void SetRandomDirection(BuildingInputOutput BuildingInputOutput)
        {
            List<Tile> corners = GetCorners().ToList();

            if (corners.Contains(BuildingInputOutput.Tile))
            {
                switch (corners.Count)
                {
                    case 1:
                        BuildingInputOutput.Direction = new Direction[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left }.Random();
                        break;

                    case 2:
                        if (Size.Y == 1)
                        {
                            if (BuildingInputOutput.Tile == corners[0])
                                BuildingInputOutput.Direction = new Direction[] { Direction.Up, Direction.Down, Direction.Left }.Random();
                            else
                                BuildingInputOutput.Direction = new Direction[] { Direction.Up, Direction.Down, Direction.Right }.Random();
                        }
                        else
                        {
                            if (BuildingInputOutput.Tile == corners[0])
                                BuildingInputOutput.Direction = new Direction[] { Direction.Up, Direction.Right, Direction.Left }.Random();
                            else
                                BuildingInputOutput.Direction = new Direction[] { Direction.Left, Direction.Down, Direction.Right }.Random();
                        }
                        break;

                    case 4:
                        if (corners[0] == BuildingInputOutput.Tile)
                            BuildingInputOutput.Direction = new Direction[] { Direction.Left, Direction.Up }.Random();
                        else if (corners[1] == BuildingInputOutput.Tile)
                            BuildingInputOutput.Direction = new Direction[] { Direction.Up, Direction.Right }.Random();
                        else if (corners[2] == BuildingInputOutput.Tile)
                            BuildingInputOutput.Direction = new Direction[] { Direction.Down, Direction.Right }.Random();
                        else if (corners[3] == BuildingInputOutput.Tile)
                            BuildingInputOutput.Direction = new Direction[] { Direction.Down, Direction.Left }.Random();
                        break;
                    default:
                        break;
                }
            }
            else if (Size.X == 1)
                BuildingInputOutput.Direction = new Direction[] { Direction.Left, Direction.Right }.Random();
            else if (Size.Y == 1)
                BuildingInputOutput.Direction = new Direction[] { Direction.Up, Direction.Down }.Random();
            else if (BuildingInputOutput.Position.X == Position.X)
                BuildingInputOutput.Direction = Direction.Left;
            else if (BuildingInputOutput.Position.X == Position.X + Size.X - 1)
                BuildingInputOutput.Direction = Direction.Right;
            else if (BuildingInputOutput.Position.Y == Position.Y)
                BuildingInputOutput.Direction = Direction.Up;
            else if (BuildingInputOutput.Position.Y == Position.Y - (Size.Y - 1))
                BuildingInputOutput.Direction = Direction.Down;
        }

        public Building(Map map, Point position) : this(map, position, new Point(Random.Next(1, Math.Min(5, map.Width - position.X)), Random.Next(1, Math.Min(5, position.Y + 1))))
        {
        }

        public void AddInput(Point position, Direction direction)
        {
            BuildingInput buildingInput = new BuildingInput(this, position);
            buildingInput.Direction = direction;
            Inputs.Add(buildingInput);
        }

        public void AddOutput(Point position, Direction direction)
        {
            BuildingOutput buildingOutput = new BuildingOutput(this, position);
            buildingOutput.Direction = direction;
            Outputs.Add(buildingOutput);
        }

        public void AddRandomInputsOutputs()
        {
            List<Tile> ioPossibilites = GetEdges().Randomized();

            int iosToPlace = Math.Min(Size.X * Size.Y, Math.Max(2, Random.Next(0, (int)Math.Ceiling(ioPossibilites.Count / 1.5))));
            int inputsToPlace = Random.Next(iosToPlace > 1 ? 1 : 0, iosToPlace > 1 ? iosToPlace - 1 : 2);
            int outputsToPlace = iosToPlace - inputsToPlace;

            for (int i = 0; i < inputsToPlace; i++)
            {
                Tile t = ioPossibilites.First();
                ioPossibilites.RemoveAt(0);
                BuildingInput input = new BuildingInput(this, new Point(t.X, t.Y));
                SetRandomDirection(input);
                Inputs.Add(input);
            }

            for (int i = 0; i < outputsToPlace; i++)
            {
                Tile t = ioPossibilites.First();
                ioPossibilites.RemoveAt(0);
                BuildingOutput output = new BuildingOutput(this, new Point(t.X, t.Y));
                SetRandomDirection(output);
                Outputs.Add(output);
            }
        }

        public Building(Map map, Point position, Point size)
        {
            this.Map = map;
            this.Position = position;
            this.Size = size;
        }
        
        public Point Size;
        public Point Position; //Top left corner

        public IEnumerable<Tile> GetTiles()
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    yield return Map.Tiles[Position.X + x, Position.Y - y];
        }

        public IEnumerable<Tile> GetCorners()
        {
            //Top left
            yield return Map.Tiles[Position.X, Position.Y];

            //Top right
            if(Size.X > 1)
                yield return Map.Tiles[Position.X + Size.X - 1, Position.Y];

            if(Size.Y > 1)
            {
                //Bottom right
                yield return Map.Tiles[Position.X + Size.X - 1, Position.Y - (Size.Y - 1)];

                //Bottom left
                if(Size.X > 1)
                    yield return Map.Tiles[Position.X, Position.Y - (Size.Y - 1)];
            }
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
                    yield return Map.Tiles[Position.X + x, Position.Y - Size.Y + 1];

            //Left
            if (Size.X > 1)
                for (int y = Size.Y - 2; y > 0; y--)
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
