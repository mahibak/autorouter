using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Idlorio
{
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel1, new object[] { true });
        }
        
        public Map map;
        public MapRenderer mapRenderer;
        public UX ux;

        public void SetMap(Map map)
        {
            this.map = map;
            mapRenderer = new MapRenderer(map);
            ux = new UX(map);

            panel1.MouseMove += MapView_MouseMove;
            panel1.MouseDown += MapView_MouseDown;
            panel1.MouseUp += MapView_MouseUp;
            panel1.Paint += MapView_Paint;

            txtMax.TextChanged += TxtMax_TextChanged;
        }

        private void TxtMax_TextChanged(object sender, EventArgs e)
        {
            if (selectedBuilding == null)
                return;

            double newValue = 0;
            if (!Double.TryParse(((TextBox)sender).Text, out newValue))
            {
                ((TextBox)sender).BackColor = Color.Tomato;
                return;
            }

            selectedBuilding.MaximumItemsPerSecond = newValue;
            ((TextBox)sender).BackColor = DefaultBackColor;
        }

        private void MapView_MouseUp(object sender, MouseEventArgs e)
        {
            Point p = mapRenderer.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
                return;

            ux.OnMouseUp(p.X, p.Y);

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

            ux.OnMouseDown(p.X, p.Y);
        }

        Point lastHoveredTile = new Point(-1, -1);
        Building selectedBuilding = null;

        private void MapView_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = mapRenderer.PixelToTile(e.X, e.Y);
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
                return;
            if (p == lastHoveredTile)
                return;
           
            ux.OnTileHovered(p.X, p.Y);
            lastHoveredTile = p;

            if(map.Tiles[p.X, p.Y].Building != null)
            {
                selectedBuilding = map.Tiles[p.X, p.Y].Building;
                StringBuilder sb = new StringBuilder();
                txtMax.Text = selectedBuilding.MaximumItemsPerSecond.ToString();
                txtDesired.Text = selectedBuilding.DesiredItemsPerSecond.ToString();
                txtActual.Text = selectedBuilding.ItemsPerSecond.ToString();
            }

            Refresh();
        }

        private void MapView_Paint(object sender, PaintEventArgs e)
        {
            mapRenderer.Draw(e.Graphics);
            ux.Draw(e.Graphics);
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            ProductionSpeedComputation.UpdateProductionSpeed(map.Buildings);
        }
    }
}
