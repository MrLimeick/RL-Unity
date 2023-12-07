using UnityEngine;

namespace RL.Paths
{
    public class TimePoint : ITimePoint
    {
        public float Time { get; set; }
        public Vector2 Position { get; set; }

        public TimePoint()
        {
            Time = 0;
            Position = Vector2.zero;
        }

        public TimePoint(float time, Vector2 position)
        {
            Time = time;
            Position = position;
        }

        public TimePoint(float time, float x, float y)
        {
            Time = time;
            Position = new Vector2(x, y);
        }
    }
}