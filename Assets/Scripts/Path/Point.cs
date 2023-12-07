using UnityEngine;
using System;

namespace RL.Paths
{
    [Serializable]
    public class PathPoint : IPathPoint
    {
        public virtual float Time { get; set; } = 0;

        public virtual Vector2 Position { get; set; } = Vector2.zero;
        public float X { get => Position.x; set => Position = new(value, Y); }
        public float Y { get => Position.y; set => Position = new(X, value); }

        public virtual Vector2 ControlPoint { get; set; }
        public Vector2 MirroredControlPoint
        {
            get => ControlPoint * -1;
            set => ControlPoint = value * -1;
        }

        protected PathPoint() { }

        public PathPoint(float time, float x, float y, float cx = 0, float cy = 0)
        {
            Time = time;
            Position = new(x, y);
            ControlPoint = new(cx, cy);
        }

        public PathPoint(float time, Vector2 position, Vector2? controlPoint = null)
        {
            Time = time;
            Position = position;
            ControlPoint = controlPoint ?? new(0, 0);
        }

        public static implicit operator Vector2(PathPoint a) => a.Position;
        public static implicit operator float(PathPoint a) => a.Time;
    }
}