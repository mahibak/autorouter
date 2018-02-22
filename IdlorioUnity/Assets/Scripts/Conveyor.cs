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

    public float _desiredItemsPerSecond;
    public float _itemsPerSecond;
    public Item _item;

    public float _length;

    public float _itemDrawTimeSeconds = 0;

    public float Satisfaction
    {
        get
        {
            if (_desiredItemsPerSecond == 0)
                return 1;
            else if (System.Single.IsPositiveInfinity(_desiredItemsPerSecond))
                return 0;
            else
                return _itemsPerSecond / _desiredItemsPerSecond;
        }
    }

    public Conveyor(MachineConnector output, MachineConnector input)
    {
        _output = output;
        _input = input;
        _start = _output.GetWorldPositionOneTileOut();
        _end = _input.GetWorldPositionOneTileOut();
    }

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
        _itemDrawTimeSeconds += dt * _itemsPerSecond;

        _segments.Foreach(x => x.DrawDebug(_itemDrawTimeSeconds));
    }

    public void CreateSegmentsForPath(IEnumerable<Point> path)
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
        firstSegment._startCurveDirection = _output.GetWorldPosition().DirectionTo(firstSegment._start);

        ConveyorSegment lastSegment = _segments.Last();
        lastSegment._endCurveDirection = lastSegment._end.DirectionTo(_input.GetWorldPosition());

        if((lastSegment._end - lastSegment._start).ManhattanLength == 0)
        {
            lastSegment._startCurveDirection = lastSegment._linearDirection;
            lastSegment._linearDirection = lastSegment._endCurveDirection;
        }

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
