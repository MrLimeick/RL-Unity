using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RL.Paths;
using RL.Math;
using static RL.CardEditor.PathMaker;

namespace RL.CardEditor
{
    [AddComponentMenu("RL/Card Editor/Path")]
    public class CardEditorPath : MonoBehaviour
    {
        public List<CardEditorPoint> Points = new();
        public int Count => Points.Count;

        public CardEditorPoint PointPrefab;

        public float LineWidth = 0.25f;

        public CardEditorPoint this[int index] => Points[index];

        public void GetLine(Vector2 A, Vector2 B, out Vector2 position, out float height, out float angle)
        {
            position = Vector2.Lerp(A, B, .5f);
            height = Vector2.Distance(A, B);
            angle = Mathf.Atan2(B.y - A.y, B.x - A.x) * Mathf.Rad2Deg;
        }

        public int? GetIndexByTime(float time)
        {
            for (int i = 0; i < Count - 1; i++)
                if (Points[i].Time <= time && Points[i + 1].Time > time)
                    return i;

            return null;
        }

        #region PointIsExist

        /// <summary>
        /// Существует ли уже точка с таким времяним
        /// </summary>
        /// <param name="time">время</param>
        /// <returns>true - если существует</returns>
        public bool PointIsExist(float time, out int index)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].Time == time)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
        /// <summary>
        /// Существует ли точка на таких кординатах
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public bool PointIsExist(Vector2 Position)
        {
            foreach (CardEditorPoint point in Points)
            {
                if (point.Position == Position)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        public void CreatePoint(float Time, Vector2 Position)
        {
            //Проверка на уже существующую точку
            if (PointIsExist(Time, out int index))
                Points[index].Position = Position;

            //Если созданная точка будет последняя
            else if (Points.Count <= 0 || Points[^1].Time < Time)
            {
                // Создаём точку
                CardEditorPoint point = SpawnPoint(Time, Position);

                Points.Add(point); // Добавляем точку в массив
                point.Index = Count - 1; // Ставим индекс точке
            }

            //Если точка посреди пути
            else
            {
                // Получаем индекс новой точки
                int newI = GetIndexByTime(Time) ?? throw new Exception("Unknown error");

                // Создаём точку
                CardEditorPoint point = SpawnPoint(Time, Position);

                Points.Insert(newI + 1, point);
                point.Index = newI;

                for (int i = newI + 1; i < Count; i++)
                    Points[i].Index++;
            }
        }
        /// <summary>
        /// Удалить точку из пути 
        /// </summary>
        /// <param name="point">точка для удаления</param>
        public bool RemovePoint(CardEditorPoint point)
        {
            if (point == null) return false;

            // Если первая
            if (Count < 1) Debug.LogError("Невозможно удалить первую точку, Ей можно только поменять позицию");
            
            //Если где-то в пути
            else
            {
                var index = Points.IndexOf(point);
                Points.RemoveAt(index); // удаляем точку из пути
                
                Destroy(point.gameObject); // Удаляем точку полностью

                for (int i = index; i < Count; i++)
                    Points[i].Index = i;

                for (int i = index; i < Count; i++)
                    Points[i].UpdateLine();

                return true;
            }

            return false;
        }

        private CardEditorPoint SpawnPoint(float Time, Vector2 Position)
        {
            CardEditorPoint PB = Instantiate(PointPrefab, Position, Quaternion.identity, transform);
            PB.Time = Time;
            PB.Path = this;
            return PB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns>
        /// Position on path by time.
        /// <para>if <c>null</c>, the time is greater than the time of the last point.</para>
        /// </returns>
        public Vector2? GetPosition(float time)
        {
            CardEditorPoint current = Points[0];
            int count = Points.Count;

            for (int i = 1; i < count; i++)
            {
                CardEditorPoint next = Points[i];

                if (current.Time <= time && next.Time > time)
                {
                    Point[] parts = next.LinePoints;
                    int partsCount = parts.Length;
                    Point currentPart = parts[0];

                    for(int j = 1; j < partsCount; j++)
                    {
                        Point nextPart = parts[j];

                        float currentPartTime = current.Time + currentPart.time;
                        float nextPartTime = current.Time + nextPart.time;

                        if (currentPartTime <= time && nextPartTime > time)
                        {
                            float t = (time - currentPartTime) / (nextPartTime - currentPartTime);
                            return Vector2.Lerp(currentPart.position, nextPart.position, t);
                        }

                        currentPart = nextPart;
                    }
                }

                current = next;
            }
                
            return null;
        }

        public IEnumerable<Vector2> GetPositions(Func<float> getTime)
        {
            CardEditorPoint current = Points[0];
            int count = Points.Count;
            float time = 0;

            for (int i = 1; i < count; i++)
            {
                CardEditorPoint next = Points[i];

                while (next.Time > time)
                {
                    Point[] parts = next.LinePoints;
                    int partsCount = parts.Length;
                    Point currentPart = parts[0];

                    for (int j = 1; j < partsCount; j++)
                    {
                        Point nextPart = parts[j];

                        float currentPartTime = current.Time + currentPart.time;
                        float nextPartTime = current.Time + nextPart.time;

                        while (nextPartTime > (time = getTime()))
                        {
                            float t = (time - currentPartTime) / (nextPartTime - currentPartTime);
                            yield return Vector2.Lerp(currentPart.position, nextPart.position, t);
                        }

                        currentPart = nextPart;
                    }
                }

                current = next;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Points.Count; i++) RemovePoint(Points[i]);
            Points.Clear();
        }
    }
}