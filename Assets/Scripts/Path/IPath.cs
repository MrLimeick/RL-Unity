using System.Collections.Generic;
using UnityEngine;

namespace RL.Paths
{
    public interface IPath<TSelf, TSyncPoint> : IReadOnlyList<TSyncPoint>
        where TSelf : IPath<TSelf, TSyncPoint>
        where TSyncPoint : ISyncPoint<TSyncPoint, TSelf>
    {
        /// <summary>
        /// Длинна пути.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// Получить позицию на пути.
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Позиция на пути.</returns>
        public Vector2? GetPosition(float time);
    }
}