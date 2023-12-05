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

        public void CreatePoint(float Time, Vector2 Position, Vector2? controlPoint = null)
        {
            //Проверка на уже существующую точку
            if (PointIsExist(Time, out int index))
            {
                Points[index].Position = Position;
                if (controlPoint.HasValue) Points[index].ControlPoint.Position = controlPoint.Value;
            }

            //Если созданная точка будет последняя
            else if (Points.Count <= 0 || Points[^1].Time < Time)
            {
                // Создаём точку
                CardEditorPoint point = SpawnPoint(Time, Position, controlPoint);

                Points.Add(point); // Добавляем точку в массив
                point.Index = Count - 1; // Ставим индекс точке
            }

            //Если точка посреди пути
            else
            {
                // Получаем индекс новой точки
                int newI = GetIndexByTime(Time) ?? throw new Exception("Unknown error");

                // Создаём точку
                CardEditorPoint point = SpawnPoint(Time, Position, controlPoint);

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

                if (index < Count)
                {
                    Points[index].UpdateLine();
                    for (int i = index + 2; i < Count; i++)
                        Points[i].Time = Points[i - 1].Time + Points[i].LineLenght;
                }

                return true;
            }

            return false;
        }

        private CardEditorPoint SpawnPoint(float Time, Vector2 Position, Vector2? controlPoint = null)
        {
            CardEditorPoint PB = Instantiate(PointPrefab, Position, Quaternion.identity, transform);
            PB.Time = Time;
            PB.Path = this;
            if (controlPoint.HasValue) PB.ControlPoint.Position = controlPoint.Value;
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
                        float duration = nextPartTime - currentPartTime;
                        Vector2 currentPartPosition = currentPart.position;
                        Vector2 nextPartPosition = nextPart.position;

                        while (nextPartTime > (time = getTime()))
                        {
                            float t = (time - currentPartTime) / duration;
                            yield return Vector2.Lerp(currentPartPosition, nextPartPosition, t);
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

        public string Save()
        {
            StringBuilder builder = new();
            builder.AppendLine("RLC 1");

            for (int i = 0; i < Count; i++)
            {
                CardEditorPoint point = Points[i];
                Vector2 pos = point.Position;
                Vector2 cpp = point.ControlPoint.Position;

                builder.AppendLine($"P,{point.Time},{pos.x},{pos.y},{cpp.x},{cpp.y}");
            }

            return builder.ToString();
        }

        public bool Load(string saved)
        {
            string[] lines = saved.Split('\n');
            int count = lines.Length;

            //if (saved.StartsWith("RLC")) return false;

            for(int i = 1; i < count; i++)
            {
                string[] e = lines[i].Split(',');
                switch(e[0])
                {
                    case "P": // Point
                        if (!float.TryParse(e[1], out float time) ||
                            !float.TryParse(e[2], out float x) ||
                            !float.TryParse(e[3], out float y) ||
                            !float.TryParse(e[4], out float cpx) ||
                            !float.TryParse(e[5], out float cpy))
                        {
                            Debug.LogError("Не удалось загрузить одну из точек.");
                            break;
                        }

                        CreatePoint(time, new(x, y), new(cpx, cpy));
                        break;
                }
            }

            return true;
        }
    }
}