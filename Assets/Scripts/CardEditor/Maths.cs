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
            Length = Vector2.Distance(Point1Pos, Point2Pos);
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

        public static Vector2 GetCurveBy3Point(Vector2 point1, Vector2 point2, Vector2 point3, float t)
        {
            float a = 1f - t, b = a * a;
            float getCurve(float p1, float p2, float p3) => p1 * b + 2 * t * p2 * a + t * t * p3;
            return new(
                x: getCurve(point1.x, point2.x, point3.x),
                y: getCurve(point1.y, point2.y, point3.y));
        }

        public static Vector2 GetCurveBy4Point(Vector2 point1, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 point2, float t)
        {
            float a = 1f - t;
            float getCurve(float p1, float p2, float p3, float p4)
                => (a * a * a * p1)
                + (3 * Mathf.Pow(a, 2) * t * p2)
                + (3 * a * t * t * p3)
                + (t * t * t * p4);
            return new(
                x: getCurve(point1.x, controlPoint1.x, controlPoint2.x, point2.x),
                y: getCurve(point1.y, controlPoint1.y, controlPoint2.y, point2.y));
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