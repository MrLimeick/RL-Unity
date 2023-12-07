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

        public IEnumerator<SyncPoint> GetEnumerator() => _points.GetEnumerator();

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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}