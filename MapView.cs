using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Idlorio
{
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
        
        public Map map;
        public MapRenderer mapRenderer;
        public UX ux;

        public void SetMap(Map map)
        {
            this.map = map;
            mapRenderer = new MapRenderer(map);
            ux = new UX(map);
            
            MouseMove += MapView_MouseMove;
            Paint += MapView_Paint;
            MouseDown += MapView_MouseDown;
            MouseUp += MapView_MouseUp;
        }

        private void MapView_MouseUp(object sender, MouseEventArgs e)
        {
            Point p = mapRenderer.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
                return;

            ux.OnTileUp(p.X, p.Y);

            Refresh();

            switch (ux.uxState)
            {
                case UX.UxStates.Routing:
                    Cursor = Cursors.Cross;
                    break;

                default:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void MapView_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = mapRenderer.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
                return;

            ux.OnTileDown(p.X, p.Y);
        }

        Point lastHoveredTile = new Point(-1, -1);
        private void MapView_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = mapRenderer.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
                return;
            if (p == lastHoveredTile)
                return;
           
            ux.OnTileHovered(p.X, p.Y);
            lastHoveredTile = p;

            Refresh();
        }

        private void MapView_Paint(object sender, PaintEventArgs e)
        {
            mapRenderer.Draw(e.Graphics);
        }
    }
}
