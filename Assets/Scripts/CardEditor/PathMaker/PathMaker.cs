using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RL.Game;
using RL.Paths;
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

        [SerializeField] private Camera m_Camera;
        public static Camera Camera => Instance.m_Camera;

        [SerializeField] private Player Player;
        [SerializeField] private EventSystem EventSystem;

        public static Vector2 MousePos => Instance.m_Camera.ScreenToWorldPoint(Input.mousePosition);

        [Header("Tools Buttons")]
        public Button EditingPathButton;
        public Button BuildPathByGridButton;
        public Button BuildPathWithLockedLenghtButton;
        public Button EditingCurvesButton;

        public static event UnityAction<Modes> OnModeChanged;

        public class SelectedPointsList
        {
            private readonly List<CardEditorPoint> Points = new();

            public CardEditorPoint this[int index] => Points[index];

            public void Select(CardEditorPoint point)
            {
                Points.Add(point);
                point.OnPointSelect();
            }

            public void UnSelect(CardEditorPoint point)
            {
                if (Points.Remove(point)) point.OnPointUnSelect();
            }

            public void RemoveAll()
            {
                for (int i = 0; i < Points.Count; i++) SelectedPath.RemovePoint(Points[i]);

                Points.Clear();
            }

            public void UnSelectAll()
            {
                Points.ForEach((p) => p.OnPointUnSelect());
                Points.Clear();
            }

            public void MoveAll(Vector2 vector)
            {
                Points.ForEach((p) => p.Position += vector);
            }
        }

        private SelectedPointsList m_SelectedPoints = null;
        public static SelectedPointsList SelectedPoints = new();

        #region Paths

        private static CardEditorPath m_SelectedPath = null;
        public static CardEditorPath SelectedPath
        {
            get => m_SelectedPath;
            set
            {
                if(SelectedPath != null) SelectedPath.gameObject.SetActive(false);

                m_SelectedPath = value;
                SelectedPath.gameObject.SetActive(true);

                Instance.PathsDropdown.SetValueWithoutNotify(Instance.Paths.IndexOf(value));
                Player.Path = SelectedPath;
            }
        }

        [SerializeField] private TMP_Dropdown PathsDropdown;

        public void ChangePath(int index) => SelectedPath = Paths[index];

        private int lastCreatedPathIndex = 0;

        public void CreatePathButton()
        {
            CreatePath();

            PathsDropdown.Hide();
        }

        public CardEditorPath CreatePath()
        {
            var pathName = $"Path {lastCreatedPathIndex++}";

            var path = new GameObject(pathName).AddComponent<CardEditorPath>();
            path.PointPrefab = PointPrefab; // Ставим префаб точки
            path.CreatePoint(0, new(0, 0)); // Создаём первую точку
            path.gameObject.SetActive(false);

            Paths.Add(path);
            PathsDropdown.options.Add(new(pathName));

            Debug.Log($"Создан путь {pathName}");
            return path;
        }

        public async void DeletePathButton()
        {
            bool ans = await Dialog.ShowQuestion("Remove path?", "You realy want delete this path?");
            if (ans) DeletePath();

            PathsDropdown.Hide();
        }
        public void DeletePath()
        {
            Debug.Log($"Удалён путь {SelectedPath.name}");

            Paths.Remove(SelectedPath);
            Destroy(SelectedPath.gameObject);

            SelectedPath = Paths[^1];

            UpdatePathsDropdownNames();
            PathsDropdown.SetValueWithoutNotify(Paths.Count - 1);
        }

        private void UpdatePathsDropdownNames()
        {
            PathsDropdown.ClearOptions();

            TMP_Dropdown.OptionDataList list = new();
            for (int i = 0; i < Paths.Count; i++)
                list.options.Add(new(Paths[i].name));

            PathsDropdown.AddOptions(list.options);
            PathsDropdown.RefreshShownValue();
        }

        #endregion

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

            

            #region Tools buttons
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
            #endregion

            PathsDropdown.ClearOptions();
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

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) SelectedPoints.RemoveAll();
            if (Input.GetKeyDown(KeyCode.R) && SelectedPath.Count > 1) SelectedPath.RemovePoint(SelectedPath[^1]);

            if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) Mode = Modes.EditingPath;
                if (Input.GetKeyDown(KeyCode.Alpha2)) Mode = Modes.BuildsPath;
                if (Input.GetKeyDown(KeyCode.Alpha3)) Mode = Modes.EditingControlPoints;

                if (Input.GetKeyDown(KeyCode.S))
                {
                    GUIUtility.systemCopyBuffer = SelectedPath.Save();
                    Debug.Log("Path has been saved");
                }

                if(Input.GetKeyDown(KeyCode.L))
                {
                    string buffer = GUIUtility.systemCopyBuffer;
                    if (buffer.StartsWith("RLC"))
                    {
                        var path = CreatePath();
                        path.name = "Loaded path";
                        path.Load(buffer);
                        UpdatePathsDropdownNames();
                        SelectedPath = Paths[^1];

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

        #region Save and load
        /// <summary>
        /// Загружает пути.
        /// </summary>
        /// <param name="paths">Пути, при <c>null</c> загружаеться пустой путь.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public void Load(params Paths.Path[] paths)
        {
            CreatePath();
            SelectedPath = Paths[0];
        }
        #endregion
    }
}
