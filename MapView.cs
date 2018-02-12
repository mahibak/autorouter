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

            MouseClick += OnMouseClick;
            MouseMove += MapView_MouseMove;
            Paint += MapView_Paint;
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

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            Point p = mapRenderer.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
                return;

            ux.OnTileClicked(p.X, p.Y);

            Refresh();

            switch (ux.uxState)
            {
                case UX.UxStates.StartedRouting:
                    Cursor = Cursors.Cross;
                    break;

                default:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }
    }
}
