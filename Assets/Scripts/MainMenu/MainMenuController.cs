using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Threading.Tasks;
using Button = RL.UI.Button;


namespace RL.MainMenu
{

    public class MainMenuController : RLScript
    {
        public static MainMenuController Instance { get; private set; }
        #region Classes
        [Serializable]
        public class Buttons
        {
            public Button Play;
            public Button Account;
            public Button GameEditor;
            public Button Online;
            public Button Settings;
            public Button Quit;
        }
        [Serializable]
        public class Windows
        {
            public MainMenuWindow MainMenu;
            public MapList.MapListWindow Play;
            public SettingsWindow Settings;
            public GameEditorWindow GameEditor;
        }
        #endregion

        public RectTransform GameNameTextParent;

        public TMPro.TMP_Text GameNameText;
        public TMPro.TMP_Text VersionText;

        public Buttons buttons;
        public Windows windows;

        public static bool NowStarted = true;
        void Start()
        {
            Instance = this;
            #region Buttons

            //buttons.Play.OnClick += () =>
            //{
            //    //Message.ShowNotifyMessage(Localization.GetString("Soon"));
            //    windows.Play.OpenCloseToggle();
            //};

            //buttons.GameEditor.OnClick += async () => await SceneLoader.LoadSceneAsync("GameEditor");

            //buttons.Account.OnClick += () =>
            //{
            //    Message.Instance.ShowNotifyMessage("Soon");
            //};

            //buttons.Online.OnClick += () =>
            //{
            //    Message.Instance.ShowNotifyMessage("Soon");
            //};

            //buttons.Settings.OnClick += () => windows.Settings.OpenCloseToggle();

            //buttons.Quit.OnClick += async () =>
            //{
            //    bool ans = await Message.Instance.ShowQuestionMessage("Quit?");
            //    if (ans) Application.Quit();
            //};

            #endregion

            print("��������� ��������� � " + AppDomain.CurrentDomain.BaseDirectory);
            print("Env " + Environment.CurrentDirectory);
            if (NowStarted)
            {
                /*StartAnimation();*/
                NowStarted = false;
            }
        }
        public void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.E))
            {
                 StartAnimation();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                QuitAnimation();
            }*/
        }

        /*public async void StartAnimation()
        {
            GameNameTextParent.sizeDelta = new(0, 200);
            GameNameTextParent.anchorMin = new(0, 0.5f);
            GameNameTextParent.anchorMax = new(1, 0.5f);
            GameNameTextParent.anchoredPosition = new(0, 0);

            GameNameText.characterSpacing = 200;
            GameNameText.color = new(1, 1, 1, 0);

            windows.MainMenu.GetComponent<RectTransform>().anchoredPosition = new(100,0);

            await Task.Delay(500);

            GameNameTextParent.SizeAnim(new Vector2(0, 150), 1f);

            float OldTime = Time.unscaledTime;

            float localTime;
            while ((localTime = (Time.unscaledTime - OldTime) / 1f) < 1)
            {
                await Task.Yield();
                GameNameText.characterSpacing = Mathf.Lerp(200,0,Easing.Get(EasingType.QuartOut, localTime));
                GameNameText.color = Color.Lerp(new Color(1,1,1,0),new Color(1,1,1,1), Easing.Get(EasingType.QuartOut, localTime));
            }
            GameNameText.characterSpacing = 0;
            GameNameText.color =  new Color(1,1,1,1);

            await Task.Delay(500);

            List<Task> tasks = new()
            {
                GameNameTextParent.AnchorMinAnimAsync(new Vector2(0,1),1f),
                GameNameTextParent.AnchorMaxAnimAsync(new Vector2(1,1),1f),
                GameNameTextParent.PositionAnimAsync(new Vector2(-70, -100), 1f),
                GameNameTextParent.SizeAnimAsync(new Vector2(-140, 150), 1f),
                windows.MainMenu.GetComponent<RectTransform>().PositionAnimAsync(new(-20,0),1f),
            };
            await Task.WhenAll(tasks);
        }
        public async void QuitAnimation()
        {
            List<Task> tasks = new()
            {
                GameNameTextParent.AnchorMinAnimAsync(new Vector2(0, 0.5f), 1f),
                GameNameTextParent.AnchorMaxAnimAsync(new Vector2(1, 0.5f), 1f),
                GameNameTextParent.PositionAnimAsync(new Vector2(0, 0), 1f),
                GameNameTextParent.SizeAnimAsync(new Vector2(0, 200), 1f),
                windows.MainMenu.GetComponent<RectTransform>().PositionAnimAsync(new(100,0),1f),
            };
            await Task.WhenAll(tasks);

            float OldTime = Time.unscaledTime;

            float localTime;
            while ((localTime = (Time.unscaledTime - OldTime) / 1f) < 1)
            {
                await Task.Yield();
                GameNameText.characterSpacing = Mathf.Lerp(0,200, Easing.Get(EasingType.QuartOut, localTime));
                GameNameText.color = Color.Lerp(new Color(1,1,1,1),new Color(1,1,1,0), Easing.Get(EasingType.QuartOut, localTime));
            }
            GameNameText.characterSpacing = 200;
            GameNameText.color = new Color(1,1,1,0);

        }
    }*/
    }
}