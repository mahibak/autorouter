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
            public readonly int x;
            public readonly int y;

            public int pathId = 0;

            public Tile(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public Color[] colors = new Color[] { Color.White, Color.CornflowerBlue, Color.Fuchsia, Color.LimeGreen, Color.OliveDrab, Color.DarkSalmon };

            public Color color
            {
                get
                {
                    return colors[pathId];
                }
            }
        }

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
            g.FillRectangle(new SolidBrush(t.color), t.x * TILE_WIDTH, (HEIGHT - 1 - t.y) * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);
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
