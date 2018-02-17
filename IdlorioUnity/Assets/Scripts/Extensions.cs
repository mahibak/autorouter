using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

static class Extensions
{
    public static IEnumerable<Point> GetCorners(this IEnumerable<Point> points)
    {
        var enumerator = points.GetEnumerator();

        if (!enumerator.MoveNext())
            yield break;

        yield return enumerator.Current;

        var enumeratorTwoInFront = points.GetEnumerator();

        if (!enumeratorTwoInFront.MoveNext())
            yield break;

        if (!enumeratorTwoInFront.MoveNext())
            yield break;

        Point middle = enumeratorTwoInFront.Current;

        if (!enumeratorTwoInFront.MoveNext())
        {
            yield return middle;
            yield break;
        }

        do
        {
            if (System.Math.Abs(enumeratorTwoInFront.Current.X - enumerator.Current.X) != 2 && System.Math.Abs(enumeratorTwoInFront.Current.Y - enumerator.Current.Y) != 2)
            {
                yield return middle;
            }

            middle = enumeratorTwoInFront.Current;

        } while (enumeratorTwoInFront.MoveNext() && enumerator.MoveNext());

        yield return middle;
    }

    public static IEnumerable<Point[]> ScanWindow(this IEnumerable<Point> points, int windowSize)
    {
        var enumerator = points.GetEnumerator();

        Point[] ret = new Point[windowSize];

        for (int i = 0; i < windowSize; i++)
        {
            if (!enumerator.MoveNext())
                yield break;
            ret[i] = enumerator.Current;
        }

        yield return ret;

        while (enumerator.MoveNext())
        {
            for (int i = 0; i < windowSize - 1; i++)
                ret[i] = ret[i + 1];
            ret[windowSize - 1] = enumerator.Current;
            yield return ret;
        }
    }

    public static IEnumerable<Point> PointsFromCorners(this IEnumerable<Point> points)
    {
        var enumerator = points.GetEnumerator();

        if (!enumerator.MoveNext())
            yield break;

        Point last = enumerator.Current;
        yield return last;

        while(enumerator.MoveNext())
        {
            var e2 = last.GetPointsTo(enumerator.Current).Skip(1).GetEnumerator();
            while(e2.MoveNext())
                yield return e2.Current;
            last = enumerator.Current;
        }
    }

    public static IEnumerable<Point> GetPointsTo(this Point p, Point to)
    {
        int dx = System.Math.Sign(to.X - p.X);
        int dy = System.Math.Sign(to.Y - p.Y);

        Point ret = p;

        yield return ret;

        while (ret != to)
        {
            ret.X += dx;
            ret.Y += dy;
            yield return ret;
        }
    }

    public static T Random<T>(this T[] array)
    {
        return array[0];
    }

    public static IEnumerable<Point> GetOrthogonalNeighbors(this Point p)
    {
        yield return new Point(p.X + 0, p.Y + 1);
        yield return new Point(p.X + 1, p.Y + 0);
        yield return new Point(p.X + 0, p.Y - 1);
        yield return new Point(p.X - 1, p.Y + 0);
    }

    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list)
    {
        return list.GetPermutations(list.Count());
    }

    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new T[] { t });

        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    public static List<T> Randomized<T>(this IEnumerable<T> list)
    {
        return list.OrderBy(order => UnityEngine.Random.Range(0.0f, 1.0f)).ToList();
    }

    public static void Foreach<T>(this IEnumerable<T> list, Action<T> action)
    {
        var e = list.GetEnumerator();

        while (e.MoveNext())
            action(e.Current);
    }

    static IEnumerable<Point> plotLineLow(int x0, int y0, int x1, int y1, bool continuous)
    {
        int dx = x1 - x0;
        int dy = y1 - y0;
        int yi = 1; 
        if (dy < 0)
        {
            yi = -1;
            dy = -dy;
        }
        float D = 2 * dy - dx;
        int y = y0;

        for (int x = x0; x <= x1; x++)
        {
            yield return new Point(x, y);
            if (D > 0)
            {
                y = y + yi;
                if (continuous && x != x1)
                    yield return new Point(x, y);

                D = D - 2 * dx;
            }
            D = D + 2 * dy;
        }
    }

    static IEnumerable<Point> plotLineHigh(int x0, int y0, int x1, int y1, bool continuous)
    {
        int dx = x1 - x0;
        int dy = y1 - y0;
        int xi = 1;
        if (dx < 0)
        {
            xi = -1;
            dx = -dx;
        }

        float D = 2 * dx - dy;
        int x = x0;

        for (int y = y0; y <= y1; y++)
        {
            yield return new Point(x, y);
            if (D > 0)
            {
                x = x + xi;
                if(continuous && y != y1)
                    yield return new Point(x, y);

                D = D - 2 * dy;
            }
            D = D + 2 * dx;
        }
    }

    public static IEnumerable<Point> LineTo(this Point from, Point to, bool continuous = true)
    {
        if (System.Math.Abs(to.Y - from.Y) < System.Math.Abs(to.X - from.X))
        {
            if (from.X > to.X)
                return plotLineLow(to.X, to.Y, from.X, from.Y, continuous);
            else
                return plotLineLow(from.X, from.Y, to.X, to.Y, continuous);
        }
        else
        {
            if (from.Y > to.Y)
                return plotLineHigh(to.X, to.Y, from.X, from.Y, continuous);
            else
                return plotLineHigh(from.X, from.Y, to.X, to.Y, continuous);
        }
    }
}
