using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
//using static RL.GameEditor.PathMaker;

namespace RL.GameEditor
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

        private HierarchyPoint p_HierarchyPoint;
        public HierarchyPoint HierarchyPoint { get => p_HierarchyPoint; set => p_HierarchyPoint = value; }

        CancellationTokenSource PlayerAnim = null;
        public SpriteRenderer DecarativeCircle;
        public SpriteRenderer graphic;

        public void OnMouseEnter()
        {
            PlayerAnim?.Cancel();
            PlayerAnim = new();
            /*await graphic.transform.ScaleAnimationAsync(new Vector3(0.5f,0.5f,0.5f),0.5f,PlayerAnim.Token);*/
            if (PlayerAnim != null && PlayerAnim.IsCancellationRequested) return;
            PlayerAnim = null;
        }

        public void OnMouseExit()
        {
            PlayerAnim?.Cancel();
            PlayerAnim = new();
            /*await graphic.transform.ScaleAnimationAsync(new Vector3(0.35f, 0.35f, 0.35f), 0.5f, PlayerAnim.Token);*/
            if (PlayerAnim != null && PlayerAnim.IsCancellationRequested) return;
            PlayerAnim = null;
        }

        private void OnMouseOver()
        {
            int button = PathMaker.Mode switch
            {
                PathMaker.Modes.EditingPath => 0,
                _ => 1
            };

            if(Input.GetMouseButtonDown(button)) SelectToggle();
        }

        private void OnMouseDrag()
        {
            static float round(float num, float num2) => Mathf.Round(num / num2) * num2;

            var MousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var NewPos = new Vector2(
                x: round(MousePos.x, PathMaker.GridResolution.x),
                y: round(MousePos.y, PathMaker.GridResolution.y));

            Position = NewPos;
            Path.Sync();
        }

        public void OnDestroy()
        {
            if (PlayerAnim != null && !PlayerAnim.IsCancellationRequested) PlayerAnim.Cancel();
            OnRemove?.Invoke();
        }

        public void SelectToggle()
        {
            if (PathMaker.SelectedPoint != null && PathMaker.SelectedPoint == this) PathMaker.SelectedPoint = null;
            else PathMaker.SelectedPoint = this;
        }

        public static CardEditorPoint[] SortByIndex(params CardEditorPoint[] points)
        {
            List<int> indexes = new();
            foreach (CardEditorPoint PB in points)
            {
                indexes.Add(PB.index);
            }
            indexes.Sort();

            string preSort = "";
            foreach (int point in indexes) preSort += point.ToString() + ", ";
            print("Sort: " + preSort);

            List<CardEditorPoint> sortedPoints = new();

            foreach (int index in indexes)
            {
                foreach (CardEditorPoint PB in points)
                {
                    if (PB.index == index)
                    {
                        sortedPoints.Add(PB);
                    }
                }
            }
            return sortedPoints.ToArray();
        }

        private CircleCollider2D Collider;

        private void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
        }

        public void Start()
        {
            PathMaker.OnModeChanged += (arg)
                => graphic.gameObject.SetActive(Collider.enabled = arg == PathMaker.Modes.EditingPath);

            PathMaker.OnSelectedPointChanged += (arg) =>
            {
                if (arg != null && arg == this) graphic.color = new Color(0, 0, 1, 0.5f);
                else graphic.color = new Color(0, 0, 0, 0.5f);
            };
        }
    }
}