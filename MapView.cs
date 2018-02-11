using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autorouter
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
            StartedDraw,
        }
        UxStates uxState = UxStates.Idle;
        
        Map map;

        ContextMenuStrip tileContextMenuStrip = new ContextMenuStrip();

        public void SetMap(Map map)
        {
            this.map = map;
            MouseClick += OnMouseClick;
            MouseMove += MapView_MouseMove;
            Paint += MapView_Paint;
        }
        
        private void OnTileHovered(int tileX, int tileY)
        {
            switch (uxState)
            {
                case UxStates.StartedDraw:
                    //if (map.tiles[tileX, tileY].net != null && map.tiles[tileX, tileY].net != netBeingRouted)
                    //    return;
                    map.AutorouteWithRemoval(drawStart, map.tiles[tileX, tileY], netIdBeingRouted);

                    Refresh();
                    break;
                    
                default:
                    break;
            }
        }

        Point lastHoveredTile = new Point(-1, -1);
        private void MapView_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = map.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= Map.WIDTH || p.Y < 0 || p.Y >= Map.HEIGHT)
                return;
            if (p == lastHoveredTile)
                return;
           
            OnTileHovered(p.X, p.Y);
            lastHoveredTile = p;
        }

        private void MapView_Paint(object sender, PaintEventArgs e)
        {
            map.Draw(e.Graphics);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            Point p = map.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= Map.WIDTH || p.Y < 0 || p.Y >= Map.HEIGHT)
                return;

            OnTileClicked(p.X, p.Y, e.X, e.Y, e.Button);
        }

        Map.Tile drawStart;
        int netIdBeingRouted;

        void OnTileClicked(int tileX, int tileY, int mouseX, int mouseY, MouseButtons button)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    if (map.tiles[tileX, tileY].netId != 0)
                    {
                        map.RipupNet(map.tiles[tileX, tileY].netId);
                        Refresh();
                    }
                    else
                    {
                        uxState = UxStates.StartedDraw;
                        drawStart = map.tiles[tileX, tileY];
                        netIdBeingRouted = map.nextMapId++;
                        Cursor = Cursors.Cross;
                        Refresh();
                    }
                    
                    break;

                default:
                    uxState = UxStates.Idle;
                    Cursor = Cursors.Arrow;
                    break;
            }
        }
    }
}
