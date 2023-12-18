using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

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
        {
            throw new NotImplementedException();

            float time = point.Time;
        }

        public SyncPoint Add(float time, Vector2 position, Vector2? controlPoint = null)
        {
            throw new NotImplementedException();

            //if (Count <)

            for (int i = 1; i < Count; i++)
                if (_points[i - 1].Time <= time && _points[i].Time > time)
                {
                    SyncPoint point = new(this, i)
                    {
                        Time = time,
                        Position = position,
                        ControlPoint = controlPoint ?? Vector2.zero
                    };

                    _points.Insert(i, point);
                }
        }

        public IEnumerator<SyncPoint> GetEnumerator() => _points.GetEnumerator();

        public int GetIndexByTime(float time)
        {
            throw new NotImplementedException();
        }

        public Vector2? GetPosition(float time)
        {
            SyncPoint current = _points[0];
            int count = Count;

            for (int i = 1; i < count; i++)
            {
                SyncPoint next = _points[i];

                if (current.Time <= time && next.Time > time)
                {
                    Vector2 controlPoint1 = current.ControlPoint;
                    Vector2 controlPoint2 = next.ControlPoint *= -1;

                    PathPoint[] parts = new Line<SyncPoint>(current, next, 10, controlPoint1, controlPoint2).Parts;
                    int partsCount = parts.Length;
                    PathPoint currentPart = parts[0];

                    float localTime = time - _points[i].Time;
                    for (int j = 1; j < partsCount; j++)
                    {
                        PathPoint nextPart = parts[i];

                        if (currentPart.Time <= localTime && next.Time > localTime)
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
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public bool TryGetIndexByTime(float time, out int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}