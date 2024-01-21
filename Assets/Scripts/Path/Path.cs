using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using RL.Math;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace RL.Paths
{
    [Serializable]
    public class Path : IPath<Path, SyncPoint>
    {
        private readonly List<SyncPoint> _points = new();

        public SyncPoint this[int index] => _points[index];

        public int Count => _points.Count;
        public float Duration => _points[^1].Time;

        public float Width => .25f;

        public SyncPoint Add(IPathPoint point)
            => Add(point.Time, point.Position, point.ControlPoint);

        public SyncPoint Add(float time, Vector2 position, Vector2? controlPoint = null)
        {
            int index = GetIndexByTime(time);
            SyncPoint syncPoint = new(this, index)
            {
                Time = time,
                Position = position,
                ControlPoint = controlPoint ?? new(0, 0)
            };
            _points.Insert(index, syncPoint);
            return syncPoint;
        }

        public IEnumerator<SyncPoint> GetEnumerator() => _points.GetEnumerator();

        public int GetIndexByTime(float time)
        {
            for (int i = 0; i < Count - 1; i++)
                if (_points[i].Time <= time && _points[i + 1].Time > time)
                    return i;

            return Count;
        }

        public Vector2[] GetPointsPositions()
            => _points.Select((p) => p.Position).ToArray();

        public Vector2? GetPosition(float time)
        {
            SyncPoint current = _points[0];
            int count = Count;

            for (int i = 1; i < count; i++)
            {
                SyncPoint next = _points[i];

                if (current.Time <= time && next.Time > time)
                {
                    TimePoint[] parts = GetCurve(current, next, 16);
                    int partsCount = parts.Length;
                    TimePoint currentPart = parts[0];

                    float localTime = time - current.Time;
                    for (int j = 1; j < partsCount; j++)
                    {
                        TimePoint nextPart = parts[j];

                        if (currentPart.Time <= localTime && nextPart.Time > localTime)
                        {
                            float t = (localTime - currentPart.Time) / (nextPart.Time - currentPart.Time);
                            return Vector2.Lerp(currentPart.Position, nextPart.Position, t);
                        }

                        currentPart = nextPart;
                    }
                }

                current = next;
            }

            return null;
        }

        public IEnumerable<Vector2> GetPositions(Func<float> getTime)
        {
            SyncPoint current = _points[0];
            int count = _points.Count;
            float time = getTime();

            for (int i = 1; i < count; i++)
            {
                SyncPoint next = _points[i];

                while (next.Time > time)
                {
                    int partsCount = 16;
                    var parts = GetCurve(current, next, partsCount);

                    TimePoint currentPart = parts[0];

                    for (int j = 1; j < partsCount; j++)
                    {
                        TimePoint nextPart = parts[j];

                        float cpTime = currentPart.Time;
                        float npTime = nextPart.Time;

                        float duration = npTime - cpTime;

                        Vector2 cpPosition = currentPart.Position;
                        Vector2 npPosition = nextPart.Position;

                        while (npTime > (time = getTime()))
                        {
                            float t = (time - cpTime - current.Time) / duration;
                            yield return Vector2.Lerp(cpPosition, npPosition, t);
                        }

                        currentPart = nextPart;
                    }
                }

                current = next;
            }
        }

        private TimePoint[] GetCurve(SyncPoint a, SyncPoint b, int count)
        {
            Vector2 getCurve(float t) => Maths.GetCurveBy4Point(
                point1: a.Position,
                controlPoint1: a.Position + a.MirroredControlPoint,
                controlPoint2: b.Position + b.ControlPoint,
                point2: b.Position, t);

            float step = 1f / (count - 1);
            float lenght = 0;

            var parts = new TimePoint[count];
            parts[0] = new(lenght, a.Position);

            for (int i = 1; i < count; i++)
            {
                var pos = getCurve(step * i);
                lenght += Vector2.Distance(parts[i - 1].Position, pos);
                parts[i] = new(lenght, pos);
            }

            float speed = lenght / (b.Time - a.Time);

            for (int i = 1; i < count; i++)
                parts[i].Time /= speed;

            return parts;
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}