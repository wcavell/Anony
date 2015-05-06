using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Anony.Primitives
{
    #region Enumerations

    internal enum InkDrawingMode
    {
        Draw,
        Erase,
        Hand
    }

    #endregion Enumerations

    class InkTools
    {
        public InkDrawingMode Mode { get; set; }
        public double StrokeThickness { get; set; }
        
    }
    internal class InkDrawing
    {
        public double StrokeThickness { get; set; }
        public SolidColorBrush Brush { get; set; } 
        private Canvas canvas;
        uint PenID, TouchID;
        private double X1, X2, Y1, Y2;
        Point StartPoint, PreviousContactPoint, CurrentContactPoint;
        double IntX = 0;
        double IntY = 0;
        int totalDrawing = 0;
        private bool IsDraw = false;
        public InkDrawingMode Mode { get; set; }
        public InkDrawing()
        {
            StrokeThickness = 1;
            Brush = new SolidColorBrush(Colors.Black);
        }

        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            canvas.PointerPressed += OnPointerPressed;
            canvas.PointerCaptureLost += OnPointerCaptureLost;
            canvas.PointerExited += OnPointerExited;
            canvas.PointerMoved += OnPointerMoved;
            canvas.PointerReleased += OnPointerReleased;
        }
        private void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            TouchID = 0;
            PenID = 0;
            e.Handled = true;
            IsDraw = true;
            totalDrawing = 0;
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!IsDraw) return;
            if (e.Pointer.PointerId == PenID || e.Pointer.PointerId == TouchID)
            {
                CurrentContactPoint = e.GetCurrentPoint(canvas).Position;
                X1 = PreviousContactPoint.X;
                Y1 = PreviousContactPoint.Y;
                X2 = CurrentContactPoint.X;
                Y2 = CurrentContactPoint.Y;
                if (Distance(X1, Y1, X2, Y2) > 2.0)
                {
                    Line line  = new Line()
                    {
                        X1 = X1,
                        Y1 = Y1,
                        X2 = X2,
                        Y2 = Y2,
                        StrokeThickness = StrokeThickness,
                        Stroke = Brush,
                        Fill = Brush
                    };
                    if (Mode == InkDrawingMode.Erase)
                    {
                        ChackeLine(line);
                        line = null;
                    }
                    PreviousContactPoint = CurrentContactPoint;
                    if (line != null)
                    {
                        this.canvas.Children.Add(line);
                    }
                }
                PreviousContactPoint = CurrentContactPoint;
            }
        }

        private void OnPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            IsDraw = false;
        }

        private void OnPointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            IsDraw = false;
            totalDrawing = 0;
        }

        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PreviousContactPoint = e.GetCurrentPoint(canvas).Position;
            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed)
            {
                PenID = e.GetCurrentPoint(canvas).PointerId;
                e.Handled = true;
                IsDraw = true;
            }
        }

        public void ChackeLine(Line line)
        {
            Line xline = null;
            bool flag = false;
            Segment seg1 = new Segment();
            Segment seg2 = new Segment();

            seg1.Start = new Point(line.X1, line.Y1);
            seg1.End = new Point(line.X2, line.Y2);
            foreach (var item in this.canvas.Children)
            {
                xline = (Line)item;
                if (xline.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    if (((string)xline.Tag) == null)
                    {

                        seg2.Start = new Point(xline.X1, xline.Y1);
                        seg2.End = new Point(xline.X2, xline.Y2);
                        var xx = Intersects(seg1, seg2);
                        var yy = lineSegmentIntersection(seg1.Start.X, seg1.Start.Y, seg1.End.X, seg1.End.Y, seg2.Start.X, seg2.Start.Y, seg2.End.X, seg2.End.Y, ref IntX, ref IntY);
                        if (yy)
                        {
                            xline.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            //break;

                            Debug.WriteLine("   {0} - {1}", seg1.Start, seg1.End);
                            Debug.WriteLine("-->{0} - {1}", seg2.Start, seg2.End);
                        }
                    }
                }
            }
        }
        private double Distance(double x1, double y1, double x2, double y2)
        {
            double d = 0;
            d = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            return d;
        }
        public struct Segment
        {
            public Point Start;
            public Point End;
        }

        public Point? Intersects(Segment AB, Segment CD)
        {
            double deltaACy = AB.Start.Y - CD.Start.Y;
            double deltaDCx = CD.End.X - CD.Start.X;
            double deltaACx = AB.Start.X - CD.Start.X;
            double deltaDCy = CD.End.Y - CD.Start.Y;
            double deltaBAx = AB.End.X - AB.Start.X;
            double deltaBAy = AB.End.Y - AB.Start.Y;

            double denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
            double numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

            if (denominator == 0)
            {
                if (numerator == 0)
                {
                    // collinear. Potentially infinite intersection points.
                    // Check and return one of them.
                    if (AB.Start.X >= CD.Start.X && AB.Start.X <= CD.End.X)
                    {
                        return AB.Start;
                    }
                    else if (CD.Start.X >= AB.Start.X && CD.Start.X <= AB.End.X)
                    {
                        return CD.Start;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                { // parallel
                    return null;
                }
            }

            double r = numerator / denominator;
            if (r < 0 || r > 1)
            {
                return null;
            }

            double s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
            if (s < 0 || s > 1)
            {
                return null;
            }

            return new Point((float)(AB.Start.X + r * deltaBAx), (float)(AB.Start.Y + r * deltaBAy));
        }









        /// <summary>
        /// 判断直线2的两点是否在直线1的两边。
        /// </summary>
        /// <param name="line1">直线1</param>
        /// <param name="line2">直线2</param>
        /// <returns></returns>
        private bool CheckCrose(Line line1, Line line2)
        {
            Point v1 = new Point();
            Point v2 = new Point();
            Point v3 = new Point();

            v1.X = line2.X1 - line1.X2;
            v1.Y = line2.Y1 - line1.Y2;

            v2.X = line2.X2 - line1.X2;
            v2.Y = line2.Y2 - line1.Y2;

            v3.X = line1.X1 - line1.X2;
            v3.Y = line1.Y1 - line1.Y2;

            return (CrossMul(v1, v3) * CrossMul(v2, v3) <= 0);

        }
        /// <summary>
        /// 判断两条线段是否相交。
        /// </summary>
        /// <param name="line1">线段1</param>
        /// <param name="line2">线段2</param>
        /// <returns>相交返回真，否则返回假。</returns>
        private bool CheckTwoLineCrose(Line line1, Line line2)
        {
            return CheckCrose(line1, line2) && CheckCrose(line2, line1);
        }
        /// <summary>
        /// 计算两个向量的叉乘。
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        private double CrossMul(Point pt1, Point pt2)
        {
            return pt1.X * pt2.Y - pt1.Y * pt2.X;
        }

        bool IsIntersecting(Point a, Point b, Point c, Point d)
        {
            double denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            double numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            double numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            // Detect coincident lines (has a problem, read below)
            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            double r = numerator1 / denominator;
            double s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }











        public bool lineSegmentIntersection(double Ax, double Ay, double Bx, double By, double Cx, double Cy, double Dx, double Dy, ref double X, ref double Y)
        {

            double distAB, theCos, theSin, newX, ABpos;

            //  Fail if either line segment is zero-length.
            if (Ax == Bx && Ay == By || Cx == Dx && Cy == Dy)
            {
                if ((Cx == Dx) && (Cx >= Ax && Dx <= Bx) && (Cy == Dy && Ay == Cy))
                {
                    X = Cx;
                    Y = Cy;
                    return true;
                }
                if ((Ax == Bx) && (Ax >= Cx && Bx <= Dx) && (Ay == By && Cy == Ay))
                {
                    X = Ax;
                    Y = Ay;
                    return true;
                }
                return false;
            }

            //------custom--|-|-----------------------------//end of one line on the other line

            bool IsVertical = false;

            if (IsPointOnLineSegment(Ax, Ay, Cx, Cy, Dx, Dy, ref IsVertical))
            {
                if (IsVertical)
                {
                    if (Ax == Bx)
                    {
                        X = Cx;//D
                        Y = Cy;
                    }
                    else
                    {
                        X = Ax;
                        Y = Ay;
                    }

                }
                else
                {
                    X = Ax; Y = Ay;
                }
                return true;
            }
            if (IsPointOnLineSegment(Bx, By, Cx, Cy, Dx, Dy, ref IsVertical))
            {
                if (IsVertical)
                {
                    if (Ax == Bx)
                    {
                        X = Dx; //C
                        Y = Dy;
                    }
                    else
                    {
                        X = Bx;
                        Y = By;
                    }
                }
                else
                {
                    X = Bx; Y = By;
                }
                return true;
            }
            if (IsPointOnLineSegment(Cx, Cy, Ax, Ay, Bx, By, ref IsVertical))
            {
                if (IsVertical)
                {
                    X = Cx;
                    Y = Cy;
                }
                else
                {
                    X = Cx; Y = Cy;
                }
                return true;
            }
            if (IsPointOnLineSegment(Dx, Dy, Ax, Ay, Bx, By, ref IsVertical))
            {
                if (IsVertical)
                {
                    X = Dx;
                    Y = Dy;
                }
                else
                {
                    X = Dx; Y = Dy;
                }
                return true;
            }


            //------------------------------------------------
            //  Fail if the segments share an end-point.
            if (Ax == Cx && Ay == Cy || Bx == Cx && By == Cy
            || Ax == Dx && Ay == Dy || Bx == Dx && By == Dy)
            {
                return false;
            }

            //  (1) Translate the system so that point A is on the origin.
            Bx -= Ax; By -= Ay;
            Cx -= Ax; Cy -= Ay;
            Dx -= Ax; Dy -= Ay;

            //  Discover the length of segment A-B.
            distAB = Math.Sqrt(Bx * Bx + By * By);

            //  (2) Rotate the system so that point B is on the positive X axis.
            theCos = Bx / distAB;
            theSin = By / distAB;
            newX = Cx * theCos + Cy * theSin;
            Cy = Cy * theCos - Cx * theSin; Cx = newX;
            newX = Dx * theCos + Dy * theSin;
            Dy = Dy * theCos - Dx * theSin; Dx = newX;

            //  Fail if segment C-D doesn't cross line A-B.
            if (Cy < 0 && Dy < 0 || Cy >= 0 && Dy >= 0) return false;

            //  (3) Discover the position of the intersection point along line A-B.
            ABpos = Dx + (Cx - Dx) * Dy / (Dy - Cy);

            //  Fail if segment C-D crosses line A-B outside of segment A-B.
            if (ABpos < 0 || ABpos > distAB) return false;

            //  (4) Apply the discovered position to line A-B in the original coordinate system.
            X = Math.Round((Ax + ABpos * theCos), 3);
            Y = Math.Round((Ay + ABpos * theSin), 3);

            //  Success.
            return true;
        }


        public static bool IsPointOnLineSegment(double Px, double Py, double Ax, double Ay, double Bx, double By, ref bool IsVertical)
        {
            double least = 0;
            if (Px < least) least = Px;
            if (Ax < least) least = Ax;
            if (Bx < least) least = Bx;

            if (least < 0)
            {
                Px += Math.Abs(least);
                Ax += Math.Abs(least);
                Bx += Math.Abs(least);
            }

            if (!(Ax <= Px && Bx >= Px)) return false;
            if (Bx == Ax) //vertical line, slope = infinity
            {
                IsVertical = true;
                if ((Px == Ax && (Py >= Ay && Py <= By)) || (Px == Ax && (Py >= By && Py <= Ay)))
                {
                    return true;
                }

                return false;

            }
            double S = (By - Ay) / (Bx - Ax);//S=0 horizontal line
            double Y = Ay - (S * Ax);

            if (Math.Abs(Py - (S * Px + Y)) < 0.0009) return true;   //change the precision you want

            return false;
        }



         
    }
}
