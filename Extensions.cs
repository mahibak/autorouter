using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Idlorio
{
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
                if (Math.Abs(enumeratorTwoInFront.Current.X - enumerator.Current.X) != 2 && Math.Abs(enumeratorTwoInFront.Current.Y - enumerator.Current.Y) != 2)
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
            int dx = Math.Sign(to.X - p.X);
            int dy = Math.Sign(to.Y - p.Y); 

            Point ret = p;

            yield return ret;

            while(ret != to)
            {
                ret.X += dx;
                ret.Y += dy;
                yield return ret;
            }
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

        public static void Foreach<T>(this IEnumerable<T> list, Action<T> action)
        {
            var e = list.GetEnumerator();

            while (e.MoveNext())
                action(e.Current);
        }
    }
}
