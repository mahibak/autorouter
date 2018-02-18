using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Conveyor
{
    public List<ConveyorSegment> _segments = new List<ConveyorSegment>();
    public Point _start;
    public Point _end;
    public MachineConnector _input = null;
    public MachineConnector _output = null;
    public float _length;

    public float _itemDrawTimeSeconds = 0;

    public IEnumerable<Point> GetOccupiedPoints()
    {
        foreach (IEnumerable<Point> p in _segments.Select(x => x.GetOccupiedPoints()))
        {
            foreach (Point p1 in p)
                yield return p1;
        }
    }

    public void DrawDebug(float dt)
    {
        _itemDrawTimeSeconds += dt * _input._itemsPerSecond;

        _segments.Foreach(x => x.DrawDebug(_itemDrawTimeSeconds));
    }

    public void CreateSegmentsForPath(IEnumerable<Point> path, Point conveyorStartFacing, Point conveyorEndFacing)
    {
        _segments.Clear();

        foreach (Point[] segments in path.GetCorners().ScanWindow(2))
        {
            ConveyorSegment s = new ConveyorSegment();
            s._start = segments[0];
            s._end = segments[1];
            s._linearDirection = s._start.DirectionTo(s._end);
            _segments.Add(s);
        }

        //Trim starts
        for (int i = 0; i < _segments.Count; i++)
        {
            ConveyorSegment s = _segments[i];

            if (_segments.First() != s)
                s._start += s._linearDirection;
        }

        //Set curve directions
        foreach (ConveyorSegment[] segments in _segments.ScanWindow(2))
        {
            segments[0]._endCurveDirection = segments[0]._end.DirectionTo(segments[1]._start);
            segments[0]._startCurveDirection = segments[0]._linearDirection;
            segments[1]._startCurveDirection = segments[1]._linearDirection;
        }
        ConveyorSegment firstSegment = _segments.First();
        firstSegment._startCurveDirection = conveyorStartFacing.DirectionTo(firstSegment._start);

        ConveyorSegment lastSegment = _segments.Last();
        lastSegment._endCurveDirection = lastSegment._end.DirectionTo(conveyorEndFacing);

        //Set start 'phase'
        float length = 0;
        foreach (ConveyorSegment s in _segments)
        {
            s._startLength = length;
            length += s.Length;
        }

        _length = length;
    }
}
