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

            public Net net;

            public Tile(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public Color[] colors = new Color[] { Color.White, Color.CornflowerBlue, Color.Fuchsia, Color.LimeGreen, Color.OliveDrab, Color.DarkSalmon };

            public Color color
            {
                get
                {
                    if (net != null)
                        return net.color;
                    return Color.White;
                }
            }
        }

        public class Net
        {
            List<Tile> tiles = new List<Tile>();

            public void Ripup()
            {
                while(tiles.Count > 0)
                {
                    tiles[0].net = null;
                    tiles.RemoveAt(0);
                }
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

            public Net()
            {
                color = ColorFromHSV(new Random().NextDouble(), 1, 1);
            }

            public bool Autoroute(Tile start, Tile to, Map map)
            {
                Ripup();
                
                Func<AStar.Point, AStar.Point, float> costEvaluator = delegate (AStar.Point from, AStar.Point to2)
                {
                    if (map.tiles[to2.X, to2.Y].net != null)
                        return float.PositiveInfinity;
                    else
                        return 1;
                };

                var ret = new AStar(Map.WIDTH, Map.HEIGHT, costEvaluator).Find(start.X, start.Y, to.X, to.Y);

                if (ret == null)
                    return false;

                List<Point> corners = ret.GetCorners().ToList();

                for (int i = 0; i < corners.Count - 3; i++)
                {
                    Point newPoint = new Point(corners[i + 0].X, corners[i + 3].Y);
                    Point newPoint2 = new Point(corners[i + 3].X, corners[i + 0].Y);

                    if (corners[i + 0].GetPointsTo(newPoint).All(x => map.tiles[x.X, x.Y].net == null) && newPoint.GetPointsTo(corners[i + 3]).All(x => map.tiles[x.X, x.Y].net == null))
                    {
                        corners.RemoveAt(i + 1);
                        corners.RemoveAt(i + 1);
                        corners.Insert(i + 1, newPoint);
                        i--;
                    }
                    else if (corners[i + 0].GetPointsTo(newPoint2).All(x => map.tiles[x.X, x.Y].net == null) && newPoint2.GetPointsTo(corners[i + 3]).All(x => map.tiles[x.X, x.Y].net == null))
                    {
                        corners.RemoveAt(i + 1);
                        corners.RemoveAt(i + 1);
                        corners.Insert(i + 1, newPoint2);
                        i--;
                    }
                }

                var ret2 = corners.PointsFromCorners().ToList();

                foreach (Point p in ret2)
                {
                    map.tiles[p.X, p.Y].net = this;
                    tiles.Add(map.tiles[p.X, p.Y]);
                }

                return true;
            }
            
            public Color color
            {
                get; set;
            }
        }

        public List<Net> Nets = new List<Net>();

        public const int WIDTH = 30;
        public const int HEIGHT = 20;

        public Tile[,] tiles = new Tile[WIDTH, HEIGHT];

        const int TILE_WIDTH = 50;

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

        public Point PixelToTile(int x, int y)
        {
            return new Point(x / TILE_WIDTH, HEIGHT - y / TILE_WIDTH - 1);
        }
    }
}
