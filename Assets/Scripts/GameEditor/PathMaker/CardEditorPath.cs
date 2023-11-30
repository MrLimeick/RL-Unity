using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RL.Paths;
using RL.Math;
using static RL.GameEditor.PathMaker;

namespace RL.GameEditor
{
    public class CardEditorPath : MonoBehaviour
    {
        [SerializeField]
        private List<CardEditorPoint> m_Points = new();

        /// <summary>
        /// Получить массив со всеми точками
        /// </summary>
        /// <returns></returns>
        public CardEditorPoint[] GetPointsArray() => m_Points.ToArray();

        public CardEditorPoint PointPrefab;
        public CardEditorLine LinePrefab;

        public Hierarchy Hierarchy;

        /// <summary>
        /// Ширина создоваемой линии
        /// </summary>
        public float LineWidth = 0.25f;

        /// <summary>
        /// Синхронизировать путь. Использовать если путь "ломается". 
        /// </summary>
        /// <remarks>Например игрок проехал, проигнорив одну из точек.</remarks>
        public void Sync()
        {
            for (int i = 1; i < Count; i++)
            {
                m_Points[i].index = i;
                m_Points[i - 1].NextLine = m_Points[i].PerviousLine;
                GetLineProperty(m_Points[i - 1].Position, m_Points[i].Position, out Vector2 Position, out float height, out float angle);
                //m_Points[i].Time = m_Points[i - 1].Time + height / m_Points[i - 1].Speed;

                m_Points[i].PerviousLine.Position = Position;

            }
            Hierarchy.Sync();
        }
        public CardEditorPoint this[int index]
        {
            get => m_Points[index];
        }
        /// <summary>
        /// Получить позицию по времени
        /// </summary>
        /// <param name="time">время</param>
        /// <returns>позицию объекта на пути</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public Vector2 GetPosition(float time)
        {
            if (m_Points.Count <= 1) throw new Exception("количество точек меньше 2");
            try
            {
                int i = GetIndexByTime(time);
                //int i = 0;
                //return Maths.GetCurveBy3Point(m_Points[i].Position, m_Points[i].NextLine.CurvePoint, m_Points[i + 1].Position, (time - m_Points[i].Time) / (m_Points[i + 1].Time - m_Points[i].Time));
                return m_Points[i].NextLine.GetPosOnLine(time);
            }
            catch
            {
                return m_Points[0].Position;
            }
        }
        /// <summary>
        /// Получить позицию,вращение,размер для линии
        /// Из A в B
        /// </summary>
        /// <param name="index">индекс точки B</param>
        /// <param name="position">позиция линии</param>
        /// <param name="height">высота линии</param>
        /// <param name="angle">угол вращения</param>
        public void GetLineProperty(Vector2 A, Vector2 B, out Vector2 position, out float height, out float angle)
        {
            position = new Vector3(
                (B.x - A.x) / 2 + A.x,
                (B.y - A.y) / 2 + A.y);
            Vector3 size = new(
                B.x - A.x,
                B.y - A.y);
            height = Mathf.Sqrt(size.x * size.x + size.y * size.y);
            angle =
                Mathf.Atan2(B.y - A.y,
                B.x - A.x) * Mathf.Rad2Deg + 90;
        }
        /// <summary>
        /// Количество точек в пути
        /// </summary>
        public int Count { get => m_Points.Count; }
        /// <summary>
        /// Получить индекс точки A
        /// </summary>
        /// <remarks>+1 для получения точки B</remarks>
        /// <param name="time">время</param>
        /// <returns>индекс точки A</returns>
        public int GetIndexByTime(float time)
        {
            for (int i = 0; i < m_Points.Count - 1; i++)
            {
                if (m_Points[i].Time < time && m_Points[i + 1].Time > time)
                {
                    return i;
                }
            }
            throw new Exception("Точки с данным временем не существует");
        }

        #region PointIsExist

        /// <summary>
        /// Существует ли уже точка с таким времяним
        /// </summary>
        /// <param name="time">время</param>
        /// <returns>true - если существует</returns>
        public bool PointIsExist(float time, out int index)
        {
            for (int i = 0; i < m_Points.Count; i++)
            {
                if (m_Points[i].Time == time)
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
            foreach (CardEditorPoint point in m_Points)
            {
                if (point.Position == Position)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Создать точку и добавить её в путь
        /// </summary>
        /// <param name="point"></param>
        public void CreatePoint(float Time, Vector2 Position)
        {
            //Проверка на первую точку
            if (m_Points.Count <= 0)
            {
                CardEditorPoint PB = SpawnPoint(Time, Position);

                m_Points.Add(PB);
                PB.index = Count - 1;
                Hierarchy.CreatePoint(PB);
                return;
            }
            //Проверка на уже существующую точку
            else if (PointIsExist(Time, out int index))
            {
                m_Points[index].Position = Position;
                Sync();
                return;
            }
            //Если созданная точка будет последняя
            else if (m_Points[^1].Time < Time)
            {
                // Создаём точку
                CardEditorPoint PB = SpawnPoint(Time, Position);

                // Создаём предыдущию линию
                CardEditorLine Line = SpawnLine(m_Points[^1], PB);

                // Ставим линию для точек
                PB.PerviousLine = Line;
                m_Points[^1].NextLine = Line;

                m_Points.Add(PB); // Добавляем точку в массив
                PB.index = Count - 1; // Ставим индекс точке
                Hierarchy.CreatePoint(PB); // Создаём точку в иерархии
                Sync();
            }
            //Если точка где-то в пути
            else
            {
                // Получаем индекс новой точки
                int i = GetIndexByTime(Time);

                // Уничтажаем старую линию
                Destroy(m_Points[i].NextLine.gameObject);

                // Создаём точку
                CardEditorPoint PB = SpawnPoint(Time, Position);

                // Создаём предыдущию линию
                CardEditorLine PerviousLine = SpawnLine(m_Points[i], PB);
                m_Points[i].NextLine = PerviousLine;
                PB.PerviousLine = PerviousLine;

                // Создаём следующию линию
                CardEditorLine NextLine = SpawnLine(PB, m_Points[i + 1]);
                PB.NextLine = NextLine;
                m_Points[i + 1].PerviousLine = NextLine;

                m_Points.Insert(i + 1, PB);
                PB.index = i;
                Hierarchy.CreatePoint(PB);
                Sync();
            }
        }
        /// <summary>
        /// Удалить точку из пути 
        /// </summary>
        /// <param name="point">точка для удаления</param>
        public void RemovePoint(CardEditorPoint point)
        {
            if (m_Points.Contains(point))
            {
                // Если первая
                if (Count < 1)
                {
                    Debug.LogError("Не возможно удалить первую точку, Ей можно только поменять позицию");
                    return;
                }
                // Если предпоследняя
                else if (Count < 2)
                {
                    m_Points.Remove(point);
                    Destroy(point.gameObject);
                    Sync();

                }
                // Если последняя 
                else if (m_Points[^1] == point)
                {
                    m_Points.Remove(point);
                    m_Points[^1].NextLine = null;

                    Destroy(point.PerviousLine.gameObject);
                    Destroy(point.gameObject);
                    Sync();
                }
                //Если где-то в пути
                else
                {
                    m_Points.Remove(point); // удаляем точку из пути
                    Destroy(point.PerviousLine.gameObject); // удаляем предыдущию линию
                    Destroy(point.NextLine.gameObject); // удаляем следующую линию

                    CardEditorLine Line = SpawnLine(m_Points[point.index - 1], m_Points[point.index]);

                    m_Points[point.index - 1].NextLine = Line; // Ставим предыдущей точке новую линию как следующию
                    m_Points[point.index].PerviousLine = Line; // Ставим следующей точке новую линию как предыдущая

                    Destroy(point.gameObject);// Удаляем точку полностью
                    Sync();
                }
            }
            else throw new Exception("Данной точки нет в пути");
        }
        private CardEditorLine SpawnLine(CardEditorPoint PerviousPoint, CardEditorPoint NextPoint)
        {
            GetLineProperty(PerviousPoint.Position, NextPoint.Position, out Vector2 LinePosition, out float LineHeight, out float LineAngle);

            CardEditorLine NextLine = Instantiate(LinePrefab);
            NextLine.PreviousPoint = PerviousPoint;
            NextLine.NextPoint = NextPoint;
            NextLine.transform.transform.position = LinePosition;
            return NextLine;
        }
        private CardEditorPoint SpawnPoint(float Time, Vector2 Position)
        {
            CardEditorPoint PB = Instantiate(PointPrefab, Position, Quaternion.identity);
            PB.Time = Time;
            PB.Path = this;
            return PB;
        }
        public void Clear()
        {
            List<CardEditorPoint> list = m_Points;
            m_Points.Clear();
            foreach (CardEditorPoint point in list)
            {
                RemovePoint(point);
            }
        }
        public Paths.Path GetPath()
        {
            Sync();
            List<Paths.Line> lines = new();
            for (int i = 0; i < m_Points.Count - 1; i++)
            {
                Paths.Line SaveLine = new(
                    new(m_Points[i].Time, m_Points[i].Position),
                    new(m_Points[i + 1].Time, m_Points[i + 1].Position),
                    m_Points[i].NextLine.CurvePoint.Position);
                lines.Add(SaveLine);
            }
            return new Paths.Path(lines.ToArray());
        }

        public void LoadPath(Paths.Path path)
        {
            try
            {
                if (path == null || path.Count == 0)
                {
                    CreatePoint(0, new Vector2(0, 0));
                }
                else
                {
                    var lines = path.GetLinesArray();
                    CreatePoint(lines[0].A.time, lines[0].Position);
                    foreach (var line in lines)
                    {
                        CreatePoint(line.B.time, line.B.position);
                        m_Points[^1].PerviousLine.LoadedFromFile = true;
                        m_Points[^1].PerviousLine.CurvePoint.Position = line.CurvePoint;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось загрузить путь", e);
            }
        }
    }
}