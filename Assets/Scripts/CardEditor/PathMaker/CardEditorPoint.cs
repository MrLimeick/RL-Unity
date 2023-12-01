using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static RL.CardEditor.PathMaker;
using System.Linq;
using Unity.Burst.Intrinsics;

namespace RL.CardEditor
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CardEditorPoint : MonoBehaviour
    {
        public UnityEvent OnSelect = new();
        public UnityEvent OnDeSelect = new();
        public UnityEvent OnRemove = new();

        public CardEditorPath Path;

        public int index = 0;

        public float Time = 0;

        public Vector2 Position { get => transform.position; set => transform.position = value; }

        public float X { get => Position.x; set => Position = new Vector2(value, Position.y); }
        public float Y { get => Position.x; set => Position = new Vector2(Position.x, value); }

        public CardEditorLine NextLine;
        public CardEditorLine PerviousLine;

        public SpriteRenderer DecarativeCircle;
        public SpriteRenderer graphic;

        public void OnMouseEnter()
            => graphic.transform.localScale = new Vector2(0.5f, 0.5f);

        public void OnMouseExit()
            => graphic.transform.localScale = new Vector2(0.35f, 0.35f);

        private void OnMouseOver()
        {
            int button = Mode switch
            {
                Modes.EditingPath => 0,
                _ => 1
            };

            if(Input.GetMouseButtonDown(button)) SelectToggle();
        }

        private void OnMouseDrag()
        {
            static float round(float num, float num2) => Mathf.Round(num / num2) * num2;

            var MousePos = (Vector2)PathMaker.Camera.ScreenToWorldPoint(Input.mousePosition);
            var NewPos = new Vector2(
                x: round(MousePos.x, GridResolution.x),
                y: round(MousePos.y, GridResolution.y));

            Position = NewPos;
            Path.Sync();
        }

        public void OnDestroy()
        {
            OnModeChanged -= CardEditorPoint_OnModeChanged;
            OnSelectedPointChanged -= CardEditorPoint_OnSelectedPointChanged;

            OnRemove?.Invoke();
        }

        public void SelectToggle()
        {
            if (SelectedPoint != null && SelectedPoint == this) SelectedPoint = null;
            else SelectedPoint = this;
        }

        public static CardEditorPoint[] SortByIndex(params CardEditorPoint[] points)
            => points.OrderBy((point) => point.Time).ToArray();

        private CircleCollider2D Collider;

        private void Awake()
            => Collider = GetComponent<CircleCollider2D>();

        public void Start()
        {
            OnModeChanged += CardEditorPoint_OnModeChanged;
            OnSelectedPointChanged += CardEditorPoint_OnSelectedPointChanged;
        }

        private void CardEditorPoint_OnSelectedPointChanged(CardEditorPoint arg0)
        {
            graphic.color = new Color(0, 0, (arg0 != null && arg0 == this) ? 1 : 0, 0.5f);
        }

        private void CardEditorPoint_OnModeChanged(Modes arg0)
        {
            graphic.gameObject.SetActive(Collider.enabled = arg0 == Modes.EditingPath);
        }
    }
}