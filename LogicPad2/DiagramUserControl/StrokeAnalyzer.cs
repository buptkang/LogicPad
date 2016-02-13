using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Ink;

namespace LogicPad2.Diagram
{
    public class StrokeAnalyzer
    {
        public static double SpeedMin = 1.0f;
        public static double DistanceMin = 70.0f;
        public static double DistanceMax = 200.0f;
        public static double PressureRatioThreshold = 1.0f;
        public static double TimeThreshold = 200.0f;

        private static StrokeAnalyzer _instance;

        private StrokeAnalyzer()
        {
        }

        public static StrokeAnalyzer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StrokeAnalyzer();
                }
                return _instance;
            }
        }

        public bool IsFlick(StrokeInfo strokeInfo)
        {
            //Feature 1: speed > SpeedRatio
            //Feature 2: first 5 points pressure average > last 5 points pressure average
            //Feature 3: distance between first point and last point < distanceRatio
            //Feature 4: TimeSpan total milliseconds < 400
            if (strokeInfo.Span.TotalMilliseconds < TimeThreshold)
            {
                //if (strokeInfo.Speed > SpeedMin)
                //{
                if (strokeInfo.StrokeLength < DistanceMax && DistanceMin < strokeInfo.StrokeLength)
                {
                    if (strokeInfo.PressureRatio > PressureRatioThreshold)
                    {
                        strokeInfo.IsFlickGesture = true;
                        return true;
                    }
                }
                //}
            }

            strokeInfo.IsFlickGesture = false;
            return false;
        }

        public FlickDirections DetectFlickDirection(StrokeInfo strokeInfo)
        {
            //Find the first points as A(X1, Y1) and last points as B(X2, Y2) in the current stroke
            StylusPoint pointA = strokeInfo.CurrentStroke.StylusPoints[0];
            StylusPoint pointB = strokeInfo.CurrentStroke.StylusPoints[strokeInfo.CurrentStroke.StylusPoints.Count - 1];

            double pointAx = pointA.X;
            double pointAy = pointA.Y;
            double pointBx = pointB.X;
            double pointBy = pointB.Y;

            double deltaX = Math.Abs(pointAx - pointBx);
            double deltaY = Math.Abs(pointAy - pointBy);

            double radians = Math.Atan2(deltaY, deltaX);
            double angle = radians * (180 / Math.PI);

            if (pointBy < pointAy)
            {
                //up direction
                if (pointBx < pointAx && angle < 60)
                {
                    strokeInfo.StrokeFlickDirection = FlickDirections.UpLeft;
                    return FlickDirections.UpLeft;
                }
                else if (pointBx > pointAx && angle < 60)
                {
                    strokeInfo.StrokeFlickDirection = FlickDirections.UpRight;
                    return FlickDirections.UpRight;
                }
                else
                {
                    strokeInfo.StrokeFlickDirection = FlickDirections.Up;
                    return FlickDirections.Up;
                }
            }
            else
            {
                //down direction
                if (pointBx < pointAx && angle < 60)
                {
                    strokeInfo.StrokeFlickDirection = FlickDirections.DownLeft;
                    return FlickDirections.DownLeft;
                }
                else if (pointBx > pointAx && angle < 60)
                {
                    strokeInfo.StrokeFlickDirection = FlickDirections.DownRight;
                    return FlickDirections.DownRight;
                }
                else
                {
                    strokeInfo.StrokeFlickDirection = FlickDirections.Down;
                    return FlickDirections.Down;
                }
            }
        }

        public static bool IsTriggerPieMenuStroke(TimeSpan elapsedTime, double distance)
        {
            //1. Distance between Start point and Current point
            //2. Time elapsed since stylus down
            if (elapsedTime.TotalMilliseconds > 500 && distance < 6.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    /**
     * Stroke Data Representation
     */
    public class StrokeInfo
    {
        public Stroke CurrentStroke { get; set; }

        public TimeSpan Span { get; set; }

        public double StrokeLength { get; set; }

        public double Speed { get; set; }

        public float FirstThreePointsAveragePressure { set; get; }
        public float LastThreePointsAveragePressure { set; get; }

        public float PressureRatio { set; get; }

        public int StrokePointsNmbr { set; get; }

        //Flick Gesture
        public bool IsFlickGesture { set; get; }

        public FlickDirections StrokeFlickDirection { set; get; }

        public StylusPoint StartPoint { set; get; }
        public StylusPoint EndPoint { set; get; }
        public StylusPoint MiddlePoint { set; get; }

        public double BoundPercentage { set; get; }

        public StrokeInfo(Stroke stroke, TimeSpan span)
        {
            CurrentStroke = stroke;
            StrokePointsNmbr = stroke.StylusPoints.Count;
            Span = span;
            StrokeLength = GetStrokeLength(stroke);
            Speed = StrokeLength / Span.TotalMilliseconds;

            GetAveragePressure(stroke);

            StartPoint = CurrentStroke.StylusPoints[0];
            MiddlePoint = CurrentStroke.StylusPoints[CurrentStroke.StylusPoints.Count / 2];
            EndPoint = CurrentStroke.StylusPoints[CurrentStroke.StylusPoints.Count - 1];

            BoundPercentage = CalculateBoundPercentage();
        }

        private double CalculateBoundPercentage()
        {
            StylusPoint sp;
            double firstHalfBoundSize;
            double lastHalfBoundSize;

            double xMin = Double.PositiveInfinity, xMax = Double.NegativeInfinity;
            double yMin = Double.PositiveInfinity, yMax = Double.NegativeInfinity;
            for (int i = 0; i < CurrentStroke.StylusPoints.Count / 3; i++)
            {
                sp = CurrentStroke.StylusPoints[i];
                if (xMin > sp.X)
                    xMin = sp.X;
                if (xMax < sp.X)
                    xMax = sp.X;
                if (yMin > sp.Y)
                    yMin = sp.Y;
                if (yMax < sp.Y)
                    yMax = sp.Y;
            }
            firstHalfBoundSize = (xMax - xMin) * (yMax - yMin);

            xMin = Double.PositiveInfinity;
            xMax = Double.NegativeInfinity;
            yMin = Double.PositiveInfinity;
            yMax = Double.NegativeInfinity;
            for (int i = CurrentStroke.StylusPoints.Count * 2 / 3; i < CurrentStroke.StylusPoints.Count; i++)
            {
                sp = CurrentStroke.StylusPoints[i];
                if (xMin > sp.X)
                    xMin = sp.X;
                if (xMax < sp.X)
                    xMax = sp.X;
                if (yMin > sp.Y)
                    yMin = sp.Y;
                if (yMax < sp.Y)
                    yMax = sp.Y;
            }
            lastHalfBoundSize = (xMax - xMin) * (yMax - yMin);

            return lastHalfBoundSize / firstHalfBoundSize;
        }

        private void GetAveragePressure(Stroke stroke)
        {
            float firstThreePressureSum = 0.0f;
            float lastThreePressureSum = 0.0f;
            for (int i = 0; i < stroke.StylusPoints.Count; i++)
            {
                if (i < 3)
                    firstThreePressureSum += stroke.StylusPoints[i].PressureFactor;
                if (i > stroke.StylusPoints.Count - 4)
                    lastThreePressureSum += stroke.StylusPoints[i].PressureFactor;
            }
            FirstThreePointsAveragePressure = firstThreePressureSum / 3;
            LastThreePointsAveragePressure = lastThreePressureSum / 3;
            PressureRatio = FirstThreePointsAveragePressure / LastThreePointsAveragePressure;
        }

        private double GetStrokeLength(Stroke stroke)
        {
            double length = 0;
            for (int i = 1; i < stroke.StylusPoints.Count; i++)
            {
                length += pointDistance(stroke.StylusPoints[i], stroke.StylusPoints[i - 1]);
            }
            return length;
        }

        public static double pointDistance(StylusPoint a, StylusPoint b)
        {
            return pointDistance(a.ToPoint(), b.ToPoint());
        }//pointDistance

        public static double pointDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2.0) + Math.Pow(a.Y - b.Y, 2.0));
        }//pointDistance
    }
}
