using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio
{
    public class Point
    {
        public int X;
        public int Y;

        public Point()
        {
            X = 0;
            Y = 0;
        }

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
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
                    break;
            }
        }

        public static Point operator +(Point p, Direction d)
        {
            return p + new Idlorio.Point(d);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Idlorio.Point(p1.X + p2.X, p1.Y + p2.Y);
        }
    }
}
