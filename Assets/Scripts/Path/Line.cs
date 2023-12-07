using UnityEngine;
using RL.Math;

namespace RL.Paths
{
    public class Line<TPoint> : ILine<TPoint>
        where TPoint : IPathPoint
    {
        public TPoint Start { get; }
        public TPoint End { get; }

        public Vector2[] ControlPoints { get; }

        public int Vertices = 10;
        public PathPoint[] Parts { get; }

        public float Duration { get; }

        public Line(TPoint start, TPoint end, int vertices = 0, params Vector2[] controlPoints)
        {
            Start = start;
            End = end;
            ControlPoints = controlPoints;
            Vertices = vertices;

            Parts = new PathPoint[vertices];
            Parts[0] = new(0, 0, 0);
            float step = 1f / (vertices - 1);
            for (int i = 1; i < vertices; i++)
            {
                float t = i * step;
                Vector2 position = controlPoints.Length switch
                {
                    0 => Vector2.Lerp(Start.Position, End.Position, t),
                    1 => Maths.GetCurveBy3Point(Start.Position, controlPoints[0], End.Position, t),
                    2 => Maths.GetCurveBy4Point(Start.Position, controlPoints[0], controlPoints[1], End.Position, t),
                    _ => GetCurve(t)
                };
                float time = Vector2.Distance(Parts[i - 1], position);

                Parts[i] = new(time, position);
            }
        }

        Vector2[] _points = null;
        Vector2 GetCurve(float t)
        {
            if(_points == null)
            {
                _points = new Vector2[ControlPoints.Length];
                _points[0] = Start.Position;
                _points[^1] = End.Position;

                for (int i = 0; i < ControlPoints.Length; i++)
                    _points[i + 1] = ControlPoints[i];
            }

            return Maths.GetCurve(t, _points);
        }
    }
}