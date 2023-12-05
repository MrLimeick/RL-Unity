using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RL.Game;
using RL.Paths;
using RL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RL.CardEditor
{
    public partial class PathMaker : MonoBehaviour
    {
        private static PathMaker Instance { get; set; } = null;

        [SerializeField] private Camera m_Camera;
        public static Camera Camera => Instance.m_Camera;

        [SerializeField] private Player Player;
        [SerializeField] private EventSystem EventSystem;

        public static Vector2 MousePos => Camera.ScreenToWorldPoint(Input.mousePosition);

        [Header("Tools Buttons")]
        public Button EditingPathButton;
        public Button BuildPathByGridButton;
        public Button BuildPathWithLockedLenghtButton;
        public Button EditingCurvesButton;

        public static event UnityAction<CardEditorPoint> OnSelectedPointChanged;
        public static event UnityAction<Modes> OnModeChanged;

        private CardEditorPoint m_SelectedPoint = null;
        public static CardEditorPoint SelectedPoint
        {
            get => Instance.m_SelectedPoint;
            set
            {
                if (SelectedPoint != null) SelectedPoint.OnDeSelect.Invoke();
                Instance.m_SelectedPoint = value;
                if (SelectedPoint != null) SelectedPoint.OnSelect.Invoke();

                OnSelectedPointChanged?.Invoke(SelectedPoint);
            }
        }

        private CardEditorPath m_SelectedPath = null;
        public static CardEditorPath SelectedPath
        {
            get => Instance.m_SelectedPath;
            set
            {
                Instance.m_SelectedPath = value;
            }
        }

        [Header("Prefabs")]
        [SerializeField] private CardEditorPoint PointPrefab;

        #region Preview
        [Header("Preview")]
        [SerializeField] private GameObject PreviewPoint;
        [SerializeField] private GameObject PreviewLine;

        private bool m_IsPreviewEnabled = false;
        public static bool IsPreviewEnabled
        {
            get => Instance.m_IsPreviewEnabled;
            set
            {
                bool val = InViewport && Mode == Modes.BuildsPath && value;

                Instance.m_IsPreviewEnabled = val;

                Instance.PreviewLine.SetActive(val);
                Instance.PreviewPoint.SetActive(val);
            }
        }

        [Header("Grid")]
        [SerializeField] private GridCamera Grid;
        
        private Vector2 m_GridResolution = new(1f, 1f);
        public static Vector2 GridResolution
        {
            get => Instance.m_GridResolution;
            set
            {
                Instance.m_GridResolution = value;
                static float clamp(float num) => Mathf.Clamp(num, 0.01f, 10f);
                Instance.Grid.Resolution = new(clamp(value.x), clamp(value.y));
            }
        }

        [SerializeField] private Vector2 m_OnBuildPathGridResolution = new(1f, 1f);
        [SerializeField] private Vector2 m_OnEditingCurveGridResolution = new(0.25f, 0.25f);

        public static Vector2 OnBuildPathGridResolution { get => Instance.m_OnBuildPathGridResolution; set => Instance.m_OnBuildPathGridResolution = value; }
        public static Vector2 OnEditingCurveGridResolution { get => Instance.m_OnEditingCurveGridResolution; set => Instance.m_OnEditingCurveGridResolution = value; }

        public class Preview
        {
            /// <summary>
            /// Позиция изменяемой на данный момент линии
            /// </summary>
            public static Vector3 Position => new(
                (EndPoint.x - StartPoint.x) / 2 + StartPoint.x,
                (EndPoint.y - StartPoint.y) / 2 + StartPoint.y);

            /// <summary>
            /// Ширина изменяемой на данный момент линии
            /// </summary>
            public const float Thickness = 0.25f;

            /// <summary>
            /// Угол изменяемой на данный момент линии
            /// </summary>
            public static float Angle => Mathf.Atan2(
                EndPoint.y - StartPoint.y,
                EndPoint.x - StartPoint.x) * Mathf.Rad2Deg;

            /// <summary>
            /// Высота изменяемой на данный момент линии
            /// </summary>
            public static float Lenght => Vector2.Distance(StartPoint, EndPoint);

            /// <summary>
            /// Позиция точки A
            /// </summary>
            public static Vector2 StartPoint => SelectedPath[^1].Position;

            /// <summary>
            /// Позиция точки B
            /// </summary>
            public static Vector2 EndPoint
            {
                get
                {
                    var MousePos = PathMaker.MousePos;
                    MousePos = new Vector2(
                           Mathf.Round(MousePos.x / GridResolution.x) * GridResolution.x,
                           Mathf.Round(MousePos.y / GridResolution.y) * GridResolution.y);
                    return MousePos;
                }
            }
        }

        #endregion

        private Modes m_Mode = Modes.None;
        public static Modes Mode
        {
            get => Instance.m_Mode;
            set
            {
                if (value == Instance.m_Mode) return;

                Instance.m_Mode = value;
                IsPreviewEnabled = true;
                GridResolution = value switch
                {
                    Modes.BuildsPath or Modes.EditingPath => Instance.m_OnBuildPathGridResolution,
                    _ => Instance.m_OnEditingCurveGridResolution
                };
                OnModeChanged?.Invoke(value);
            }
        }

        private BuildModes m_BuildMode = BuildModes.ByGrid;
        public static BuildModes BuildMode
        {
            get => Instance.m_BuildMode;
            set
            {
                if (value == Instance.m_BuildMode) return;

                Instance.m_BuildMode = value;
            }
        }

        [Header("Other")]
        [SerializeField] private EditorViewport Viewport;
        [SerializeField] private CameraController MainCameraController;

        private static bool InViewport => EditorViewport.IsStay;

        private const string USE_TRACHPAD_KEY = "CardEditor_UseTrackpad";

        public static bool UseTrackpad
        {
            get => Instance.MainCameraController.IsTrackpad;
            set
            {
                if (UseTrackpad == value) return;

                Instance.MainCameraController.IsTrackpad = value;
                PlayerPrefs.SetInt(USE_TRACHPAD_KEY, value ? 1 : 0);
            }
        }

        public void SetUseTrackpad(bool value) => UseTrackpad = value;

        public void SetGameSpeed(string value)
        {
            value.Replace('%', ' ');
            value.Trim();

            if (int.TryParse(value, out int res))
            {
                Time.timeScale = res / 100;
                Debug.Log("Time set successful.");
            }
            else Debug.LogError("value is NaN.");
        }

        public void Awake()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            Instance = this;

            if (PlayerPrefs.HasKey(USE_TRACHPAD_KEY))
                MainCameraController.IsTrackpad = PlayerPrefs.GetInt(USE_TRACHPAD_KEY) == 1;

            EditingPathButton.OnClick.AddListener(() =>
            {
                Mode = Modes.EditingPath;
            });
            BuildPathByGridButton.OnClick.AddListener(() =>
            {
                Mode = Modes.BuildsPath;
                BuildMode = BuildModes.ByGrid;
            });
            BuildPathWithLockedLenghtButton.OnClick.AddListener(() =>
            {
                Mode = Modes.BuildsPath;
                BuildMode = BuildModes.LockedHeight;
            });
            EditingCurvesButton.OnClick.AddListener(() =>
            {
                Mode = Modes.EditingControlPoints;
            });

            Load(null);

            Player.Path = Paths[0];

            #region Viewport events

            Viewport.OnLeftClick.AddListener((_) =>
            {
                if (Mode == Modes.BuildsPath)
                    SelectedPath.CreatePoint(SelectedPath[^1].Time + Preview.Lenght, Preview.EndPoint);
            }); // При нажатии левой клавишой мышки по Editor Viewport создавать новый точку и линию
            Viewport.OnStay.AddListener((_) =>
            {
                PreviewLine.transform.position = Preview.Position;
                PreviewLine.transform.localScale = new Vector3(Preview.Lenght, Preview.Thickness, 0);
                PreviewLine.transform.rotation = Quaternion.Euler(0, 0, Preview.Angle);
                PreviewPoint.transform.position = Preview.EndPoint;
            }); // Если находится в EditorViewport
            Viewport.OnEnter.AddListener((_) =>
            {
                IsPreviewEnabled = true;

                MainCameraController.CanMove = true;
                MainCameraController.CanScroll = true;
            }); // Если вошёл в viewport
            Viewport.OnExit.AddListener((_) =>
            {
                IsPreviewEnabled = false;

                MainCameraController.CanMove = false;
                MainCameraController.CanScroll = false;
            }); // Если вышел из viewport

            #endregion

            Mode = Modes.BuildsPath;
            BuildMode = BuildModes.ByGrid;
        }

        /// <summary>
        /// Все пути для игрока
        /// </summary>
        public List<CardEditorPath> Paths = new();

        private bool InPreview => Player.Moved;

        public void Update()
        {
            #region Hotkeys
            if (EventSystem.currentSelectedGameObject != null) return;

            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) Save();
            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) Load();

            if ((Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) && SelectedPoint != null) SelectedPath.RemovePoint(SelectedPoint);
            if (Input.GetKeyDown(KeyCode.R) && SelectedPath.Count > 1) SelectedPath.RemovePoint(SelectedPath[^1]);

            if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) Mode = Modes.EditingPath;
                if (Input.GetKeyDown(KeyCode.Alpha2)) Mode = Modes.BuildsPath;
                if (Input.GetKeyDown(KeyCode.Alpha3)) Mode = Modes.EditingControlPoints;
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
                OnSelectedPointChanged = (_) => { };

                SceneLoader.LoadScene(0);
            }
        }

        #region Save and load
        /// <summary>
        /// Загружает пути.
        /// </summary>
        /// <param name="paths">Пути, при <c>null</c> загружаеться пустой путь.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public void Load(params Paths.Path[] paths)
        {
            CardEditorPath createPath(int num)
            {
                var path = new GameObject($"Path {num}").AddComponent<CardEditorPath>();
                path.PointPrefab = PointPrefab; // Ставим префаб точки

                return path;
            }

            // Создаём новый путь
            var path = createPath(0);
            path.CreatePoint(0, new Vector2(0, 0)); // Создаём первую точку

            Paths.Add(path);
            SelectedPath = Paths[0];
        }
        #endregion
    }
}
