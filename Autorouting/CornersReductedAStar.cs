using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Idlorio.Autorouting
{
    class CornersReductedAStar
    {
        public static List<Point> Find(int width, int height, Func<System.Drawing.Point, float> costEvaluator, int startX, int startY, int goalX, int goalY)
        {
            var aStarResult = new AStar(width, height, costEvaluator).Find(startX, startY, goalX, goalY);

            if (aStarResult == null)
                return null;

            List<Point> corners = aStarResult.GetCorners().ToList();

            for (int i = 0; i < corners.Count - 3; i++)
            {
                Point newPoint = new Point(corners[i + 0].X, corners[i + 3].Y);
                Point newPoint2 = new Point(corners[i + 3].X, corners[i + 0].Y);
                
                if (corners[i + 0].GetPointsTo(newPoint).All(x => costEvaluator(x) != float.PositiveInfinity) && newPoint.GetPointsTo(corners[i + 3]).All(x => costEvaluator(x) != float.PositiveInfinity))
                {
                    corners.RemoveAt(i + 1);
                    corners.RemoveAt(i + 1);
                    corners.Insert(i + 1, newPoint);
                    i--;
                }
                else if (corners[i + 0].GetPointsTo(newPoint2).All(x => costEvaluator(x) != float.PositiveInfinity) && newPoint2.GetPointsTo(corners[i + 3]).All(x => costEvaluator(x) != float.PositiveInfinity))
                {
                    corners.RemoveAt(i + 1);
                    corners.RemoveAt(i + 1);
                    corners.Insert(i + 1, newPoint2);
                    i--;
                }
            }

            return corners.PointsFromCorners().ToList();
        }
    }
}
