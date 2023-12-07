using System.Linq;
using RL.Math;
using RL.Paths;
using UnityEngine;
using UnityEngine.Events;
using static RL.CardEditor.PathMaker;

namespace RL.CardEditor
{
    [AddComponentMenu("RL/Card Editor/Point")]
    [RequireComponent(typeof(CircleCollider2D))]
    public class CardEditorPoint : MonoBehaviour, ISyncPoint<CardEditorPoint, CardEditorPath>
    {
        [Header("Events")]
        public UnityEvent OnSelect = new();
        public UnityEvent OnDeSelect = new();
        public UnityEvent OnRemove = new();

        [Header("Line")]
        [SerializeField] private GameObject _controlPointsGroup;
        [SerializeField] private CardEditorControlPoint _controlPoint;
        [SerializeField] private CardEditorControlPoint _mirroredControlPoint;

        public Vector2 ControlPoint
        {
            get => _controlPoint.Position;
            set => _controlPoint.Position = value;
        }

        public Vector2 MirroredControlPoint
        {
            get => _mirroredControlPoint.Position;
            set => _mirroredControlPoint.Position = value;
        }

        [SerializeField] private Transform ControlPointLine;

        [System.NonSerialized]
        public PathPoint[] LinePoints;

        [SerializeField] private int _linePointsLenght = 20;
        public int LinePointsLength => _linePointsLenght;
        public float LineLenght { get; protected set; } = 0;

        //[Header("Path")]
        public CardEditorPath Path { get; set; }
        public int Index { get; set; } = 0;

        public float Time { get; set; } = 0;

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
            var MousePos = PathMaker.MousePos;

            Vector2 oldPosition = Position;
            if (Position != MousePos)
            {
                NoDrag = false;

                if (Selected) SelectedPoints.MoveAll(MousePos - oldPosition);
                else Position = MousePos;
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

            LinePoints = new PathPoint[_linePointsLenght];
            for (int i = 0; i < _linePointsLenght; i++) LinePoints[i] = new PathPoint(0, 0, 0, 0, 0);
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
                Path.GetLine(ControlPoint, MirroredControlPoint, out _, out float height, out float angle);
                ControlPointLine.transform.rotation = Quaternion.Euler(0, 0, angle);
                ControlPointLine.transform.localScale = new(height, 0.2f);
            }

            if (Index == 0) return;

            var point = transform.position;
            var pointControlPoint = _controlPoint.transform.position;
            var prevPoint = Previous.transform.position;
            var prevControlPoint = Previous._mirroredControlPoint.transform.position;

            float step = 1f / (LinePointsLength - 1);
            float lenght = 0;
            Vector2 getCurve(float t) => Maths.GetCurveBy4Point(
                point1: prevPoint,
                controlPoint1: prevControlPoint,
                controlPoint2: pointControlPoint,
                point2: point, t);

            LinePoints[0].Position = getCurve(0);
            LinePoints[0].Time = lenght;
            for (int i = 1; i < LinePointsLength; i++)
            {
                LinePoints[i].Position = getCurve(step * i);
                lenght += Vector2.Distance(LinePoints[i - 1], LinePoints[i]);
                LinePoints[i].Time = lenght;
            }
            LineLenght = lenght;

            m_LineRenderer.SetPositions(LinePoints.Select((p) => (Vector3)p.Position).ToArray());

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

            _controlPointsGroup.SetActive(arg0 == Modes.EditingControlPoints);
        }
    }
}