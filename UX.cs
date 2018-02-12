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
        BuildingInputOutput inputOutputBeingRouted;
        int x, y;

        public void Draw(Graphics g)
        {
            new Point(10, 10).LineTo(new Point(x, y)).Foreach(x => new MapRenderer(map).DrawTile(map.Tiles[x.X, x.Y], g, Color.Tomato));
        }

        public void OnTileHovered(int tileX, int tileY)
        {
            x = tileX;
            y = tileY;

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
                    break;
                default:
                    break;
            }
        }

        public void OnBuildingHeld(Building building, System.Drawing.Point point)
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

        public void OnNetClicked(Net net, Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    map.RemoveNet(net);
                    break;

                case UxStates.Routing:
                    if (map.Tiles[point.X, point.Y].BuildingInput != null)
                    {
                        map.RemoveNet(net);
                        OnInputOutputClicked(map.Tiles[point.X, point.Y].BuildingInput, point);
                    }
                    else if(map.Tiles[point.X, point.Y].BuildingOutput != null)
                    {
                        map.RemoveNet(net);
                        OnInputOutputClicked(map.Tiles[point.X, point.Y].BuildingOutput, point);
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnNetHeld(Net net, System.Drawing.Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    Autorouting.Autorouter.Reroute(map, net);
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
                    Building b = new Building(map, new Point(point.X, point.Y));

                    if (!b.IsIntersectingThings())
                        map.Add(b);
                    break;
                    
                default:
                    break;
            }
        }

        public void OnFuckallHeld(System.Drawing.Point point)
        {

        }

        System.Diagnostics.Stopwatch clickStopwatch = new System.Diagnostics.Stopwatch();

        Point tileDownPoint;

        public void OnMouseDown(int tileX, int tileY)
        {
            clickStopwatch.Reset();
            clickStopwatch.Start();
            tileDownPoint = new Point(tileX, tileY);
        }

        public void OnMouseUp(int tileX, int tileY)
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
            if (map.Tiles[point.X, point.Y].Net != null)
            {
                OnNetHeld(map.Tiles[point.X, point.Y].Net, point);
            }
            else if (map.Tiles[point.X, point.Y].Building != null)
            {
                OnBuildingHeld(map.Tiles[point.X, point.Y].Building, point);
            }
            else
            {
                OnFuckallHeld(point);
            }
        }

        void OnInputOutputClicked(BuildingInputOutput io, Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    if (map.Tiles[point.X, point.Y].Net != null)
                        map.RemoveNet(map.Tiles[point.X, point.Y].Net);

                    uxState = UxStates.Routing;
                    netBeingRouted = new Net();
                    inputOutputBeingRouted = io;
                    netBeingRouted.Start = map.Tiles[point.X, point.Y];
                    netBeingRouted.End = netBeingRouted.Start;
                    break;

                case UxStates.Routing:
                    if (inputOutputBeingRouted == io)
                    {
                        netBeingRouted = null;
                        inputOutputBeingRouted = null;
                        uxState = UxStates.Idle;
                        return;
                    }
                    if (inputOutputBeingRouted.IsInput == io.IsInput)
                        return;
                    if (inputOutputBeingRouted.Building == io.Building)
                        return;

                    if (map.Tiles[point.X, point.Y].Net != null)
                        map.RemoveNet(map.Tiles[point.X, point.Y].Net);

                    netBeingRouted.End = map.Tiles[point.X, point.Y];
                    if (Autorouting.Autorouter.Autoroute(map, netBeingRouted))
                    {
                        netBeingRouted = null;
                        inputOutputBeingRouted = null;
                        uxState = UxStates.Idle;
                    }
                    break;

                default:
                    break;
            }
        }
        

        public void OnTileClicked(Point point)
        {
            if (map.Tiles[point.X, point.Y].Building != null)
            {
                OnBuildingClicked(map.Tiles[point.X, point.Y].Building, point);
                return;
            }

            Building buildingThatThisTileWouldBeDirectlyAdjacentTo = point.GetOrthogonalNeighbors().Where(x => map.IsInMap(x)).Where(x => map.Tiles[x.X, x.Y].Building != null).Select(x => map.Tiles[x.X, x.Y].Building).FirstOrDefault();
            if (buildingThatThisTileWouldBeDirectlyAdjacentTo != null)
            {
                BuildingInput input = buildingThatThisTileWouldBeDirectlyAdjacentTo.Inputs.Where(x => x.Position == point).FirstOrDefault();
                if (input != null)
                    OnInputOutputClicked(input, point);
                else
                {
                    BuildingOutput output = buildingThatThisTileWouldBeDirectlyAdjacentTo.Outputs.Where(x => x.Position == point).FirstOrDefault();
                    if (output != null)
                        OnInputOutputClicked(output, point);
                }
                return;
            }

            if (map.Tiles[point.X, point.Y].Net != null)
            {
                OnNetClicked(map.Tiles[point.X, point.Y].Net, point);
                return;
            }

            OnFuckallClicked(point);
        }
    }
}
