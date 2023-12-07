using UnityEngine;

namespace RL.Paths
{
    public class SyncPoint : ISyncPoint<SyncPoint, Path>
    {
        public int Index { get; set; }

        public Vector2 Position { get; set; } = Vector2.zero;
        public float Time { get; set; } = 0;
        public Vector2 ControlPoint { get; set; } = Vector2.zero;

        public Vector2 MirroredControlPoint
        {
            get => ControlPoint *= -1;
            set => ControlPoint = value * -1;
        }

        public Path Path { get; set; }
        public SyncPoint Previous => (Index - 1 >= 0) ? Path[Index - 1] : null;
        public SyncPoint Next => (Index + 1 < Path.Count) ? Path[Index + 1] : null;

        public SyncPoint(Path path, int index)
        {
            Path = path;
            Index = index;
        }
    }
}