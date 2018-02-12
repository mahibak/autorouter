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

        public UX(Map map)
        {
            this.map = map;
        }

        public enum UxStates
        {
            Idle,
            StartedRouting,
        }
        public UxStates uxState = UxStates.Idle;

        Net netBeingRouted;

        public void OnTileHovered(int tileX, int tileY)
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
                    break;

                default:
                    break;
            }
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
                default:
                    break;
            }

            //uxState = UxStates.StartedRouting;
            //netBeingRouted = new Net();

            //netBeingRouted.Start = map.tiles[tileX, tileY];
            //netBeingRouted.End = netBeingRouted.Start;
        }

        public void OnTileClicked(int tileX, int tileY)
        {
            Point point = new Point(tileX, tileY);

            if (map.tiles[tileX, tileY].Net != null)
            {
                OnNetClicked(map.tiles[tileX, tileY].Net, point);
            }
            else if (map.tiles[tileX, tileY].Building != null)
            {
                OnBuildingClicked(map.tiles[tileX, tileY].Building, point);
            }
            else
            {
                OnFuckallClicked(point);
            }
            /*
            switch (uxState)
            {
                case UxStates.Idle:

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
                    break;

                default:
                    uxState = UxStates.Idle;
                    break;
            }*/
        }
    }
}
