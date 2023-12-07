using UnityEngine;

namespace RL.Paths
{
    public class ControlPoint : IControlPoint
    {
        public Vector2 Position { get; set; } 
        public Vector2 Mirrored
        {
            get => Position * -1f;
            set => Position = value * -1f;
        }
    }
}