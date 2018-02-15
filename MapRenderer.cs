using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    public class MapRenderer
    {
        const int TILE_WIDTH = 50;

        Map map;

        public MapRenderer(Map map)
        {
            this.map = map;
        }

        public Point PixelToTile(int x, int y)
        {
            return new Point(x / TILE_WIDTH, map.Height - y / TILE_WIDTH - 1);
        }

        public void DrawTile(Tile t, Graphics g, Color color)
        {
            g.FillRectangle(new SolidBrush(color), t.X * TILE_WIDTH, (map.Height - 1 - t.Y) * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);
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

        void DrawNets(Graphics g)
        {
            for (int i = 0; i < map.Nets.Count; i++)
            {
                foreach (Tile t in map.Nets[i].Tiles)
                    DrawTile(t, g, ColorFromHSV(new System.Random(i).NextDouble(), 1, 1));
            }
        }

        void DrawBuildings(Graphics g)
        {
            foreach (Machine b in map.Buildings)
            {
                b.GetTiles().Foreach(x => DrawTile(x, g, Color.DarkGray));
            }
        }

        void DrawGrid(Graphics g)
        {
            for (int y = 0; y <= map.Height; y++)
            {
                g.DrawLine(Pens.Black, 0, y * TILE_WIDTH, map.Width * TILE_WIDTH, y * TILE_WIDTH);
            }

            for (int x = 0; x <= map.Width; x++)
            {
                g.DrawLine(Pens.Black, x * TILE_WIDTH, 0, x * TILE_WIDTH, map.Height * TILE_WIDTH);
            }
        }

        void DrawInputsOutputs(Graphics g)
        {
            foreach (Machine b in map.Buildings)
            {
                b._inputSlots.Foreach(t => g.FillEllipse(Brushes.LightBlue, (t.Position.X + new Point(t.Direction).X * 0.5f) * TILE_WIDTH, (map.Height - 1 - t.Position.Y - new Point(t.Direction).Y * 0.5f) * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH));
                b._outputSlots.Foreach(t => g.FillEllipse(Brushes.DodgerBlue, (t.Position.X + new Point(t.Direction).X * 0.5f) * TILE_WIDTH, (map.Height - 1 - t.Position.Y - new Point(t.Direction).Y * 0.5f) * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH));
            }
        }

        public void Draw(Graphics g)
        {
            g.Clear(Color.White);
            DrawNets(g);
            DrawGrid(g);
            DrawBuildings(g);
            DrawInputsOutputs(g);
        }
    }
}
