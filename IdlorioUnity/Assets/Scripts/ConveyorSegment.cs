using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ConveyorSegment
{
    public Point _start = new Point(1, 1);
    public Point _end = new Point(5, 1);
    public float _startLength = 0;

    const float BOX_SPACING = 1f;

    void DrawBox(float time)
    {
        if (time < StartLength)
        {
            if (_linearDirection == _startCurveDirection)
            {
                GDK.DrawFilledAABB(_start.ToVector3() + new Point(_linearDirection).ToVector3() * (time - 0.5f) + new Vector3(0.5f, 1.0f, 0.5f), new Vector3(0.25f, 0.25f, 0.25f), Color.yellow);
            }
            else
            {
                Vector3 entry = _start.ToVector3() - new Point(_startCurveDirection).ToVector3() * 0.5f;
                Vector3 centerOfRotation = entry + new Point(_linearDirection).ToVector3() * 0.5f;

                float angleToDo = Vector3.SignedAngle(new Point(_startCurveDirection).ToVector3(), new Point(_linearDirection).ToVector3(), new Vector3(0, 1, 0)); ;

                Matrix4x4 mat = Matrix4x4.Rotate(Quaternion.Euler(0, angleToDo * (time / StartLength), 0));

                Vector3 displacement = mat.MultiplyVector(entry - centerOfRotation);

                Vector3 positionToDraw = centerOfRotation + displacement;

                GDK.DrawFilledAABB(positionToDraw + new Vector3(0.5f, 1.0f, 0.5f), new Vector3(0.25f, 0.25f, 0.25f), Color.yellow);
            }
        }
        else if (time < Length - EndLength)
        {
            GDK.DrawFilledAABB(_start.ToVector3() + new Point(_linearDirection).ToVector3() * (time - StartLength + 0.5f) + new Vector3(0.5f, 1.0f, 0.5f), new Vector3(0.25f, 0.25f, 0.25f), Color.yellow);
        }
        else
        {
            if (_linearDirection == _endCurveDirection)
            {
                GDK.DrawFilledAABB(_start.ToVector3() + new Point(_linearDirection).ToVector3() * (time - 0.5f) + new Vector3(0.5f, 1.0f, 0.5f), new Vector3(0.25f, 0.25f, 0.25f), Color.yellow);
            }
            else
            {
                Vector3 entry = _end.ToVector3() - new Point(_linearDirection).ToVector3() * 0.5f;
                Vector3 centerOfRotation = entry + new Point(_endCurveDirection).ToVector3() * 0.5f;

                float angleToDo = Vector3.SignedAngle(new Point(_linearDirection).ToVector3(), new Point(_endCurveDirection).ToVector3(), new Vector3(0, 1, 0)); ;

                Matrix4x4 mat = Matrix4x4.Rotate(Quaternion.Euler(0, angleToDo * ((time - Length + EndLength) / EndLength), 0));

                Vector3 displacement = mat.MultiplyVector(entry - centerOfRotation);

                Vector3 positionToDraw = centerOfRotation + displacement;

                GDK.DrawFilledAABB(positionToDraw + new Vector3(0.5f, 1.0f, 0.5f), new Vector3(0.25f, 0.25f, 0.25f), Color.yellow);
            }
        }
    }

    public void DrawDebug(float relativeTime)
    {
        if(_start.GetPointsTo(_end).Contains(InputManager.GetPointerTile()))
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("Start at {0} from {1}", _start, _startCurveDirection));
            sb.AppendLine(string.Format("Linear to {0}", _linearDirection));
            sb.AppendLine(string.Format("Ends  at {0} from {1}", _end, _endCurveDirection));

            GDK.DrawText(sb.ToString(), _start.ToVector3(), Color.black);
        }

        if (_start.X == _end.X)
            GDK.DrawFilledAABB(new Vector3(_start.X + 0.5f, 0.1f, (System.Math.Abs(_start.Y + _end.Y)) / 2.0f + 0.5f), new Vector3(System.Math.Abs(_end.X - _start.X) + 1, 0.1f, System.Math.Abs(_end.Y - _start.Y) + 1) / 2, Color.magenta);
        else
            GDK.DrawFilledAABB(new Vector3((_start.X + _end.X) / 2.0f + 0.5f, 0.1f, _start.Y + 0.5f), new Vector3(System.Math.Abs(_end.X - _start.X) + 1, 0.1f, System.Math.Abs(_end.Y - _start.Y) + 1) / 2, Color.magenta);
        
        relativeTime -= _startLength;

        if (relativeTime >= 0)
        {
            float length = Length;

            for (float i = relativeTime % BOX_SPACING; i < Mathf.Min(relativeTime, length); i += BOX_SPACING)
            {
                DrawBox(i);
            }
        }
    }

    public IEnumerable<Point> GetOccupiedPoints()
    {
        return _start.GetPointsTo(_end);
    }

    public Direction _linearDirection;
    public Direction _endCurveDirection;
    public Direction _startCurveDirection;

    public float StartLength
    {
        get
        {
            if (_linearDirection == _startCurveDirection)
                return 1.0f;
            else
                return Mathf.PI / 4.0f;
        }
    }

    public float EndLength
    {
        get
        {
            if (_linearDirection == _endCurveDirection)
                return 1.0f;
            else
                return Mathf.PI / 4.0f;
        }
    }
    
    public float Length
    {
        get
        {
            return (_end - _start).ManhattanLength - 1 + StartLength + EndLength;
        }
    }
}
