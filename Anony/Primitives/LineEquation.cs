using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Anony.Primitives
{
    public class LineEquation
    {
        public LineEquation(Point start, Point end)
        {
            Start = start;
            End = end;

            IsVertical = Math.Abs(End.X - start.X) < 0.00001f;
            M = (End.Y - Start.Y) / (End.X - Start.X);
            A = -M;
            B = 1;
            C = Start.Y - M * Start.X;
        }

        public bool IsVertical { get; private set; }

        public double M { get; private set; }

        public Point Start { get; private set; }
        public Point End { get; private set; }

        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        public bool IntersectsWithLine(LineEquation otherLine, out Point intersectionPoint)
        {
            intersectionPoint = new Point(0, 0);
            if (IsVertical && otherLine.IsVertical)
                return false;
            if (IsVertical || otherLine.IsVertical)
            {
                intersectionPoint = GetIntersectionPointIfOneIsVertical(otherLine, this);
                return true;
            }
            double delta = A * otherLine.B - otherLine.A * B;
            bool hasIntersection = Math.Abs(delta - 0) > 0.0001f;
            if (hasIntersection)
            {
                double x = (otherLine.B * C - B * otherLine.C) / delta;
                double y = (A * otherLine.C - otherLine.A * C) / delta;
                intersectionPoint = new Point(x, y);
            }
            return hasIntersection;
        }

        private static Point GetIntersectionPointIfOneIsVertical(LineEquation line1, LineEquation line2)
        {
            LineEquation verticalLine = line2.IsVertical ? line2 : line1;
            LineEquation nonVerticalLine = line2.IsVertical ? line1 : line2;

            double y = (verticalLine.Start.X - nonVerticalLine.Start.X) *
                       (nonVerticalLine.End.Y - nonVerticalLine.Start.Y) /
                       ((nonVerticalLine.End.X - nonVerticalLine.Start.X)) +
                       nonVerticalLine.Start.Y;
            double x = line1.IsVertical ? line1.Start.X : line2.Start.X;
            return new Point(x, y);
        }

        public bool IntersectWithSegementOfLine(LineEquation otherLine, out Point intersectionPoint)
        {
            bool hasIntersection = IntersectsWithLine(otherLine, out intersectionPoint);
            if (hasIntersection)
                return intersectionPoint.IsBetweenTwoPoints(otherLine.Start, otherLine.End);
            return false;
        }

        public bool GetIntersectionLineForRay(Rect rectangle, out LineEquation intersectionLine)
        {
            if (Start == End)
            {
                intersectionLine = null;
                return false;
            }
            IEnumerable<LineEquation> lines = rectangle.GetLinesForRectangle();
            intersectionLine = new LineEquation(new Point(0, 0), new Point(0, 0));
            var intersections = new Dictionary<LineEquation, Point>();
            foreach (LineEquation equation in lines)
            {
                Point point;
                if (IntersectWithSegementOfLine(equation, out point))
                    intersections[equation] = point;
            }
            if (!intersections.Any())
                return false;

            var intersectionPoints = new SortedDictionary<double, Point>();
            foreach (var intersection in intersections)
            {
                if (End.IsBetweenTwoPoints(Start, intersection.Value) ||
                    intersection.Value.IsBetweenTwoPoints(Start, End))
                {
                    double distanceToPoint = Start.DistanceToPoint(intersection.Value);
                    intersectionPoints[distanceToPoint] = intersection.Value;
                }
            }
            if (intersectionPoints.Count == 1)
            {
                Point endPoint = intersectionPoints.First().Value;
                intersectionLine = new LineEquation(Start, endPoint);
                return true;
            }

            if (intersectionPoints.Count == 2)
            {
                Point start = intersectionPoints.First().Value;
                Point end = intersectionPoints.Last().Value;
                intersectionLine = new LineEquation(start, end);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return "[" + Start + "], [" + End + "]";
        }
    }
    internal static class RectangeExtentions
    {
        public static IEnumerable<LineEquation> GetLinesForRectangle(this Rect rectangle)
        {
            var lines = new List<LineEquation>{
                new LineEquation(new Point(rectangle.X, rectangle.Y),
                                 new Point(rectangle.X, rectangle.Y + rectangle.Height)),
                new LineEquation(new Point(rectangle.X, rectangle.Y + rectangle.Height),
                                 new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height)),
                new LineEquation(new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
                                 new Point(rectangle.X + rectangle.Width, rectangle.Y)),
                new LineEquation(new Point(rectangle.X + rectangle.Width, rectangle.Y),
                                 new Point(rectangle.X, rectangle.Y)),
            };
            return lines;
        }
    }
    public static class PointExtensions
    {
        public static float DistanceToPoint(this Point point, Point point2)
        {
            return
                (float)
                Math.Sqrt((point2.X - point.X) * (point2.X - point.X) + (point2.Y - point.Y) * (point2.Y - point.Y));
        }

        public static bool IsBetweenTwoPoints(this Point targetPoint, Point point1, Point point2)
        {
            double minX = Math.Min(point1.X, point2.X);
            double minY = Math.Min(point1.Y, point2.Y);
            double maxX = Math.Max(point1.X, point2.X);
            double maxY = Math.Max(point1.Y, point2.Y);

            double targetX = targetPoint.X;
            double targetY = targetPoint.Y;

            return LessOrEqual(minX, targetX)
                   && LessOrEqual(targetX, maxX)
                   && LessOrEqual(minY, targetY)
                   && LessOrEqual(targetY, maxY);
        }

        private static bool LessOrEqual(double left, double right)
        {
            return left <= right || (1 <= left / right && left / right < 1.00000000001);
        }
    }
}
