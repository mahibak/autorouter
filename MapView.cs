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

        enum UxStates
        {
            Idle,
            StartedRouting,
        }
        UxStates uxState = UxStates.Idle;

        public Map map;
        public MapRenderer mapRenderer;

        public void SetMap(Map map)
        {
            this.map = map;
            mapRenderer = new MapRenderer(map);

            MouseClick += OnMouseClick;
            MouseMove += MapView_MouseMove;
            Paint += MapView_Paint;
        }
        
        private void OnTileHovered(int tileX, int tileY)
        {
            switch (uxState)
            {
                case UxStates.StartedRouting:
                    map.RemoveNet(netBeingRouted);

                    if (!map.tiles[tileX, tileY].IsNetTip)
                    {
                        netBeingRouted.End = map.tiles[tileX, tileY];
                    }

                    Autorouting.Autorouter.Autoroute(map, netBeingRouted);

                    Refresh();
                    break;
                    
                default:
                    break;
            }
        }

        Point lastHoveredTile = new Point(-1, -1);
        private void MapView_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = mapRenderer.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
                return;
            if (p == lastHoveredTile)
                return;
           
            OnTileHovered(p.X, p.Y);
            lastHoveredTile = p;
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

            OnTileClicked(p.X, p.Y, e.X, e.Y, e.Button);
        }

        Net netBeingRouted;
        
        void OnTileClicked(int tileX, int tileY, int mouseX, int mouseY, MouseButtons button)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    if (map.tiles[tileX, tileY].Net != null)
                    {
                        map.RemoveNet(map.tiles[tileX, tileY].Net);
                    }
                    else
                    {
                        uxState = UxStates.StartedRouting;
                        netBeingRouted = new Net();
                        
                        netBeingRouted.Start = map.tiles[tileX, tileY];
                        netBeingRouted.End = netBeingRouted.Start;
                        
                        Cursor = Cursors.Cross;
                    }

                    Refresh();
                    break;

                case UxStates.StartedRouting:
                    if (map.tiles[tileX, tileY].Net == netBeingRouted)
                    {
                        netBeingRouted = null;
                    }
                    else
                    {
                        map.RemoveNet(netBeingRouted);
                    }
                    uxState = UxStates.Idle;
                    Cursor = Cursors.Arrow;
                    break;

                default:
                    uxState = UxStates.Idle;
                    Cursor = Cursors.Arrow;
                    break;
            }
        }
    }
}
