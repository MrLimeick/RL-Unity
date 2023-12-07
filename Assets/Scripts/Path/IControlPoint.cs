using UnityEngine;

namespace RL.Paths
{
    public interface IControlPoint
    {
        public Vector2 Position { get; set; }
        public Vector2 Mirrored { get; set; }
    }
}