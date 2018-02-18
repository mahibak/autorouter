using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Conveyor
{
    public List<ConveyorSegment> ConveyorSegments = new List<ConveyorSegment>();
    public Point _start;
    public Point _end;
    public MachineConnector _input = null;
    public MachineConnector _output = null;
    public float _length;

    public float _itemDrawTimeSeconds = 0;

    public IEnumerable<Point> GetOccupiedPoints()
    {
        foreach (IEnumerable<Point> p in ConveyorSegments.Select(x => x.GetOccupiedPoints()))
        {
            foreach (Point p1 in p)
                yield return p1;
        }
    }

    public void DrawDebug(float dt)
    {
        _itemDrawTimeSeconds += dt;

        ConveyorSegments.Foreach(x => x.DrawDebug(_itemDrawTimeSeconds));
    }
}
