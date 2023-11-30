using System.Collections;
using UnityEngine;
using static RL.GameEditor.PathMaker;

namespace RL.GameEditor
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class CardEditorCurvePoint : MonoBehaviour
    {
        public Vector2 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                Line.Sync();
            }
        }

        public CardEditorLine Line;

        private CircleCollider2D Collider;
        private SpriteRenderer Renderer;

        private void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
            Renderer = GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            OnModeChanged += (arg) => Collider.enabled = Renderer.enabled = arg == Modes.EditingCurvesPoints;
        }

        private void OnMouseDrag()
        {
            static float round(float num, float num2) => Mathf.Round(num / num2) * num2;

            var MousePos = (Vector2)PathMaker.Camera.ScreenToWorldPoint(Input.mousePosition);
            Position = new(
                round(MousePos.x, OnEditingCurveGridResolution.x),
                round(MousePos.y, OnEditingCurveGridResolution.y));
        }

        public void OnMouseEnter() => transform.localScale = new Vector2(0.5f, 0.5f);
        public void OnMouseExit() => transform.localScale = new Vector2(0.35f, 0.35f);
    }
}