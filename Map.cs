using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorouter
{
    public class Map
    {
        public class Tile
        {
            public readonly int X;
            public readonly int Y;
            public int netId = 0;

            public Tile(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public Point GetPoint()
            {
                return new Point(X, Y);
            }
            
            public static Color ColorFromHSV(double hue, double saturation, double value)
            {
                hue *= 360;

                int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
                double f = hue / 60 - Math.Floor(hue / 60);

                value = value * 255;
                int v = Convert.ToInt32(value);
                int p = Convert.ToInt32(value * (1 - saturation));
                int q = Convert.ToInt32(value * (1 - f * saturation));
                int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

                if (hi == 0)
                    return Color.FromArgb(255, v, t, p);
                else if (hi == 1)
                    return Color.FromArgb(255, q, v, p);
                else if (hi == 2)
                    return Color.FromArgb(255, p, v, t);
                else if (hi == 3)
                    return Color.FromArgb(255, p, q, v);
                else if (hi == 4)
                    return Color.FromArgb(255, t, p, v);
                else
                    return Color.FromArgb(255, v, p, q);
            }

            public Color color
            {
                get
                {
                    if (netId != 0)
                        return ColorFromHSV(new Random(netId).NextDouble(), 1, 1);
                    return Color.White;
                }
            }

            public Tile GetCopy()
            {
                return new Tile(X, Y) { netId = this.netId };
            }
        }

        public const int WIDTH = 30;
        public const int HEIGHT = 20;

        public Tile[,] tiles = new Tile[WIDTH, HEIGHT];

        const int TILE_WIDTH = 50;
        internal int nextMapId = 1;

        public int RipupNet(int netId)
        {
            int removals = 0;

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (tiles[x, y].netId == netId)
                    {
                        tiles[x, y].netId = 0;
                        removals++;
                    }
                }
            }

            return removals;
        }

        public Map()
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    tiles[x, y] = new Tile(x, y);
                }
            }
        }

        public Map GetCopy()
        {
            Map ret = new Map();

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    ret.tiles[x, y] = tiles[x, y].GetCopy();
                }
            }

            ret.nextMapId = nextMapId;

            return ret;
        }

        void DrawTile(Tile t, Graphics g)
        {
            g.FillRectangle(new SolidBrush(t.color), t.X * TILE_WIDTH, (HEIGHT - 1 - t.Y) * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);
        }

        void DrawTiles(Graphics g)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    DrawTile(tiles[x, y], g);
                }
            }
        }

        void DrawGrid(Graphics g)
        {
            for (int y = 0; y <= HEIGHT; y++)
            {
                g.DrawLine(Pens.Black, 0, y * TILE_WIDTH, WIDTH * TILE_WIDTH, y * TILE_WIDTH);
            }

            for (int x = 0; x <= WIDTH; x++)
            {
                g.DrawLine(Pens.Black, x * TILE_WIDTH, 0, x * TILE_WIDTH, HEIGHT * TILE_WIDTH);
            }
        }

        public void Draw(Graphics g)
        {
            DrawTiles(g);
            DrawGrid(g);
        }

        int GetNetLength(int netId)
        {
            int ret = 0;

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (tiles[x, y].netId == netId)
                        ret++;
                }
            }

            return ret;
        }

        public bool IsInMap(Point p)
        {
            return p.X >= 0 && p.X < WIDTH && p.Y >= 0 && p.Y < HEIGHT;
        }

        public Point GetNetStart(int netId)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (tiles[x, y].netId == netId && new Point(x, y).GetOrthogonalNeighbors().Where(z => IsInMap(z) && tiles[z.X, z.Y].netId == netId).Count() == 1)
                        return new Point(x, y);
                }
            }

            return new Point(-1, -1);
        }

        public bool IsNetTip(Point p)
        {
            return p.GetOrthogonalNeighbors().Where(z => IsInMap(z) && tiles[z.X, z.Y].netId == GetTile(p).netId).Count() <= 1;
        }

        public Point GetNetEnd(int netId)
        {
            for (int y = HEIGHT - 1; y >= 0; y--)
            {
                for (int x = WIDTH - 1; x >= 0; x--)
                {
                    if (tiles[x, y].netId == netId && new Point(x, y).GetOrthogonalNeighbors().Where(z => IsInMap(z) && tiles[z.X, z.Y].netId == netId).Count() == 1)
                        return new Point(x, y);
                }
            }

            return new Point(-1, -1);
        }

        public Point PixelToTile(int x, int y)
        {
            return new Point(x / TILE_WIDTH, HEIGHT - y / TILE_WIDTH - 1);
        }

        public Tile GetTile(Point p)
        {
            return tiles[p.X, p.Y];
        }

        public void AutorouteWithRemoval(Tile start, Tile end, int netId)
        {
            RipupNet(netId);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            if (IsNetTip(start.GetPoint()) || IsNetTip(end.GetPoint()))
                return;

            List<int> netIdsInTheWay = new List<int>();

            int ret = Autoroute(start, end, netId, netIdsInTheWay);

            if (netIdsInTheWay.Count == 0)
                return;

            netIdsInTheWay.Add(netId);

            int bestCost = Int32.MaxValue;
            int[] bestPermutation = null;

            Parallel.ForEach<IEnumerable<int>>(netIdsInTheWay.GetPermutations(), permutation =>
            //netIdsInTheWay.GetPermutations().Foreach(permutation =>
            {
                if (stopwatch.ElapsedMilliseconds >= 50 && (bestCost < Int32.MaxValue || ret > 0))
                    return;
                else
                if (stopwatch.ElapsedMilliseconds >= 500)
                    return;

                Map possibleMap = GetCopy();
                
                permutation.Foreach(x => { possibleMap.RipupNet(x); possibleMap.GetTile(x == netId ? start.GetPoint() : GetNetStart(x)).netId = x; possibleMap.GetTile(x == netId ? end.GetPoint() : GetNetEnd(x)).netId = x; });

                List<int> costs = permutation.Select(x => possibleMap.Autoroute(possibleMap.GetTile(x == netId ? start.GetPoint() : GetNetStart(x)),possibleMap.GetTile(x == netId ? end.GetPoint() : GetNetEnd(x)), x)).ToList();

                if(costs.Where(x => x == 0).Count() > 0)
                    return; //Bad permutation, can't route everything

                if (costs.Sum() < bestCost)
                {
                    bestCost = costs.Sum();
                    bestPermutation = permutation.ToArray();
                }
            });

            if (bestCost != Int32.MaxValue)
            {
                List<Point> starts = new List<Point>();
                List<Point> ends = new List<Point>();

                foreach (var x in bestPermutation)
                {
                    starts.Add(x == netId ? start.GetPoint() : GetNetStart(x));
                    ends.Add(x == netId ? end.GetPoint() : GetNetEnd(x));
                    RipupNet(x);
                }

                for(int i = 0; i < starts.Count; i++)
                {
                    GetTile(starts[i]).netId = bestPermutation[i];
                    GetTile(ends[i]).netId = bestPermutation[i];
                }

                for(int i = 0; i < bestPermutation.Length; i++)
                    if(Autoroute(GetTile(starts[i]), GetTile(ends[i]), bestPermutation[i]) == 0)
                    {

                    }
            }
            else
            {
                //Routing failed
            }
        }

        public int Autoroute(Tile start, Tile end, int netId, List<int> netIdsInTheWay = null)
        {
            RipupNet(netId);
            
            Func<AStar.Point, AStar.Point, float> costEvaluator = delegate (AStar.Point from, AStar.Point to)
            {
                if (tiles[to.X, to.Y].netId != 0)
                {
                    if(netIdsInTheWay != null && !netIdsInTheWay.Contains(tiles[to.X, to.Y].netId))
                        netIdsInTheWay.Add(tiles[to.X, to.Y].netId);
                    return float.PositiveInfinity;
                }
                else
                    return 1;
            };

            var aStarResult = new AStar(Map.WIDTH, Map.HEIGHT, costEvaluator).Find(start.X, start.Y, end.X, end.Y);

            if (aStarResult == null)
                return 0;

            List<Point> corners = aStarResult.GetCorners().ToList();

            for (int i = 0; i < corners.Count - 3; i++)
            {
                Point newPoint = new Point(corners[i + 0].X, corners[i + 3].Y);
                Point newPoint2 = new Point(corners[i + 3].X, corners[i + 0].Y);

                if (corners[i + 0].GetPointsTo(newPoint).All(x => tiles[x.X, x.Y].netId == 0) && newPoint.GetPointsTo(corners[i + 3]).All(x => tiles[x.X, x.Y].netId == 0))
                {
                    corners.RemoveAt(i + 1);
                    corners.RemoveAt(i + 1);
                    corners.Insert(i + 1, newPoint);
                    i--;
                }
                else if (corners[i + 0].GetPointsTo(newPoint2).All(x => tiles[x.X, x.Y].netId == 0) && newPoint2.GetPointsTo(corners[i + 3]).All(x => tiles[x.X, x.Y].netId == 0))
                {
                    corners.RemoveAt(i + 1);
                    corners.RemoveAt(i + 1);
                    corners.Insert(i + 1, newPoint2);
                    i--;
                }
            }

            var optimisedREsult = corners.PointsFromCorners().ToList();

            foreach (Point p in optimisedREsult)
                tiles[p.X, p.Y].netId = netId;

            return optimisedREsult.Count - 1;
        }
    }
}
