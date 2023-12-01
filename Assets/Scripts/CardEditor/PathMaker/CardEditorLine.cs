using System.Collections.Generic;
using System.Linq;
using RL.Math;
using UnityEngine;

namespace RL.CardEditor
{
    [RequireComponent(typeof(LineRenderer))]
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

        [SerializeField] private int Length = 10;
        [SerializeField] private CardEditorCurvePoint m_CurvePoint;
        public bool LoadedFromFile = false;

        public Paths.Point[] Points;

        Vector2 PointAPos => PreviousPoint.transform.position;
        Vector2 PointBPos => NextPoint.transform.position;

        private LineRenderer Renderer;

        private void Awake()
        {
            Renderer = GetComponent<LineRenderer>();
            Renderer.positionCount = Length;

            Points = new Paths.Point[Length];
            for (int i = 0; i < Length; i++) Points[i] = new();
        }

        private void Start()
        {
            if (!LoadedFromFile) CurvePoint.Position = transform.position;
            Sync();
        }

        public void Sync()
        {
            float step = 1f / (Length - 1);
            float height = 0;
            Vector2 getCurve(float t) => Maths.GetCurveBy3Point(PointAPos, CurvePoint.Position, PointBPos, t);

            Points[0].position = getCurve(0);
            for (int i = 1; i < Length; i++)
            {
                Points[i].position = getCurve(step * i);
                float distance = Vector2.Distance(Points[i - 1], Points[i]);
                Points[i].time = height;
                height += distance;
            }

            Renderer.SetPositions(Points.Select((p) => (Vector3)p.position).ToArray());

            NextPoint.Time = PreviousPoint.Time + height;
        }

        public Vector2 GetPosOnLine(float time)
        {
            int i = -1;
            for (int a = 0; a < Length - 1; a++)
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