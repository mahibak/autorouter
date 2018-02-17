using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Conveyor
{
    public List<ConveyorSegment> ConveyorSegments = new List<ConveyorSegment>();
    public Point Start;
    public Point End;
    public MachineConnector BuildingInput = null;
    public MachineConnector BuildingOutput = null;

    public IEnumerable<Point> GetOccupiedPoints()
    {
        foreach (IEnumerable<Point> p in ConveyorSegments.Select(x => x.GetOccupiedPoints()))
        {
            foreach (Point p1 in p)
                yield return p1;
        }
    }

    public void DrawDebug()
    {
        ConveyorSegments.Foreach(x => x.DrawDebug());
    }
}
