using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Idlorio
{
    public class UX
    {
        Map map;

        const int LONG_PRESS_DURATION_MS = 250;

        public UX(Map map)
        {
            this.map = map;
        }

        public enum UxStates
        {
            Idle,
            Routing,
        }
        public UxStates uxState = UxStates.Idle;

        Net netBeingRouted;

        public void OnTileHovered(int tileX, int tileY)
        {
            /*switch (uxState)
            {
                case UxStates.StartedRouting:
                    map.RemoveNet(netBeingRouted);

                    if (!map.tiles[tileX, tileY].IsNetTip)
                    {
                        netBeingRouted.End = map.tiles[tileX, tileY];
                    }

                    Autorouting.Autorouter.Autoroute(map, netBeingRouted);
                    break;

                default:
                    break;
            }*/
        }

        public void OnBuildingClicked(Building building, System.Drawing.Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    map.Remove(building);
                    break;
                default:
                    break;
            }
        }

        public void OnNetClicked(Net net, System.Drawing.Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    map.RemoveNet(net);
                    break;

                case UxStates.Routing:
                    if (map.tiles[point.X, point.Y].IsNetTip)
                        return; //Ignore, can't route here

                    netBeingRouted.End = map.tiles[point.X, point.Y];
                    if (Autorouting.Autorouter.Autoroute(map, netBeingRouted))
                    {
                        netBeingRouted = null;
                        uxState = UxStates.Idle;
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnFuckallClicked(System.Drawing.Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    Building b = new Building(map);
                    b.Position = new System.Drawing.Point(point.X, point.Y);

                    if (!b.IsIntersectingThings())
                        map.Add(b);
                    break;

                case UxStates.Routing:
                    netBeingRouted.End = map.tiles[point.X, point.Y];
                    if(Autorouting.Autorouter.Autoroute(map, netBeingRouted))
                    {
                        netBeingRouted = null;
                        uxState = UxStates.Idle;
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnFuckallHeld(System.Drawing.Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    uxState = UxStates.Routing;
                    netBeingRouted = new Net();

                    netBeingRouted.Start = map.tiles[point.X, point.Y];
                    netBeingRouted.End = netBeingRouted.Start;
                    break;
                default:
                    break;
            }
        }

        System.Diagnostics.Stopwatch clickStopwatch = new System.Diagnostics.Stopwatch();

        Point tileDownPoint;

        public void OnTileDown(int tileX, int tileY)
        {
            clickStopwatch.Reset();
            clickStopwatch.Start();
            tileDownPoint = new Point(tileX, tileY);
        }

        public void OnTileUp(int tileX, int tileY)
        {
            if (tileX != tileDownPoint.X || tileY != tileDownPoint.Y)
                return;

            if (clickStopwatch.ElapsedMilliseconds < LONG_PRESS_DURATION_MS)
                OnTileClicked(tileDownPoint);
            else
                OnHeld(tileDownPoint);
        }

        public void OnHeld(Point point)
        {
            if (map.tiles[point.X, point.Y].Net != null)
            {
                //OnNetClicked(map.tiles[point.X, point.Y].Net, point);
            }
            else if (map.tiles[point.X, point.Y].Building != null)
            {
                //OnBuildingClicked(map.tiles[point.X, point.Y].Building, point);
            }
            else
            {
                OnFuckallHeld(point);
            }
        }

        public void OnTileClicked(Point point)
        {
            if (map.tiles[point.X, point.Y].Net != null)
            {
                OnNetClicked(map.tiles[point.X, point.Y].Net, point);
            }
            else if (map.tiles[point.X, point.Y].Building != null)
            {
                OnBuildingClicked(map.tiles[point.X, point.Y].Building, point);
            }
            else
            {
                OnFuckallClicked(point);
            }
        }
    }
}
