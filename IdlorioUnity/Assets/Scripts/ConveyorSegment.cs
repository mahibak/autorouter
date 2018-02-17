using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSegment
{
    public Point _start = new Point(1, 1);
    public Point _end = new Point(5, 1);

    public void DrawDebug()
    {
        if(_start.X == _end.X)
            GDK.DrawFilledAABB(new Vector3(_start.X + 0.5f, 0.1f, (System.Math.Abs(_start.Y + _end.Y)) / 2.0f + 0.5f), new Vector3(System.Math.Abs(_end.X - _start.X) + 1, 0.1f, System.Math.Abs(_end.Y - _start.Y) + 1) / 2, Color.magenta);
        else
            GDK.DrawFilledAABB(new Vector3((_start.X + _end.X) / 2.0f + 0.5f, 0.1f, _start.Y + 0.5f), new Vector3(System.Math.Abs(_end.X - _start.X) + 1, 0.1f, System.Math.Abs(_end.Y - _start.Y) + 1) / 2, Color.magenta);
    }

    public IEnumerable<Point> GetOccupiedPoints()
    {
        return _start.GetPointsTo(_end);
    }
}
