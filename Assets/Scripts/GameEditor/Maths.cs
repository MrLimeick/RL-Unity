using System.Collections;
using UnityEngine;

namespace RL.Math
{
    public class Maths
    {
        /// <summary>
        /// Получить позицию, длинну, угол вращения для линии
        /// </summary>
        /// <param name="Position">позиция</param>
        /// <param name="Length">длинна</param>
        /// <param name="Angle">угол вращения</param>
        public static void GetLineTransform(Vector3 Point1Pos, Vector3 Point2Pos, out Vector3 Position, out float Length, out float Angle)
        {
            Position = GetPosition(Point1Pos, Point2Pos);
            Length = GetLength(Point1Pos, Point2Pos);
            Angle = GetAngle(Point1Pos, Point2Pos);
        }
        /// <summary>
        /// Полчить среднию позицию двух точек
        /// </summary>
        /// <param name="Point1Pos">Позиция точки A</param>
        /// <param name="Point2Pos">Позиция точки B</param>
        /// <returns></returns>
        public static Vector3 GetPosition(Vector3 Point1Pos, Vector3 Point2Pos)
        {
            return new Vector3(
                (Point2Pos.x - Point1Pos.x) / 2 + Point1Pos.x,
                (Point2Pos.y - Point1Pos.y) / 2 + Point1Pos.y);
        }
        /// <summary>
        /// Получить угол точки A смотрящую на точку B
        /// </summary>
        /// <param name="Point1Pos">Позиция точки A</param>
        /// <param name="Point2Pos">Позиция точки B</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 Point1Pos, Vector3 Point2Pos)
        {
            return Mathf.Atan2(Point2Pos.y - Point1Pos.y,
                Point2Pos.x - Point1Pos.x) * Mathf.Rad2Deg + 90;
        }
        /// <summary>
        /// Получить длину от точки A до точки B
        /// </summary>
        /// <param name="Point1Pos">Позиция точки A</param>
        /// <param name="Point2Pos">Позиция точки B</param>
        /// <returns></returns>
        public static float GetLength(Vector3 Point1Pos, Vector3 Point2Pos)
        {
            Vector3 size = new(
                Point2Pos.x - Point1Pos.x,
                Point2Pos.y - Point1Pos.y);
            return Mathf.Sqrt(size.x * size.x + size.y * size.y);
        }
        /// <summary>
        /// Получить позицию на кривой
        /// </summary>
        /// <param name="point1">точка A</param>
        /// <param name="point2">точка B</param>
        /// <param name="point3">точка C</param>
        /// <param name="t">параметр от 0 до 1</param>
        /// <returns>позцию на кривой</returns>
        /*public static Vector2 GetCurveByPivot(Vector2 point1, Vector2 point2, Pivot pivot, float t)
        {
            Vector2 sq = point2 - point1;
            Vector2 Pivot = sq * pivot;
            Pivot += point1;
            Vector2 point = new Vector2
                (
                (1f - t) * (1f - t) * point1.x + 2 * (1f - t) * t * pivot.x + t * t * point2.x,
                (1f - t) * (1f - t) * point1.y + 2 * (1f - t) * t * pivot.y + t * t * point2.y
                );
            return point;
        }*/
        public static Vector2 GetCurveBy3Point(Vector2 point1, Vector2 point2, Vector2 point3, float t)
        {
            /*Vector2 point = new Vector2
                (
                (1f - t) * (1f - t) * point1.x + 2 * (1f - t) * t * point2.x + t * t * point3.x,
                (1f - t) * (1f - t) * point1.y + 2 * (1f - t) * t * point2.y + t * t * point3.y
                );*/
            Vector2 point = new Vector2
                (
                point1.x * ((1f - t) * (1f - t)) + 2 * t * point2.x * (1f - t) + t * t * point3.x,
                point1.y * ((1f - t) * (1f - t)) + 2 * t * point2.y * (1f - t) + t * t * point3.y
                );
            return point;
        }
    }
    public static class TransformExtension
    {
        public static void SetPosRotScale(this Transform transform, Vector2 pos, Quaternion rot, Vector2 scale)
        {
            transform.SetPositionAndRotation(pos, rot);
            transform.localScale = scale;
        }
    }
}