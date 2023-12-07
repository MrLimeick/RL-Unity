using UnityEngine;

namespace RL.Paths
{
    public interface ITimePoint
    {
        public float Time { get; set; }

        public Vector2 Position { get; set; }
    }
}