using System;
using RL.Game;
using RL.Math;
using RL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace RL.CardEditor
{
    public partial class PathMaker : MonoBehaviour
    {
        private static PathMaker _instance;
        public static SelectedPointsList SelectedPoints { get; } = new();
        private static PathsList _paths;
        public static GridSettings Grids { get; private set; }
        public static Vector2 LastPointPosition => _instance._preview.Start;
        public static bool GlobalGrid;

        public void SetGlobalGrid(bool value)
            => GlobalGrid = value;
        
        public static Vector2 MousePos
        {
            get
            {
                var mousePos = (Vector2)_instance.cam.ScreenToWorldPoint(Input.mousePosition);
                var center = GlobalGrid ? Vector2.zero : LastPointPosition;

                switch (BuildMode)
                {
                    case BuildModes.ByGrid:
                        Vector2 grid = Grids.Resolution;
                        return Maths.Round(mousePos - center, grid) + center;
                    case BuildModes.LockedHeight:
                    default:
                        Maths.GetLineTransform(center, mousePos, out _, out var dis, out var angle);
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
            public Button editingPath;
            public Button buildPath;
            public Button editingCurves;

            public void Init()
            {
                editingPath.OnClick.AddListener(() => Mode = Modes.EditingPath);
                buildPath.OnClick.AddListener(() => Mode = Modes.BuildsPath);
                editingCurves.OnClick.AddListener(() => Mode = Modes.EditingControlPoints);
            }
        }
        [SerializeField] private ToolsButtons toolsButtons;

        #region Paths

        [FormerlySerializedAs("PathsDropdown")]
        [Header("Paths")]
        [SerializeField] private TMP_Dropdown pathsDropdown;

        public void ChangePathButton(int index)
            => _paths.Index = index;

        public void CreatePathButton()
        {
            _paths.Create();

            pathsDropdown.Hide();
        }

        public async void DeletePathButton()
        {
            bool ans = await Dialog.ShowQuestion("Remove path?", "You realy want delete this path?");
            if (!ans) return;

            _paths.DeleteCurrent();
            pathsDropdown.Hide();
        }

        #endregion

        [FormerlySerializedAs("PointPrefab")]
        [Header("Prefabs")]
        [SerializeField] private CardEditorPoint pointPrefab;

        [FormerlySerializedAs("Point")]
        [Header("Preview")] // TODO: Preview и… Preview? Переделать предпросмотр карты.
        [SerializeField] private Transform point;
        [FormerlySerializedAs("Line")] [SerializeField] private Transform line;

        private LinePreview _preview;

        [FormerlySerializedAs("GridCamera")]
        [Header("Grid")]
        [SerializeField] private GridCamera gridCamera;

        #region Modes
        public static event UnityAction<Modes> OnModeChanged;

        private bool InPreview => Player.Moved;

        private Modes m_Mode = Modes.None;
        public static Modes Mode
        {
            get => _instance.m_Mode;
            set
            {
                if (value == _instance.m_Mode) return;

                _instance.m_Mode = value;
                _instance._preview.Enabled = true;
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
            get => _instance.m_BuildMode;
            set
            {
                if (value == _instance.m_BuildMode) return;

                _instance.m_BuildMode = value;
                _instance.gridCamera.GridType = value switch
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

        [FormerlySerializedAs("_viewport")]
        [Header("Other")]
        [SerializeField] EditorViewport viewport;
        [FormerlySerializedAs("_mainCameraController")] [SerializeField] CameraController mainCameraController;

        [SerializeField] private Camera cam;

        [SerializeField] Player player;
        public static Player Player => _instance.player;
        private EventSystem _eventSystem;

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
            get => mainCameraController.CanMove;
            set => mainCameraController.CanMove = value;
        }

        private bool CamCanScroll
        {
            get => mainCameraController.CanScroll;
            set => mainCameraController.CanScroll = value;
        }

        public void Awake()
        {
            this.SetInstance(ref _instance, true, false, false);

            _eventSystem = EventSystem.current;

            toolsButtons.Init();

            Grids = new(gridCamera);
            _preview = gameObject.AddComponent<LinePreview>();
            _preview.Line = line;
            _preview.Point = point;
            _preview.Enabled = true;

            _paths = new(pointPrefab, pathsDropdown);
            _paths.CurrentChanged.AddListener((path) =>
            {
                if (Player.Moved) Player.Stop();
            });

            #region Viewport events

            viewport.OnLeftClick.AddListener((_) =>
            {
                if (Mode == Modes.BuildsPath)
                    _paths.Current.Add(_paths.Current[^1].Time + _preview.Lenght, _preview.End);
            }); // При нажатии левой клавишой мышки по Editor Viewport создавать новый точку и линию
            viewport.OnStay.AddListener(_
                => _preview.Update()); // Если находится в EditorViewport
            viewport.OnEnter.AddListener(_ =>
            {
                _preview.Enabled = true;
                CamCanMove = true;
                CamCanScroll = true;
            }); // Если вошёл в viewport
            viewport.OnExit.AddListener(_ =>
            {
                _preview.Enabled = false;
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
            if (_eventSystem.currentSelectedGameObject != null) return;

            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) Save();
            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) Load();

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                SelectedPoints.RemoveAll();

            if (Input.GetKeyDown(KeyCode.R) && _paths.Current.Count > 1)
                _paths.Current.Remove(_paths.Current[^1]);

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
                    GUIUtility.systemCopyBuffer = _paths.Current.Save();
                    Debug.Log("Path has been saved");
                }

                if(Input.GetKeyDown(KeyCode.L))
                {
                    string buffer = GUIUtility.systemCopyBuffer;
                    if (buffer.StartsWith("RLC"))
                    {
                        _paths.Load(buffer);
                        Debug.Log("Path has been loaded");
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) // Enter to preview mode
            {
                if (InPreview) Player.Stop();
                else Player.Move(_paths.Current);
            }
            #endregion
        }

        public async void Close()
        {
            var ans = await Dialog.ShowQuestion("Quit?", "Are you sure you want to exit the card editor?");
            if (!ans) return;
           
            OnModeChanged = _ => { };
            SceneLoader.LoadScene(0);
        }
    }
}
