﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Point
{
    public int X;
    public int Y;
        
    public Point(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, 0, Y);
    }

    public Point(Direction Direction)
    {
        switch (Direction)
        {
            case Direction.Up:
                X = 0;
                Y = 1;
                break;

            case Direction.Right:
                X = 1;
                Y = 0;
                break;

            case Direction.Down:
                X = 0;
                Y = -1;
                break;

            case Direction.Left:
                X = -1;
                Y = 0;
                break;

            default:
                X = 0;
                Y = 0;
                break;
        }
    }

    public static Point operator +(Point p, Direction d)
    {
        return p + new Point(d);
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }
    
    public static Point operator -(Point p, Direction d)
    {
        return p - new Point(d);
    }

    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1 == p2);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return '(' + X.ToString() + ',' + Y.ToString() + ')';
    }

    public IEnumerable<Point> GetPointsInRectangle(int sizeX, int sizeY)
    {
        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeX; y++)
                yield return new Point(X + x, Y + y);
    }

    public int ManhattanLength
    {
        get
        {
            return System.Math.Abs(X) + System.Math.Abs(Y);
        }
    }

    public Direction DirectionTo(Point p2)
    {
        if (this.X < p2.X)
            return Direction.Right;
        if (this.X > p2.X)
            return Direction.Left;
        if (this.Y < p2.Y)
            return Direction.Up;
        return Direction.Down;
    }
}