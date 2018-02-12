using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void OnTileClicked(int tileX, int tileY)
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
                    }
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
            }
        }
    }
}
