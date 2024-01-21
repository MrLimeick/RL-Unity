using System;
using System.Collections.Generic;
using UnityEngine;

namespace RL.Paths
{
    public interface IReadOnlyPath 
    {
        /// <summary>
        /// Длинна пути.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// Ширина пути.
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// Получить позицию на пути.
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Позиция на пути.</returns>
        public Vector2? GetPosition(float time);

        public IEnumerable<Vector2> GetPositions(Func<float> getTime);

        public Vector2[] GetPointsPositions();

        public int GetIndexByTime(float time);
    }
}