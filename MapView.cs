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
                    if (map.tiles[tileX, tileY].pathId != 0 && map.tiles[tileX, tileY].pathId != lastPathId + 1)
                        return;


                        for (int x = 0; x < Map.WIDTH; x++)
                        for (int y = 0; y < Map.HEIGHT; y++)
                            if(map.tiles[x, y].pathId == lastPathId + 1)
                            {
                                map.tiles[x, y].pathId = 0;
                            }


                    Func<AStar.Point, AStar.Point, float> costEvaluator = delegate(AStar.Point from, AStar.Point to) 
                    {
                    if (map.tiles[to.X, to.Y].pathId != 0)
                        return float.PositiveInfinity;
                    else 
                        return 1;
                    };

                    var ret = new AStar(Map.WIDTH, Map.HEIGHT, costEvaluator).Find(drawStart.X, drawStart.Y, tileX, tileY);

                    List<Point> corners = ret.GetCorners().ToList();

                    for (int i = 0; i < corners.Count - 3; i++)
                    {
                        Point newPoint = new Point(corners[i + 0].X, corners[i + 3].Y);
                        Point newPoint2 = new Point(corners[i + 3].X, corners[i + 0].Y);

                        if (corners[i + 0].GetPointsTo(newPoint).All(x => map.tiles[x.X, x.Y].pathId == 0) && newPoint.GetPointsTo(corners[i + 3]).All(x => map.tiles[x.X, x.Y].pathId == 0))
                        {
                            corners.RemoveAt(i + 1);
                            corners.RemoveAt(i + 1);
                            corners.Insert(i + 1, newPoint);
                            i--;
                        }
                        else if (corners[i + 0].GetPointsTo(newPoint2).All(x => map.tiles[x.X, x.Y].pathId == 0) && newPoint2.GetPointsTo(corners[i + 3]).All(x => map.tiles[x.X, x.Y].pathId == 0))
                        {
                            corners.RemoveAt(i + 1);
                            corners.RemoveAt(i + 1);
                            corners.Insert(i, newPoint2);
                            i--;
                        }
                    }

                    var ret2 = corners.PointsFromCorners().ToList();

                    foreach (Point p in ret2)
                    {
                        map.tiles[p.X, p.Y].pathId = lastPathId + 1;
                    } 

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

        Point drawStart;
        int lastPathId = 0;

        void OnTileClicked(int tileX, int tileY, int mouseX, int mouseY, MouseButtons button)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    uxState = UxStates.StartedDraw;
                    drawStart = new Point(tileX, tileY);
                    Cursor = Cursors.Cross;
                    break;

                default:
                    uxState = UxStates.Idle;
                    Cursor = Cursors.Arrow;
                    lastPathId++;
                    break;
            }
        }
    }
}
