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
        
        BuildingInputOutput inputOutputBeingRouted;

        public void Draw(Graphics g)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    break;
                case UxStates.Routing:
                    new MapRenderer(map).DrawTile(inputOutputBeingRouted.Tile, g, Color.Orange);
                    break;
                default:
                    break;
            }
        }

        public void OnTileHovered(int tileX, int tileY)
        {
        }

        public void OnBuildingClicked(Building building, Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    break;
                default:
                    break;
            }
        }

        public void OnBuildingHeld(Building building, Point point)
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
                    Autorouting.Autorouter.Reroute(map, net);
                    break;

                default:
                    break;
            }
        }

        public void OnNetHeld(Net net, Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    map.Remove(net);
                    break;
                    
                default:
                    break;
            }
        }

        public void OnFuckallClicked(Point point)
        {
            switch (uxState)
            {
                case UxStates.Idle:
                    Building b = new Building(map, new Point(point.X, point.Y));
                    b.AddRandomInputsOutputs();
                    if (!b.IsIntersectingThings())
                        map.Add(b);
                    break;
                    
                default:
                    break;
            }
        }

        public void OnFuckallHeld(Point point)
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
                    uxState = UxStates.Routing;
                    inputOutputBeingRouted = io;
                    break;

                case UxStates.Routing:
                    if (inputOutputBeingRouted == io)
                    {
                        inputOutputBeingRouted = null;
                        uxState = UxStates.Idle;
                        return;
                    }
                    if (inputOutputBeingRouted.IsInput == io.IsInput)
                        return;
                    if (inputOutputBeingRouted.Building == io.Building)
                        return;

                    if (inputOutputBeingRouted.Net != null)
                        map.Remove(inputOutputBeingRouted.Net);

                    if (io.Net != null)
                        map.Remove(io.Net);


                    Net netBeingRouted = new Net();
                    netBeingRouted.Start = inputOutputBeingRouted.FirstTileOut;
                    netBeingRouted.End = io.FirstTileOut;
                    if (Autorouting.Autorouter.Autoroute(map, netBeingRouted))
                    {
                        map.Add(netBeingRouted, inputOutputBeingRouted, io);
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
            BuildingInput BuildingInputClicked = map.BuildingInputs.Where(x => x.Position == point).FirstOrDefault();
            if (BuildingInputClicked != null)
            {
                OnInputOutputClicked(BuildingInputClicked, point);
                return;
            }
            BuildingOutput BuildingOutputClicked = map.BuildingOutputs.Where(x => x.Position == point).FirstOrDefault();
            if (BuildingOutputClicked != null)
            {
                OnInputOutputClicked(BuildingOutputClicked, point);
                return;
            }
            
            if (map.Tiles[point.X, point.Y].Building != null)
            {
                OnBuildingClicked(map.Tiles[point.X, point.Y].Building, point);
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
