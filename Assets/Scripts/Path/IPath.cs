using System.Collections.Generic;
using UnityEngine;

namespace RL.Paths
{
    public interface IPath<TSelf, TSyncPoint> : IReadOnlyPath, IReadOnlyList<TSyncPoint>
        where TSelf : IPath<TSelf, TSyncPoint>
        where TSyncPoint : ISyncPoint<TSyncPoint, TSelf>
    {
        public TSyncPoint Add(float time, Vector2 position, Vector2? controlPoint = null);

        public void RemoveAt(int index);
    }
}