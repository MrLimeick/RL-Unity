using UnityEngine;

namespace RL.Paths
{
    public interface ILine<TimePoint>
        where TimePoint : IPathPoint
    {
        public TimePoint Start { get; }
        public TimePoint End { get; }

        public Vector2[] ControlPoints { get; }
        public PathPoint[] Parts { get; }

        public float Duration { get; }
    }
}