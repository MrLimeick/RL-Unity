using System.Collections.Generic;

using UnityEngine;

using RL.Editor;
using RL.Math;

namespace RL.GameEditor
{
    public class CardEditorLine : MonoBehaviour
    {
        public SpriteRenderer SpritePrefab;
        public List<SpriteRenderer> Sprites;
        public CardEditorPoint NextPoint, PreviousPoint;
        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                Sync();
            }
        }

        public CardEditorCurvePoint CurvePoint
        {
            get => m_CurvePoint;
            set
            {
                m_CurvePoint = value;
                Sync();
            }
        }

        #region Prefabs
        [SerializeField] private int Length = 10;
        [SerializeField] private GameObject m_LinePrefab;
        [SerializeField] private GameObject m_CirclePrefab;
        [SerializeField] private CardEditorCurvePoint m_CurvePoint;
        public bool LoadedFromFile = false;

        public List<Paths.Point> Points = new();
        public List<GameObject> Lines = new();
        public List<GameObject> Circles = new();
        #endregion
        public void Start()
        {
            for (int i = 0; i < Length; i++)
            {
                Lines.Add(Instantiate(m_LinePrefab, transform));
                Circles.Add(Instantiate(m_CirclePrefab, transform));
                Points.Add(new());
            }
            Circles.Add(Instantiate(m_CirclePrefab, transform));
            Points.Add(new());
            if (!LoadedFromFile) CurvePoint.Position = transform.position;
            Sync();
        }

        /*public void OnMouseDown()
        {
            if (Mode == PathMakerModes.Select)
            {
                Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                foreach (var collider in colliders)
                {
                    if (collider.tag == "Point")
                    {
                        return;
                    }
                }
                SelectedPath.CreatePoint(PerviousPoint.Time + (NextPoint.Time - PerviousPoint.Time) / 2, (Vector2)transform.position);
            }
        }*/
        public void Update()
        {
            PreviousPoint.DecarativeCircle.transform.position = new Vector3(PreviousPoint.DecarativeCircle.transform.position.x, PreviousPoint.DecarativeCircle.transform.position.y, -2);
        }

        Vector2 PointAPos => PreviousPoint.transform.position;
        Vector2 PointBPos => NextPoint.transform.position;

        private LineRenderer Renderer;

        private void Awake()
        {
            Renderer = GetComponent<LineRenderer>();
        }

        public void Sync()
        {
            Vector2 posA;
            Paths.Point line;
            float t = 1f / Lines.Count;
            float H = 0;
            for (int i = 0; i < Lines.Count; i++)
            {
                posA = Maths.GetCurveBy3Point(PointAPos, CurvePoint.Position, PointBPos, t * i);
                Vector2 posB = Maths.GetCurveBy3Point(PointAPos, CurvePoint.Position, PointBPos, t * i + t);
                Maths.GetLineTransform(posA, posB, out Vector3 position, out float height, out float angle);

                line = Points[i];
                line.time = PreviousPoint.Time + H;
                line.position = posA;
                Points[i] = line;
                H += height;
                Lines[i].transform.SetPosRotScale(position, Quaternion.Euler(0, 0, angle), new Vector3(0.25f, height));
                Circles[i].transform.position = posA;
            }
            NextPoint.Time = PreviousPoint.Time + H;
            posA = Maths.GetCurveBy3Point((Vector2)PreviousPoint.transform.position, CurvePoint.Position, (Vector2)NextPoint.transform.position, 1);
            if (Circles.Count > 0) Circles[^1].transform.position = posA;
            if (Points.Count > 0)
            {
                line = Points[^1];
                line.time = PreviousPoint.Time + H;
                line.position = posA;
                Points[^1] = line;
            }
        }

        public Vector2 GetPosOnLine(float time)
        {
            int i = -1;
            for (int a = 0; a < Points.Count - 1; a++)
            {
                if (Points[a].time < time && Points[a + 1].time > time)
                {
                    i = a;
                    break;
                }
            }
            if (i == -1) throw new System.Exception("�� ������ ����� ��� ����� � ������ ��������");
            return Vector2.Lerp(Points[i].position, Points[i + 1].position, (time - Points[i].time) / (Points[i + 1].time - Points[i].time));
        }
    }
}