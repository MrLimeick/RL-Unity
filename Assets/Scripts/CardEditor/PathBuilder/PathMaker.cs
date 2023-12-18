using System;
using RL.Game;
using RL.Math;
using RL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RL.CardEditor
{
    public partial class PathMaker : MonoBehaviour
    {
        private static PathMaker Instance { get; set; } = null;

        public static SelectedPointsList SelectedPoints { get; protected set; } = new();
        public static PathsList Paths { get; protected set; }
        public static GridSettings Grids { get; protected set; }

        public static Vector2 LastPointPosition => Instance.m_Preview.Start;
        public static bool GlobalGrid = false;

        public void SetGlobalGrid(bool value)
            => GlobalGrid = value;

        public static Vector2 MousePos
        {
            get
            {
                Vector2 mousePos = Instance.m_Camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 center = GlobalGrid ? Vector2.zero : LastPointPosition;

                switch (BuildMode)
                {
                    case BuildModes.ByGrid:
                        Vector2 grid = Grids.Resolution;
                        return Maths.Round(mousePos - center, grid) + center;
                    default:
                        Maths.GetLineTransform(center, mousePos, out _, out float dis, out float angle);
                        return Maths.GetPositionByAngle(Maths.Round(angle, 5f), Mathf.Max(Maths.Floor(dis, Grids.Radius), Grids.Radius)) + center;
                }
            }
        }

        public void SetDistanceSnapStep(string x)
        {
            if (float.TryParse(x, out float res))
            {
                Grids.Radius = res;
                Debug.Log("Distance snap set successful.");
            }
            else Debug.LogError("value is NaN.");
        }

        [Serializable] private class ToolsButtons
        {
            public Button EditingPath;
            public Button BuildPath;
            public Button EditingCurves;

            public void Init()
            {
                EditingPath.OnClick.AddListener(() => Mode = Modes.EditingPath);
                BuildPath.OnClick.AddListener(() => Mode = Modes.BuildsPath);
                EditingCurves.OnClick.AddListener(() => Mode = Modes.EditingControlPoints);
            }
        }
        [SerializeField] private ToolsButtons m_ToolsButtons;

        #region Paths

        [Header("Paths")]
        [SerializeField] private TMP_Dropdown PathsDropdown;

        public void ChangePathButton(int index)
            => Paths.Index = index;

        public void CreatePathButton()
        {
            Paths.Create();

            PathsDropdown.Hide();
        }

        public async void DeletePathButton()
        {
            bool ans = await Dialog.ShowQuestion("Remove path?", "You realy want delete this path?");
            if (!ans) return;

            Paths.DeleteCurrent();
            PathsDropdown.Hide();
        }

        #endregion

        [Header("Prefabs")]
        [SerializeField] private CardEditorPoint PointPrefab;

        [Header("Preview")]
        [SerializeField] private Transform Point;
        [SerializeField] private Transform Line;

        private LinePreview m_Preview;

        [Header("Grid")]
        [SerializeField] private GridCamera GridCamera;

        #region Modes
        public static event UnityAction<Modes> OnModeChanged;

        private bool InPreview => Player.Moved;

        private Modes m_Mode = Modes.None;
        public static Modes Mode
        {
            get => Instance.m_Mode;
            set
            {
                if (value == Instance.m_Mode) return;

                Instance.m_Mode = value;
                Instance.m_Preview.Enabled = true;
                Grids.Resolution = value switch
                {
                    Modes.BuildsPath or Modes.EditingPath => Grids.BuildPath,
                    _ => Grids.EditingCurves
                };
                OnModeChanged?.Invoke(value);
            }
        }

        private BuildModes m_BuildMode = BuildModes.ByGrid;
        public static BuildModes BuildMode // TODO: BuildMode это просто режим сетки?
        {
            get => Instance.m_BuildMode;
            set
            {
                if (value == Instance.m_BuildMode) return;

                Instance.m_BuildMode = value;
                Instance.GridCamera.GridType = value switch
                {
                    BuildModes.ByGrid => GridType.Square,
                    BuildModes.LockedHeight => GridType.Circle,
                    _ => throw new NotImplementedException()
                };
            }
        }

        public void UseDistanceSnap(bool value)
        {
            BuildMode = value ? BuildModes.LockedHeight : BuildModes.ByGrid;
        }
        #endregion

        [Header("Other")]
        [SerializeField] EditorViewport Viewport;
        [SerializeField] CameraController MainCameraController;

        [SerializeField] Camera m_Camera;
        public static Camera Camera => Instance.m_Camera;

        [SerializeField] Player m_Player;
        public static Player Player => Instance.m_Player;
        EventSystem EventSystem;

        static bool InViewport => EditorViewport.IsStay;

        public void SetGameSpeed(string value)
        {
            value.Replace('%', ' ');
            value.Trim();

            if (int.TryParse(value, out int res))
            {
                Time.timeScale = res / 100f;
                Debug.Log("Time set successful.");
            }
            else Debug.LogError("value is NaN.");
        }

        private bool CamCanMove
        {
            get => MainCameraController.CanMove;
            set => MainCameraController.CanMove = value;
        }

        private bool CamCanScroll
        {
            get => MainCameraController.CanScroll;
            set => MainCameraController.CanScroll = value;
        }

        public void Awake()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            Instance = this;

            EventSystem = EventSystem.current;

            m_ToolsButtons.Init();

            Grids = new(GridCamera);
            m_Preview = gameObject.AddComponent<LinePreview>();
            m_Preview.Line = Line;
            m_Preview.Point = Point;
            m_Preview.Enabled = true;

            Paths = new(PointPrefab, PathsDropdown);
            Player.Path = Paths[0];
            Paths.CurrentChanged.AddListener((path) =>
            {
                Player.Path = path;
                if (Player.Moved) Player.Stop();
            });

            #region Viewport events

            Viewport.OnLeftClick.AddListener((_) =>
            {
                if (Mode == Modes.BuildsPath)
                    Paths.Current.Add(Paths.Current[^1].Time + m_Preview.Lenght, m_Preview.End);
            }); // При нажатии левой клавишой мышки по Editor Viewport создавать новый точку и линию
            Viewport.OnStay.AddListener((_)
                => m_Preview.Update()); // Если находится в EditorViewport
            Viewport.OnEnter.AddListener((_) =>
            {
                m_Preview.Enabled = true;
                CamCanMove = true;
                CamCanScroll = true;
            }); // Если вошёл в viewport
            Viewport.OnExit.AddListener((_) =>
            {
                m_Preview.Enabled = false;
                CamCanMove = false;
                CamCanScroll = false;
            }); // Если вышел из viewport

            #endregion

            Mode = Modes.BuildsPath;
            BuildMode = BuildModes.ByGrid;
        }

        public void Update()
        {
            #region Hotkeys
            if (EventSystem.currentSelectedGameObject != null) return;

            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) Save();
            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) Load();

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                SelectedPoints.RemoveAll();

            if (Input.GetKeyDown(KeyCode.R) && Paths.Current.Count > 1)
                Paths.Current.Remove(Paths.Current[^1]);

            if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    Mode = Modes.EditingPath;
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    Mode = Modes.BuildsPath;
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    Mode = Modes.EditingControlPoints;

                if (Input.GetKeyDown(KeyCode.S))
                {
                    GUIUtility.systemCopyBuffer = Paths.Current.Save();
                    Debug.Log("Path has been saved");
                }

                if(Input.GetKeyDown(KeyCode.L))
                {
                    string buffer = GUIUtility.systemCopyBuffer;
                    if (buffer.StartsWith("RLC"))
                    {
                        Paths.Load(buffer);
                        Debug.Log("Path has been loaded");
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) // Enter to preview mode
            {
                if (InPreview) Player.Stop();
                else Player.Move();
            }
            #endregion
        }

        public async void Close()
        {
            var ans = await Dialog.ShowQuestion("Quit?", "Are you sure you want to exit the card editor?");
            if (ans)
            {
                OnModeChanged = (_) => { };

                SceneLoader.LoadScene(0);
            }
        }
    }
}
