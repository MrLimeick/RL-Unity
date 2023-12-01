using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RL.Game;
using RL.CardEditor.Menu;
using RL.UI;
using UnityEngine;
using AboutMapController = RL.CardEditor.AboutMap.AboutMapController;

namespace RL.CardEditor
{
    [Serializable]
    public class Pages
    {
        public AboutMapController AboutMap;
        public PathMaker PathMaker;
        public RectTransform Menu;
    }
    [Serializable]
    public class Buttons
    {
        public Button AboutMap;
        public Button PathMaker;
        public Button ObjectsBoard;
        public Button NoteCreation;
        public Button Preview;
        public SaveButton Save;
        public Button ToMainMenu;
    }
    public enum EditorPages
    {
        AboutMap, PathMaker, ObjectsBoard, NoteCreation, Preview
    }
    [AddComponentMenu("RL/GameEditor/Controller")]
    public class EditorController : RLScript
    {
        /// <summary>
        /// Все страницы редактора
        /// </summary>
        public Pages Pages = new();
        /// <summary>
        /// Кнопки для открытия страниц
        /// </summary>
        public Buttons Buttons = new();

        /// <summary>
        /// Открытая на данный момент страница
        /// </summary>
        public static EditorPages OpennedPage = EditorPages.AboutMap;

        public ImagePropotions Background;
        public Player Player;
        public static Card Map;

        public AudioClip AudioClip;
        public Sprite BackgroundImage;

        public CancellationTokenSource PlayerCancellationTokenSource;
        public void Awake()
        {
            /*Pages.AboutMap = new("About map");
            Pages.pathMaker = new("Path maker");
            Pages.AboutMap.Open();
            Buttons.AboutMap.onClick.AddListener(Pages.AboutMap.Open);
            Buttons.PathMaker.onClick.AddListener(Pages.PathMaker.Open);*/
            /*
            Buttons.PathMaker.onClick.AddListener(Pages.PathMaker.Open);
            Buttons.ObjectsBoard.onClick.AddListener(() => Message.Instance.ShowNotifyMessage(Localizations.Localization.GetString("Soon")));
            Buttons.NoteCreation.onClick.AddListener(() => Message.Instance.ShowNotifyMessage(Localizations.Localization.GetString("Soon")));
            Buttons.Preview.onClick.AddListener(async () => 
            {
                //Message.Instance.ShowNotifyMessage(Localizations.Localization.GetString("Soon"));
                if (PlayerCancellationTokenSource != null) PlayerCancellationTokenSource.Cancel();
                Player.gameObject.SetActive(true);
                PlayerCancellationTokenSource = new();
                await Player.Move(Controller.Instance.SelectedPath.GetPath(), PlayerCancellationTokenSource.Token);
                PlayerCancellationTokenSource.Cancel();
                Player.gameObject.SetActive(false);
            });
            Buttons.Save.onClick.AddListener(async () => 
            {
                try
                {
                    bool result = await Save();
                    if (result) Buttons.Save.PlaySaveAnimation();
                    else Buttons.Save.PlayFailedAnimation();
                }
                catch(Exception e)
                {
                    Buttons.Save.PlayFailedAnimation();
                    throw e;
                }
            });
            Buttons.ToMainMenu.onClick.AddListener(async () => 
            {
                bool otv = await Message.Instance.ShowQuestionMessage("You want to save first?");
                if (otv)
                {
                    bool result = await Save();
                    if(!result)
                    {
                        Buttons.Save.PlayFailedAnimation();
                        return;
                    }
                }
                await SceneLoader.Instance.LoadScene("MainMenu");
            });

            Pages.aboutMap.Controller.Background.OnBackgroundSelect.AddListener((arg) => 
            {
                BackgroundImage = arg;
                Background.Sprite = arg;
                map.BackgroundFilePath = Pages.aboutMap.Controller.Background.File.Name;
            });
            Pages.aboutMap.Controller.Music.OnMusicNameWrite.AddListener((arg) => map.Name = arg);
            Pages.aboutMap.Controller.Music.OnArtistNameWrite.AddListener((arg) => map.Artits = arg);
            Pages.aboutMap.Controller.OnCreatorNameWrite.AddListener((arg) => map.Creators = arg);
            Pages.aboutMap.Controller.OnDifficultyWrite.AddListener((arg) => map.Difficulty = arg);
            Pages.aboutMap.Controller.Music.OnMusicSelect.AddListener((arg) =>
            {
                AudioClip = Pages.aboutMap.Controller.Music.Clip;
                map.AudioFilePath = Pages.aboutMap.Controller.Music.File.FullName;
            });

            if(map != null)
            {
                await LoadAsync();
            }
            else
            {
                map = new();
                //Pages.PathMaker.Load(null);
            }*/
        }

        public async Task<bool> Save()
        {
            try
            {
                static async Task<bool> check(string field, string message)
                {
                    string value = (string)Map.GetType().GetField(field).GetValue(Map);

                    if (value == "" || value == null)
                    {
                        await Message.Instance.ShowNotifyMessageAsync(message);
                        return false;
                    }

                    return true;
                }

                if (!await check(nameof(Map.Name), "Write the name of the music first!")) return false;
                if (!await check(nameof(Map.Artits), "Write the music artists first!")) return false;
                if (!await check(nameof(Map.Difficulty), "First write the difficulty of the map!")) return false;
                if (!await check(nameof(Map.Creators), "Write to the creators of the map first!")) return false;

                var MapDirectory = Directory.CreateDirectory(CreatedMapsDirectory.FullName + "/" + Map.Name);
                File.Create(MapDirectory.FullName + "/" + Map.Difficulty + ".RLM").Close();
                if (Map.AudioFile is not null && Map.AudioFile.Exists)
                {
                    File.Copy(Map.AudioFile.FullName, MapDirectory.FullName + "/" + Map.AudioFile.Name,true);
                }
                else
                {
                    bool otv = await Message.Instance.ShowQuestionMessage("Do you want to keep the map without music?");
                    if (!otv) return false;
                }
                if (Map.BackgroundFile is not null && Map.BackgroundFile.Exists)
                {
                    File.Copy(Map.BackgroundFile.FullName, MapDirectory.FullName + "/" + Map.BackgroundFile.Name, true);
                }
                else
                {
                    bool otv = await Message.Instance.ShowQuestionMessage("Do you want to save the map without a background?");
                    if (!otv) return false;
                }
                Map.FileInfo = new(CreatedMapsDirectory.FullName + "/" + Map.Name + "/" + Map.Difficulty + ".RLM");
                    Map.AudioFilePath = Map.AudioFile.Name;
                    Map.BackgroundFilePath = Map.BackgroundFile.Name;
                File.WriteAllText(MapDirectory.FullName + "/" + Map.Difficulty + ".RLM", JsonUtility.ToJson(Map, true));
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось сохранить карту", e);
            }
        }

        /*public async Task LoadAsync()
        {
            var AboutMap = Pages.AboutMap;
            if(map.BackgroundFile != null)
            AboutMap.Background.BackgroundPreview.sprite = await IO.RLIO.GetSpriteFromPathAsync(map.BackgroundFile.FullName);
            AboutMap.Music.MusicName.text = map.Name;
            AboutMap.Music.ArtistName.text = map.Artits;
            AboutMap.DifficultyName.text = map.Difficulty;
            AboutMap.CreatorName.text = map.Creators;

            Pages.PathMaker.Load(map.PlayerPath);
        }*/
    }
}
        /*

        public TMPro.TMP_InputField GameSpeedInputField;
        
        public SceneLoader SceneLoader;

        public GameObject Player;

        /// <summary>
        /// Контроллер главной камеры
        /// </summary>
        public CameraController MainCameraController;

        // статические переменные
        #region Static varibles

        private static EditorPages p_Page = EditorPages.AboutMap;

        
        /// <summary>
        /// Открытая страница редактора на данный момент
        /// </summary>
        public static EditorPages Page
        {
            get => p_Page;
            set
            {
                p_Page = value;
            }
        }

        #endregion
        async void Awake()
        {

                #region PeviewMode
                GameSpeedInputField.onEndEdit.AddListener((arg) =>
                {
                    if (float.TryParse(arg, out float result))
                    {
                        GameSpeed = Mathf.Clamp(result / 100, 0, 50);
                        GameSpeedInputField.text = Mathf.Clamp(result, 0, 50000).ToString();
                    }
                });
                #endregion
                #region Загрузка пути из сохранения
                if (!IsNewMap)
                {
                    await Load(JsonUtility.FromJson<RLM>(File.ReadAllText(CreatedMapsDirectory.FullName + "/" + EditorController.map.Name + "/" + Difficulty + ".RLM")));
                    //Task load = Load();
                    //load.Wait();
                }
                else
                {
                    await Load(map);
                }
                #endregion

                //GridCam.GridResolution = GridResolution;
            }
            catch (Exception e)
            {
#if !UNITY_EDITOR
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
#endif
                throw new Exception("Не удалось загрузить редактор",e);
            }
        }
        void Update()
        {
            #region Keys

            //if (Input.GetKeyDown(KeyCode.Tab)) PreviewToggle();
            //if (Input.GetKeyDown(KeyCode.Space)) PreviewPauseToggle();
            //
            //if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeMode(0);
            //if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeMode(1);
            #endregion
        }

        #region Режим предпросмотра

        #region Переменные

        // Преременные используемые при завершении предпросмотра
        #region "Старые" переменные
        /// <summary>
        /// Старое приближение камеры
        /// </summary>
        private float OldCamSize = 5;
        /// <summary>
        /// Старая позиция камеры
        /// </summary>
        private Vector2 OldCamPosition;
        /// <summary>
        /// Было ли скрыто окно "Инструменты"
        /// </summary>
        private bool OldTabsIsHide = false;
        /// <summary>
        /// Было ли скрыто окно "Настройки точки"
        /// </summary>
        private bool OldPointSettingsIsHide = false;
        /// <summary>
        /// Было ли скрыто окно "Иерархия"
        /// </summary>
        private bool OldToolsIsHide = false;
        /// <summary>
        /// Старый режим редактора
        /// </summary>
        private EditorPages OldMode;
        private PathMakerModes OldPathMakerMode;
#endregion

        private static float p_GameSpeed = 1f;
        private static bool p_IsPaused = false;

        /// <summary>
        /// Скорость игры в режиме предпросмотра
        /// </summary>
        public float GameSpeed
        {
            get => p_GameSpeed;
            set
            {
                p_GameSpeed = value;
                if (!IsPaused && p_Page == EditorPages.Preview) Time.timeScale = GameSpeed;
            }
        }

        /// <summary>
        /// Поставлен ли на паузу предпросмотр
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return p_IsPaused;
            }
            set
            {
                if (p_Page == EditorPages.Preview)
                {
                    if (value) Time.timeScale = 0f;
                    else Time.timeScale = GameSpeed;
                    p_IsPaused = value;
                }
            }
        }
        /// <summary>
        /// Вход в режим предпросмотра
        /// </summary>
        public async void ToPreview()
        {
            if (Pages.PathMaker.SelectedPath.Count < 2)
            {
                Debug.LogError("Запустить режим предпросмотра сейчас не получится - в одном из главных путей путей менее двух точек");
                return;
            }
            Debug.Log("Запускаю режим предпросмотра...");

            if (Clip != null)
            {
                var AS = GetComponent<AudioSource>();
                AS.clip = Clip;
                AS.time = 0;
                AS.Play();
            }


            OldCamSize = MainCameraController.CamSize;
            OldCamPosition = MainCameraController.transform.position;

            MainCameraController.CamSize = 5;
            MainCameraController.CanScroll = false;
            MainCameraController.CanMove = false;
            MainCameraController.Fix(Player);

            PreviewModeWindow.Show();

            Pages.PathMaker.PreviewLine.SetActive(false);
            Pages.PathMaker.PreviewPoint.SetActive(false);

            OldMode = Page;
            Page = EditorPages.Preview;
            OldPathMakerMode = Pages.PathMaker.Mode;
            Pages.PathMaker.Mode = PathMakerModes.None;

            Player.SetActive(true);
            if (PlayerMoveCor != null) StopCoroutine(PlayerMoveCor);
            Time.timeScale = GameSpeed;
            IsPaused = false;
            await PlayerMove();
        }
        /// <summary>
        /// Выход из режима предпросмотра
        /// </summary>
        public void FromPreview()
        {
            if (PlayerMoveCor != null) StopCoroutine(PlayerMoveCor);
            Time.timeScale = 1f;

            if (Clip != null)
            {
                var AS = GetComponent<AudioSource>();
                AS.time = 0;
                AS.Stop();
            }

            MainCameraController.UnFix();
            MainCameraController.CamSize = OldCamSize;
            MainCameraController.CanScroll = true;
            MainCameraController.CanMove = true;
            MainCameraController.EasingCamMove(OldCamPosition, true, 0.5f);

            GameEditor.Page.OpennedPage.additionalWindows.CloseAll();

            PreviewModeWindow.Hide();

            Page = OldMode;
            Pages.PathMaker.Mode = OldPathMakerMode;

            //if (EditorViewport.isStay == true && Mode == EditorPages.PathMaker)
            //{
            //    PathMaker.PreviewLine.SetActive(true);
            //    PathMaker.PreviewPoint.SetActive(true);
            //}

            Player.SetActive(false);

            IsPaused = false;
        }

        /// <summary>
        /// Окно "Предпросмотр"
        /// </summary>
        public UI.UI PreviewModeWindow;
        /// <summary>
        /// Корутина с перемещением игрока
        /// </summary>
        private Coroutine PlayerMoveCor;

#endregion

        public void PreviewPauseToggle() => IsPaused = !IsPaused;
        public async Task PlayerMove()
        {
            if (Pages.PathMaker.SelectedPath.Count < 2) return; // Проверка на наличее более двух точек в пути
            Time.timeScale = GameSpeed;
            float OldTime = Time.time;

            while (Time.time - OldTime < Pages.PathMaker.SelectedPath[^1].Time)
            {
                await Task.Yield();
                Player.transform.position = Pages.PathMaker.SelectedPath.GetPosition(Time.time - OldTime);
                Camera.main.transform.position = Pages.PathMaker.SelectedPath.GetPosition(Time.time - OldTime);
            }

            FromPreview();
        }

#endregion
        #region Сохранение и загрузка карты

        /// <summary>
        /// Имя изменяемой карты
        /// </summary>
        public static string Difficulty = "Easy";

        /// <summary>
        /// Создал ли игрок новую карту
        /// </summary>
        public static bool IsNewMap = true;
        
#endregion

        /// <summary>
        /// Загрузить карту из сохранения
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Load(RLM map)
        {
            try
            {
                if(!IsNewMap)
                    Pages.PathMaker.Load(map.PlayerPath);// Загрузка пути

                try
                {
                    if (map.BackgroundFile != null && map.BackgroundFile.Exists)
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(File.ReadAllBytes(map.BackgroundFile.FullName));
                        BackgroundImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    }
                    else
                    {
                        Debug.LogError("Не был найден задний фон");
                    }
                }
                catch (Exception e)
                {
                    BackgroundImage.GetComponent<Image>().sprite = null;
                    Debug.LogError("Не удалось загрузить сохранённый задний фон, ошибка: " + e.Message);
                } // Загрузка заднего фона

                try
                {
                    if (map.AudioFile != null && map.AudioFile.Exists)
                    {
                        Clip = await IO.RLIO.GetAudioClipFromPathAsync(map.AudioFile.FullName);
                        Debug.Log("Удалось загрузить сохранённый трек");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Не удалось загрузить сохранённый трек",e);
                } // Загрузка трека
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось загрузить сохранение, " + e.Message, e);
            }
        }
    }
}
*/