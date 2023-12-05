using System.Linq;
using RL.Math;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using static RL.CardEditor.PathMaker;
using System.Runtime.CompilerServices;

namespace RL.CardEditor
{
    [AddComponentMenu("RL/Card Editor/Point")]
    [RequireComponent(typeof(CircleCollider2D))]
    public class CardEditorPoint : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent OnSelect = new();
        public UnityEvent OnDeSelect = new();
        public UnityEvent OnRemove = new();

        [Header("Line")]
        [SerializeField] private GameObject ControlPointsGroup;
        public CardEditorCurvePoint ControlPoint;
        [SerializeField] private CardEditorCurvePoint MirroredControlPoint;
        [SerializeField] private Transform ControlPointLine;
        public Paths.Point[] LinePoints;
        public int LinePointsLength => LinePoints.Length;
        public float LineLenght { get; protected set; } = 0;

        [Header("Path")]
        public CardEditorPath Path;
        public int Index = 0;

        public float Time = 0;

        public CardEditorPoint Next => (Path.Count > Index + 1) ? Path[Index + 1] : null;
        public CardEditorPoint Previous => (0 < Index) ? Path[Index - 1] : null;

        public Vector2 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;

                UpdateLine();
            }
        }

        public SpriteRenderer DecarativeCircle;
        public SpriteRenderer graphic;

        public void OnMouseEnter()
            => graphic.transform.localScale = new Vector2(2.0f, 2.0f);

        public void OnMouseExit()
            => graphic.transform.localScale = new Vector2(1.6f, 1.6f);

        bool NoDrag = true;

        private void OnMouseOver()
        {
            int button = Mode switch
            {
                Modes.EditingPath => 0,
                _ => 1
            };

            if (Input.GetMouseButtonDown(button)) NoDrag = true;
            if (Input.GetMouseButtonUp(button) && NoDrag) SelectToggle();
        }

        private void OnMouseDrag()
        {
            static float round(float num, float num2) => Mathf.Round(num / num2) * num2;

            var MousePos = (Vector2)PathMaker.Camera.ScreenToWorldPoint(Input.mousePosition);
            var NewPos = new Vector2(
                x: round(MousePos.x, GridResolution.x),
                y: round(MousePos.y, GridResolution.y));

            Vector2 oldPosition = Position;
            if (Position != NewPos)
            {
                NoDrag = false;

                if (Selected) SelectedPoints.MoveAll(NewPos - oldPosition);
                else Position = NewPos;
            }
        }

        public void OnDestroy()
        {
            OnModeChanged -= CardEditorPoint_OnModeChanged;

            OnRemove?.Invoke();
        }

        public bool Selected { get; protected set; } = false;

        public void OnPointSelect()
        {
            graphic.color = new Color(0, 0, 1, 0.5f);

            Selected = true;
        }

        public void OnPointUnSelect()
        {
            graphic.color = new Color(0, 0, 0, 0.5f);

            Selected = false;
        }

        public void SelectToggle()
        {
            if (!(Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)))
            {
                SelectedPoints.UnSelectAll();
                SelectedPoints.Select(this);
            }
            else
            {
                if (Selected) SelectedPoints.UnSelect(this);
                else SelectedPoints.Select(this);
            }    
        }

        public static CardEditorPoint[] SortByIndex(params CardEditorPoint[] points)
            => points.OrderBy((point) => point.Time).ToArray();

        [SerializeField] private CircleCollider2D m_Collider;
        [SerializeField] private LineRenderer m_LineRenderer;

        private void Awake()
        {
            m_LineRenderer.positionCount = LinePointsLength;
        }

        private void OnValidate()
        {
            TryGetComponent(out m_Collider);
            TryGetComponent(out m_LineRenderer);
        }

        public void UpdateLine()
        {
            UpdateSelfLine();
            var next = Next;
            if (next != null) next.UpdateSelfLine();
        }

        private void UpdateSelfLine()
        {
            {
                Path.GetLine(ControlPoint.transform.localPosition, MirroredControlPoint.transform.localPosition, out _, out float height, out float angle);
                ControlPointLine.transform.rotation = Quaternion.Euler(0, 0, angle);
                ControlPointLine.transform.localScale = new(height, 0.2f);
            }

            if (Index == 0) return;

            var point = transform.position;
            var pointControlPoint = ControlPoint.transform.position;
            var prevPoint = Previous.transform.position;
            var prevControlPoint = Previous.MirroredControlPoint.transform.position;

            float step = 1f / (LinePointsLength - 1);
            float lenght = 0;
            Vector2 getCurve(float t) => Maths.GetCurveBy4Point(
                point1: prevPoint,
                controlPoint1: prevControlPoint,
                controlPoint2: pointControlPoint,
                point2: point, t);

            LinePoints[0].position = getCurve(0);
            LinePoints[0].time = lenght;
            for (int i = 1; i < LinePointsLength; i++)
            {
                LinePoints[i].position = getCurve(step * i);
                lenght += Vector2.Distance(LinePoints[i - 1], LinePoints[i]);
                LinePoints[i].time = lenght;
            }
            LineLenght = lenght;

            m_LineRenderer.SetPositions(LinePoints.Select((p) => (Vector3)p.position).ToArray());

            for (int i = Index; i < Path.Count; i++)
                Path[i].Time = Path[i].Previous.Time + Path[i].LineLenght;
        }

        public void Start()
        {
            OnModeChanged += CardEditorPoint_OnModeChanged;

            UpdateLine();
        }

        private void CardEditorPoint_OnModeChanged(Modes arg0)
        {
            graphic.gameObject.SetActive(m_Collider.enabled = arg0 == Modes.EditingPath);

            ControlPointsGroup.SetActive(arg0 == Modes.EditingControlPoints);
        }
    }
}