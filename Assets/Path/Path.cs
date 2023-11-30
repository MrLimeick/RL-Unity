using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RL.Math;
using System.Runtime.CompilerServices;

namespace RL.Paths
{
    public class Less2Exeption : Exception
    {
        public Less2Exeption(Exception innerException) : base("���������� ����� � ���� ����� 2", innerException) { }
        public Less2Exeption(string Message) : base(Message) { }
        public Less2Exeption() : base("���������� ����� � ���� ����� 2") { }
    }
    [Serializable]
    public class Path
    {
        [SerializeField]
        private List<Line> Lines;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// �������� ������� �� �������
        /// </summary>
        /// <param name="time">�����</param>
        /// <returns>������� ������� �� ����</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public Vector2 GetPosition(float time)
        {
            if (Lines.Count <= 1) throw new Less2Exeption(); // ���������� ������ ���� � ���� ������ ���� �����
            try
            {
                int i = GetIndex(time); // �������� ������ �����
                return Lines[i].GetPositionOnLine(time);
            }
            catch
            {
                return Lines[0].A.position;
            }
        }
        /// <summary>
        /// �������� ������ ���� ����� � ����
        /// </summary>
        /// <returns>����� ����</returns>
        public Line[] GetLinesArray() => Lines.ToArray();
        /// <summary>
        /// �������� ������ ���� ����� ����
        /// </summary>
        /// <returns>����� ����</returns>
        public Point[] GetPointsArray()
        {
            List<Point> points = new();
            foreach (Line line in Lines)
            {
                points.Add(line.A);
            }
            points.Add(Lines[^1].B);
            return points.ToArray();
        }
        /// <summary>
        /// ���������� ����� � ����
        /// </summary>
        public int Count { get => Lines.Count; }
        /// <summary>
        /// �������� ������ ����� A
        /// </summary>
        /// <remarks>+1 ��� ��������� ����� B</remarks>
        /// <param name="time">�����</param>
        /// <returns>������ ����� A</returns>
        public int GetIndex(float time)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].A.time <= time && Lines[i].B.time > time)
                {
                    return i;
                }
            }
            throw new Exception("����� � ������ �������� �� ����������");
        }
        public Line this[int index]
        {
            get => Lines[index];
        }
        public Path(params Line[] lines)
        {
            Lines = new();
            Lines.AddRange(lines);
        }

        //public Path GetPartOfPath(float FromTime,float ToTime)
        //{
        //    int FromIndex = GetIndex(FromTime);
        //    int ToIndex = GetIndex(ToTime);
        //
        //    List<Point> points = new();
        //
        //    points.Add(new(0, GetPosition(FromTime)));
        //
        //    for (int i = FromIndex + 1; i <= ToIndex; i++)
        //    {
        //        Point point = Lines[i];
        //        point.time -= FromTime;
        //        points.Add(point);
        //    }
        //
        //    points.Add(new(ToTime - FromTime, GetPosition(FromTime), Lines[ToIndex].Pivot, Lines[ToIndex].Speed));
        //
        //    return new Path(points.ToArray());
        //}
    }

    [Serializable]
    public struct Point
    {
        [SerializeField] public float time;
        [SerializeField] public Vector2 position;

        public float X { readonly get => position.x; set => position.x = value; }
        public float Y { readonly get => position.y; set => position.x = value; }

        public Point(float time, float x, float y)
        {
            this.time = time;
            position = new(x, y);
        }

        public Point(float time, Vector2 position)
        {
            this.time = time;
            this.position = position;
        }

        public static implicit operator Vector2(Point a) => a.position;
        public static implicit operator float(Point a) => a.time;

    }
    [Serializable]
    public struct Line
    {
        /// <summary>
        /// Центральная позиция
        /// </summary>
        public readonly Vector2 Position => Maths.GetPosition(A.position, B.position);
        /// <summary>
        /// Длинна
        /// </summary>
        public float Lenght { get; private set; }
        /// <summary>
        /// Угол
        /// </summary>
        public readonly float Angle => Maths.GetAngle(A.position, B.position);
        /// <summary>
        /// Скорость игрока на этой линии
        /// </summary>
        public float Speed;

        [SerializeField]
        private Vector2 m_CurvePoint;
        /// <summary>
        /// Точка кривой Безье
        /// </summary>
        public Vector2 CurvePoint 
        {
            readonly get => m_CurvePoint;
            set
            {
                m_CurvePoint = value;
                Update();
            }
        }
        [SerializeField]
        private Point m_A; 
        /// <summary>
        /// Начальная точка
        /// </summary>
        public Point A
        {
            readonly get => m_A;
            set
            {
                m_A = value;
                Update();
            }
        }
        [SerializeField] private Point m_B;
        /// <summary>
        /// Конечная точка
        /// </summary>
        public Point B
        {
            readonly get => m_B;
            set
            {
                m_B = value;
                Update();
            }
        }

        #region ����� �����
        public struct PartOfLine
        {
            public Point A, B;
            public PartOfLine(Point A,Point B)
            {
                this.A = A;
                this.B = B;
            }
            /// <summary>
            /// �������� ������� �� ���� ����� �����
            /// </summary>
            /// <param name="t">�� A.time �� B.time</param>
            /// <returns></returns>
            public readonly Vector2 GetPosition(float t)
            {
                return Vector2.Lerp(A.position, B.position, (t - A.time) / (B.time - A.time));
            }
        }
        private List<PartOfLine> Parts;
        #endregion
        #region ������������
        public Line(Point A,Point B,Vector2 CurvePoint, float Speed = 1)
        {
            this.Speed = Speed;
            this.Lenght = 0;

            Parts = new();
            for (int i = 0; i < 10; i++) Parts.Add(new());

            m_CurvePoint = CurvePoint;
            m_A = A;
            m_B = B;

            Update();
        }
        public Line(Point A, Point B, float Speed = 1)
        {
            this.Speed = Speed;
            this.Lenght = 0;

            Parts = new();
            for (int i = 0; i < 10; i++) Parts.Add(new());

            m_CurvePoint = Maths.GetPosition(A.position,B.position);
            m_A = A;
            m_B = B;

            Update();
        }
        #endregion

        /// <summary>
        /// Получить позицию на этой линии
        /// </summary>
        /// <param name="t">коофицент времени от 0 до B.Time</param>
        /// <returns></returns>
        public Vector2 GetPositionOnLine(float t)
        {
            Update();

            t = Mathf.Clamp(t, A.time, B.time);

            int i = -1;
            for (int a = 0; a < Parts.Count; a++)
            {
                if (Parts[a].A.time <= t && Parts[a].B.time > t)
                {
                    i = a;
                    break;
                }
            }
            return Parts[i].GetPosition(t);
        }
        /// <summary>
        /// Получить конечную точку 
        /// </summary>
        /// <returns></returns>
        public readonly Point GetBWithoutSpeed()
        {
            Point B = this.B;
            B.time = A.time + Lenght;
            return B;
        }
        private void Update()
        {
            if(Parts == null)
            {
                Parts = new();
                for (int i = 0; i < 10; i++) Parts.Add(new());
            }
            
            float s = 1f / 10f; // Шаг
            float H = 0f;

            for (int i = 0; i < 10; i++)
            {
                Vector2 point1pos = Maths.GetCurveBy3Point(A.position, m_CurvePoint, B.position, s * i);
                Vector2 point2pos = Maths.GetCurveBy3Point(A.position, m_CurvePoint, B.position, s * i + s);

                PartOfLine point = Parts[i];

                point.A.time = A.time + H;
                point.A.position = point1pos;

                H += Maths.GetLength(point1pos,point2pos);

                point.B.time = A.time + H;
                point.B.position = point2pos;

                Parts[i] = point;
            }
            Lenght = H;
            m_B.time = A.time + H;
        }

        public PartOfLine[] GetPartsOfLine()
        {
            Update();
            return Parts.ToArray();
        }
    }
}