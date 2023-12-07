using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RL.Paths;
using UnityEngine;

namespace RL.CardEditor
{
    [AddComponentMenu("RL/Card Editor/Path")]
    public class CardEditorPath : MonoBehaviour, IPath<CardEditorPath, CardEditorPoint>
    {
        protected readonly List<CardEditorPoint> _points = new();

        public int Count => _points.Count;

        public float Duration { get; private set; } = 0;
        public float HalfDuration { get; private set; } = 0;

        public CardEditorPoint PointPrefab;

        public float LineWidth = 0.25f;

        public CardEditorPoint this[int index]
        {
            get => _points[index];
            set => throw new NotImplementedException();
        } 

        public void GetLine(Vector2 A, Vector2 B, out Vector2 position, out float height, out float angle)
        {
            position = Vector2.Lerp(A, B, .5f);
            height = Vector2.Distance(A, B);
            angle = Mathf.Atan2(B.y - A.y, B.x - A.x) * Mathf.Rad2Deg;
        }

        public int? GetIndexByTime(float time)
        {
            for (int i = 0; i < Count - 1; i++)
                if (_points[i].Time <= time && _points[i + 1].Time > time)
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
            for (int i = 0; i < _points.Count; i++)
                if (_points[i].Time == time)
                {
                    index = i;
                    return true;
                }

            index = -1;
            return false;
        }
        /// <summary>
        /// Существует ли точка на таких кординатах
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool PointIsExist(Vector2 position, out int index)
        {
            for (int i = 0; i < _points.Count; i++)
                if (_points[i].Position == position)
                {
                    index = i;
                    return true;
                }

            index = 0;
            return false;
        }

        #endregion

        public void CreatePoint(float time, Vector2 position, Vector2? controlPoint = null)
        {
            //Проверка на уже существующую точку
            if (PointIsExist(time, out int index))
            {
                _points[index].Position = position;
                if (controlPoint.HasValue) _points[index].ControlPoint = controlPoint.Value;
            }

            //Если созданная точка будет последняя
            else if (_points.Count <= 0 || _points[^1].Time < time)
            {
                // Создаём точку
                CardEditorPoint point = SpawnPoint(time, position, controlPoint);

                _points.Add(point); // Добавляем точку в массив
                point.Index = Count - 1; // Ставим индекс точке
            }

            //Если точка посреди пути
            else
            {
                // Получаем индекс новой точки
                int newI = GetIndexByTime(time) ?? throw new Exception("Unknown error");

                // Создаём точку
                CardEditorPoint point = SpawnPoint(time, position, controlPoint);

                _points.Insert(newI + 1, point);
                point.Index = newI;

                for (int i = newI + 1; i < Count; i++)
                    _points[i].Index++;
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
                var index = _points.IndexOf(point);
                _points.RemoveAt(index); // удаляем точку из пути
                
                Destroy(point.gameObject); // Удаляем точку полностью

                for (int i = index; i < Count; i++)
                    _points[i].Index = i;

                if (index < Count)
                {
                    _points[index].UpdateLine();

                    for (int i = index + 2; i < Count; i++)
                        _points[i].Time = _points[i - 1].Time + _points[i].LineLenght;
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
            if (controlPoint.HasValue) PB.ControlPoint = controlPoint.Value;
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
            CardEditorPoint current = _points[0];
            int count = _points.Count;

            for (int i = 1; i < count; i++)
            {
                CardEditorPoint next = _points[i];

                if (current.Time <= time && next.Time > time)
                {
                    PathPoint[] parts = next.LinePoints;
                    int partsCount = parts.Length;
                    PathPoint currentPart = parts[0];

                    for(int j = 1; j < partsCount; j++)
                    {
                        PathPoint nextPart = parts[j];

                        float currentPartTime = current.Time + currentPart.Time;
                        float nextPartTime = current.Time + nextPart.Time;

                        if (currentPartTime <= time && nextPartTime > time)
                        {
                            float t = (time - currentPartTime) / (nextPartTime - currentPartTime);
                            return Vector2.Lerp(currentPart.Position, nextPart.Position, t);
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
            CardEditorPoint current = _points[0];
            int count = _points.Count;
            float time = 0;

            for (int i = 1; i < count; i++)
            {
                CardEditorPoint next = _points[i];

                while (next.Time > time)
                {
                    PathPoint[] parts = next.LinePoints;
                    int partsCount = parts.Length;
                    PathPoint currentPart = parts[0];

                    for (int j = 1; j < partsCount; j++)
                    {
                        PathPoint nextPart = parts[j];

                        float currentPartTime = current.Time + currentPart.Time;
                        float nextPartTime = current.Time + nextPart.Time;
                        float duration = nextPartTime - currentPartTime;
                        Vector2 currentPartPosition = currentPart.Position;
                        Vector2 nextPartPosition = nextPart.Position;

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
            for (int i = 0; i < _points.Count; i++) RemovePoint(_points[i]);
            _points.Clear();
        }

        public string Save()
        {
            StringBuilder builder = new();
            builder.AppendLine("RLC 1");

            builder.AppendLine($"N,{name}");

            for (int i = 0; i < Count; i++)
            {
                CardEditorPoint point = _points[i];
                Vector2 pos = point.Position;
                Vector2 cpp = point.ControlPoint;

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
                    case "N": // Name
                        name = e[1];
                        break;
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

        public IEnumerator<CardEditorPoint> GetEnumerator() => _points.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}