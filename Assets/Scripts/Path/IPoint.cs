using UnityEngine;

namespace RL.Paths
{
    public interface IPathPoint : ITimePoint
    {
        public Vector2 ControlPoint { get; set; }
        public Vector2 MirroredControlPoint { get; set; }
    }
}