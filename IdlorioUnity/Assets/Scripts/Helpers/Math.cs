using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
{
    public const float PI = 3.14159265359f;
    public const float PI_2 = 6.28318530718f;

    public static float Square(float num)
    {
        return num * num;
    }

    public static float DeltaAngleRad(float a1, float a2)
    {
        return Mathf.DeltaAngle(a1 * Mathf.Rad2Deg, a2 * Mathf.Rad2Deg) * Mathf.Deg2Rad;
    }

    public static Vector2 ToHorizVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    public static Vector3 ToVector3(this Vector2 v, float y = 0.0f)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static float GetHorizontalSquareLength(this Vector3 v)
    {
        return v.x * v.x + v.z * v.z;
    }

    public static float GetSquareLength(this Vector2 v)
    {
        return v.x * v.x + v.y * v.y;
    }

    public static float GetSquareLength(this Vector3 v)
    {
        return v.x * v.x + v.y * v.y + v.z * v.z;
    }

    public static Vector3 GetDirFromYaw(float yaw)
    {
        return new Vector3(Mathf.Sin(yaw), 0.0f, Mathf.Cos(yaw));
    }

    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime)
    {
        if (Time.deltaTime < Mathf.Epsilon)
        {
            return current;
        }

        return Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime);
    }

    public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
    {
        if (Time.deltaTime < Mathf.Epsilon)
        {
            return current;
        }

        return Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime);
    }

    public static bool IntersectCircleArc(Vector2 circleCenter, float circleRadius, Vector2 arcCenter, float arcDirAngle, float arcRadius, float arcWidthAngleDeg)
    {
        float halfArcWidthRad = arcWidthAngleDeg * Mathf.Deg2Rad * 0.5f;
        Vector2 leftCorner = arcCenter + new Vector2(Mathf.Cos(arcDirAngle + halfArcWidthRad) * arcRadius, Mathf.Sin(arcDirAngle + halfArcWidthRad) * arcRadius);
        Vector2 rightCorner = arcCenter + new Vector2(Mathf.Cos(arcDirAngle - halfArcWidthRad) * arcRadius, Mathf.Sin(arcDirAngle - halfArcWidthRad) * arcRadius);
        return IntersectCircleArc(circleCenter, circleRadius, arcCenter, arcDirAngle, arcRadius, arcWidthAngleDeg, leftCorner, rightCorner);
    }

    public static bool IntersectCircleArc(Vector2 circleCenter, float circleRadius, Vector2 arcCenter, float arcDirAngle, float arcRadius, float arcWidthAngleDeg, Vector2 leftCorner, Vector2 rightCorner)
    {
        // Check if the center of the circle is inside the arc or along the far line.
        Vector2 arcToCircle = circleCenter - arcCenter;
        if (arcToCircle.GetSquareLength() < Math.Square(arcRadius + circleRadius))
        {
            float arcToCircleAngle = Mathf.Atan2(arcToCircle.y, arcToCircle.x);
            if (Mathf.Abs(Mathf.DeltaAngle(arcDirAngle * Mathf.Rad2Deg, arcToCircleAngle * Mathf.Rad2Deg)) <= arcWidthAngleDeg * 0.5f)
            {
                return true;
            }
        }

        // Check if we're at a corner
        float circleRadiusSq = circleRadius * circleRadius;
        if ((leftCorner - circleCenter).GetSquareLength() < circleRadiusSq
            || (rightCorner - circleCenter).GetSquareLength() < circleRadiusSq)
        {
            return true;
        }

        // Check if we are along on of the side lines.
        if (IntersectCircleLineSegment(arcCenter, leftCorner, circleCenter, circleRadius)
            || IntersectCircleLineSegment(arcCenter, rightCorner, circleCenter, circleRadius))
        {
            return true;
        }

        return false;
    }

    public static bool IntersectCircleLineSegment(Vector2 p1, Vector2 p2, Vector2 circleCenter, float circleRadius)
    {
        float c1x = circleCenter.x - p1.x;
        float c1y = circleCenter.y - p1.y;
        float e1x = p2.x - p1.x;
        float e1y = p2.y - p1.y;
        float k = c1x * e1x + c1y * e1y;

        if (k > 0)
        {
            float sqLen = e1x * e1x + e1y * e1y;
            k = k * k / sqLen;
        
            if (k < sqLen)
            {
                return c1x * c1x + c1y * c1y - k <= circleRadius * circleRadius;
            }
        }

        return false;
    }
}

public class FloatSmoother
{
    private float _current = 0f;
    private float _target = 0f;
    private float _velocity = 0f;
    private float _smoothTime = 1f;

    public FloatSmoother() {}
    public FloatSmoother(float smoothTime) { SetSmoothTime(smoothTime); }

    public void Init(float initialValue) { Init(initialValue, initialValue); }
    public void Init(float initialValue, float target) { _current = initialValue; _target = target; _velocity = 0; }
    public void SetSmoothTime(float smoothTime) { _smoothTime = smoothTime; }
    public void SetTarget(float target) { _target = target; }
    public void Update()
    {
        if (Time.deltaTime > Mathf.Epsilon)
        {
            _current = Mathf.SmoothDampAngle(_current, _target, ref _velocity, _smoothTime);
        }
    }
    public float GetValue() { return _current; }
}

public class AngleSmoother
{
    private float _current = 0f;
    private float _target = 0f;
    private float _velocity = 0f;
    private float _smoothTime = 1f;

    public AngleSmoother() { }
    public AngleSmoother(float smoothTime) { SetSmoothTime(smoothTime); }

    public void Init(float initialValue) { Init(initialValue, initialValue); }
    public void Init(float initialValue, float target) { _current = initialValue; _target = target; _velocity = 0; }
    public void SetSmoothTime(float smoothTime) { _smoothTime = smoothTime; }
    public void SetTarget(float target) { _target = target; }
    public void Update()
    {
        if (Time.deltaTime > Mathf.Epsilon)
        {
            _current = Mathf.SmoothDamp(_current, _target, ref _velocity, _smoothTime);
        }
    }
    public float GetValue() { return _current; }
}

public class VectorSmoother
{
    private Vector3 _current = Vector3.zero;
    private Vector3 _target = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private float _smoothTime = 1f;

    public VectorSmoother() { }
    public VectorSmoother(float smoothTime) { SetSmoothTime(smoothTime); }

    public void Init(Vector3 initialValue) { Init(initialValue, initialValue); }
    public void Init(Vector3 initialValue, Vector3 target) { _current = initialValue; _target = target; _velocity = Vector3.zero; }
    public void SetSmoothTime(float smoothTime) { _smoothTime = smoothTime; }
    public void SetTarget(Vector3 target) { _target = target; }
    public void Update()
    {
        if (Time.deltaTime > Mathf.Epsilon)
        {
            _current.x = Mathf.SmoothDamp(_current.x, _target.x, ref _velocity.x, _smoothTime);
            _current.y = Mathf.SmoothDamp(_current.y, _target.y, ref _velocity.y, _smoothTime);
            _current.z = Mathf.SmoothDamp(_current.z, _target.z, ref _velocity.z, _smoothTime);
        }
    }
    public Vector3 GetValue() { return _current; }
}