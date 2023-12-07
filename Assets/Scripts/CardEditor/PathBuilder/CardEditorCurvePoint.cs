using UnityEngine;
using UnityEngine.Events;
using static RL.CardEditor.PathMaker;

namespace RL.CardEditor
{
    [AddComponentMenu("RL/Card Editor/Curve Point")]
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class CardEditorControlPoint : MonoBehaviour
    {
        public UnityAction<Vector2> OnMoved;

        public Vector2 Position
        {
            get => transform.localPosition;
            set
            {
                if (value == (Vector2)transform.localPosition) return;

                transform.localPosition = value;
                MirroredCurvePoint.transform.localPosition = value * -1;
                OnMoved?.Invoke(value * (IsMirrored ? -1 : 1));
                Point.UpdateLine();
            }
        }

        public CardEditorPoint Point;
        //CardEditorPath Path => Point.Path;

        public bool IsMirrored = false;
        [SerializeField] private CardEditorControlPoint MirroredCurvePoint;

        private CircleCollider2D Collider;
        private SpriteRenderer Renderer;

        private void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
            Renderer = GetComponent<SpriteRenderer>();
        }

        public void Start()
            => OnModeChanged += CardEditorCurvePoint_OnModeChanged;

        private void OnDestroy()
            => OnModeChanged -= CardEditorCurvePoint_OnModeChanged;

        private void CardEditorCurvePoint_OnModeChanged(Modes arg0)
            => Collider.enabled = Renderer.enabled = arg0 == Modes.EditingControlPoints;

        private void OnMouseDrag()
        {
            static float round(float num, float num2) => Mathf.Round(num / num2) * num2;
            static Vector2 roundVec(Vector2 vec, Vector2 vec2)
                => new(round(vec.x, vec2.x), round(vec.y, vec2.y));

            Vector2
                grid = Grids.Resolution,
                scale = new(0.25f, 0.25f), // transform.lossyScale didn't work :( he send or 0.25f or 0.3f randomaly
                mousePos = (MousePos - (Vector2)Point.transform.position) / scale,
                position = roundVec(mousePos, grid / scale);

            Position = position;
        }

        public void OnMouseEnter() => transform.localScale = new Vector2(1.2f, 1.2f);
        public void OnMouseExit() => transform.localScale = new Vector2(1f, 1f);
    }
}