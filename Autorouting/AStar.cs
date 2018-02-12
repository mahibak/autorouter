using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idlorio.Autorouting
{
    class AStar
    {
        public class Point
        {
            public Point(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }

            public int X;
            public int Y;

            public float gScore = float.PositiveInfinity;
            public float fScore = float.PositiveInfinity;
            public Point cameFrom;

            public override string ToString()
            {
                return String.Format("({0}, {1}) g={2} f={3}", X, Y, gScore, fScore);
            }

            public IEnumerable<Point> GetReversedPath()
            {
                Point ret = cameFrom;

                while (ret != null)
                {
                    yield return ret;
                    ret = ret.cameFrom;
                }
            }

            public IEnumerable<Point> GetPath()
            {
                return GetReversedPath().Reverse();
            }
        }
        
        Point[,] points;

        int width;
        int height;
        Func<System.Drawing.Point, float> costEvaluator;

        float heuristicCostEstimate(Point current, Point goal, Point start)
        {
            int dx = Math.Abs(goal.X - current.X);
            int dy = Math.Abs(goal.Y - current.Y);
            float heuristic = (dx + dy);
            return heuristic;
        }

        public AStar(int width, int height, Func<System.Drawing.Point, float> costEvaluator)
        {
            this.width = width;
            this.height = height;
            this.costEvaluator = costEvaluator;

            points = new Point[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    points[x, y] = new Point(x, y);
        }

        IEnumerable<Point> GetNeighbors(Point p)
        {
            if (p.Y != height - 1)
                yield return points[p.X, p.Y + 1];
            if (p.X != width - 1)
                yield return points[p.X + 1, p.Y];
            if (p.Y != 0)
                yield return points[p.X, p.Y - 1];
            if (p.X != 0)
                yield return points[p.X - 1, p.Y];
        }

        public List<System.Drawing.Point> Find(int startX, int startY, int goalX, int goalY)
        {
            Point start = points[startX, startY];
            Point goal = points[goalX, goalY];

            List<Point> closedSet = new List<Point>();
            List<Point> openSet = new List<Point>();
            openSet.Add(start);

            start.gScore = 0;
            start.fScore = heuristicCostEstimate(start, goal, start);

            while(openSet.Count > 0)
            {
                Point current = openSet.OrderBy(x => x.fScore).First();

                if (current.X == goal.X && current.Y == goal.Y)
                    return reconstructPath(current);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (Point neighbor in GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    float tentativeGScore = current.gScore + costEvaluator(new System.Drawing.Point(neighbor.X, neighbor.Y));

                    if (float.IsInfinity(tentativeGScore))
                        continue;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    
                    if (tentativeGScore >= neighbor.gScore)
                        continue;
                    
                    neighbor.cameFrom = current;
                    neighbor.gScore = tentativeGScore;
                    
                    neighbor.fScore = neighbor.gScore + heuristicCostEstimate(neighbor, goal, start);
                }
            }

            return null;
        }

        private float distanceBetween(Point current, Point neighbor)
        {
            int distX = current.X - neighbor.X;
            int distY = current.Y - neighbor.Y;

            return (float)Math.Sqrt(distX * distX + distY * distY);
        }

        private List<System.Drawing.Point> reconstructPath(Point current)
        {
            List<System.Drawing.Point> totalPath = new List<System.Drawing.Point>();
            totalPath.Add(new System.Drawing.Point(current.X, current.Y));

            while(current.cameFrom != null)
            {
                totalPath.Insert(0, new System.Drawing.Point(current.cameFrom.X, current.cameFrom.Y));
                current = current.cameFrom;
            }

            return totalPath;
        }
    }
}
