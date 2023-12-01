using System;
using System.Collections.Generic;
using System.Linq;
using RL.Paths;
using RL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RL.CardEditor
{
    public partial class PathMaker : MonoBehaviour
    {
        private static PathMaker Instance { get; set; } = null;

        [SerializeField] private Camera m_Camera;
        public static Camera Camera => Instance.m_Camera;

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
        [SerializeField] private CardEditorLine LinePrefab;

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

        private static bool InViewport;

        public void Awake()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            Instance = this;

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
                Mode = Modes.EditingCurvesPoints;
            });

            Load(null);

            #region Viewport events

            Viewport.OnLeftClick.AddListener((arg) =>
            {
                if (Mode == Modes.BuildsPath)
                    SelectedPath.CreatePoint(SelectedPath[^1].Time + Preview.Lenght, Preview.EndPoint);
            }); // При нажатии левой клавишой мышки по Editor Viewport создавать новый точку и линию
            Viewport.OnStay.AddListener((arg) =>
            {
                PreviewLine.transform.position = Preview.Position;
                PreviewLine.transform.localScale = new Vector3(Preview.Lenght, Preview.Thickness, 0);
                PreviewLine.transform.rotation = Quaternion.Euler(0, 0, Preview.Angle);
                PreviewPoint.transform.position = Preview.EndPoint;
            }); // Если находится в EditorViewport
            Viewport.OnEnter.AddListener((arg) =>
            {
                InViewport = true;
                IsPreviewEnabled = true;

                MainCameraController.CanMove = true;
                MainCameraController.CanScroll = true;
            }); // Если вошёл в viewport
            Viewport.OnExit.AddListener((arg) =>
            {
                InViewport = false;
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

        public void Update()
        {
            #region Hotkeys
            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) Save();
            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) Load();
            if (Input.GetKeyDown(KeyCode.Delete) && SelectedPoint != null) SelectedPath.RemovePoint(SelectedPoint);
            if (Input.GetKeyDown(KeyCode.R) && SelectedPath.Count > 1) SelectedPath.RemovePoint(SelectedPath[^1]);

            if (Input.GetKeyDown(KeyCode.Alpha1)) Mode = Modes.EditingPath;
            if (Input.GetKeyDown(KeyCode.Alpha2)) Mode = Modes.BuildsPath;
            if (Input.GetKeyDown(KeyCode.Alpha3)) Mode = Modes.EditingCurvesPoints;
            #endregion
        }

        public async void Close()
        {
            var ans = await Dialog.ShowDialog("Выйти?", "Вы действительно хотите выйти?");
            if (ans)
            {
                OnModeChanged = (_) => { };
                OnSelectedPointChanged = (_) => { };

                SceneManager.LoadScene("MainMenu2");
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
                path.LinePrefab = LinePrefab; // Ставим префаб линии
                path.PointPrefab = PointPrefab; // Ставим префаб точки

                return path;
            }

            if (paths == null || paths.Length == 0)
            {
                // Создаём новый путь
                var path = createPath(0);
                path.CreatePoint(0, new Vector2(0, 0)); // Создаём первую точку

                Paths.Add(path);
                SelectedPath = Paths[0];
            }
            else
            {
                try
                {
                    int num = 0;
                    foreach (var savedPath in paths)
                    {
                        var path = createPath(num++);
                        path.LoadPath(savedPath);

                        path.Sync();
                        Paths.Add(path);
                    }
                    SelectedPath = Paths[^1];
                }
                catch (Exception e)
                {
                    throw new Exception("Не удалось загрузить путь", e);
                }
            }
        }

        /// <summary>
        /// Создать сохранение для всех путей
        /// </summary>
        /// <returns></returns>
        public Paths.Path[] Save()
        {
            try
            {
                List<Paths.Path> paths = new();
                foreach (var path in Paths)
                {
                    paths.Add(path.GetPath());
                }
                return paths.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось сохранить пути", e);
            }
        }
        #endregion
    }
}
