using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RL.MainMenu
{
    public class GameEditorWindow : MainMenuWindow
    {
        /*public MakeNewMapWindow MakeNewMapWindow;
        public RectTransform MapsGameObject;

        public Button CreateNewMap;
        public UI.ButtonList MapList;

        public Image PreviewImage;
        public TMPro.TMP_Text MapName;

        public SceneLoader SceneLoader;

        public List<RLM> Maps = new();

        public void Start()
        {
            CreateNewMap.onClick.AddListener(async () =>
            {
                RLM map = await MakeNewMapWindow.MakeNewMap();
                if (map == null) return;
                EditorController.map = map;
                EditorController.IsNewMap = true;
                await SceneLoader.LoadScene("GameEditor");
            });

            if (!CreatedMapsDirectory.Exists) Directory.CreateDirectory(CreatedMapsDirectory.FullName);
            foreach (DirectoryInfo DI in CreatedMapsDirectory.GetDirectories())
                foreach (FileInfo FI in DI.GetFiles())
                {
                    if (FI.Extension == ".RLM")
                    {
                        Maps.Add(JsonUtility.FromJson<RLM>(File.ReadAllText(FI.FullName)));
                    }
                }
            List<string> Names = new();
            foreach (RLM map in Maps)
            {
                Names.Add(map.Name);
            }
            MapList.ButtonTexts = Names;
            MapList.UpdateList();
            MapList.OnClick.AddListener(async (arg) =>
            {
                EditorController.map = Maps[arg];

                EditorController.IsNewMap = false;
                await SceneLoader.LoadScene("GameEditor");
            });

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                print("Обновить лист с созданными картами");

            }
        }*/
    }
}
