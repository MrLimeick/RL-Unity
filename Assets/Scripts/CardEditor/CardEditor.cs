using System.Collections;
using System.Collections.Generic;
using System.IO;
using RL;
using UnityEngine;
using UnityEngine.UI;

namespace RL.CardEditor
{
    public enum Pages
    {
        MapInformation = 0,
        PathMaker = 1
    }

    public class CardEditor : MonoBehaviour
    {
        private static CardEditor Instance;

        public static Card Card;
        public static FileInfo AudioFile;

        private static DirectoryInfo CardDirectory = null;

        [SerializeField] private RawImage BackgroundImage;
        [SerializeField] private AspectRatioFitter BackgroundAspectRatio;

        [SerializeField] private AudioSource AudioSource;

        public static void SetBackground(FileInfo file)
        {
            Card.BackgroundFile = file;

            Texture2D texture = new(0, 0);
            texture.LoadImage(File.ReadAllBytes(file.FullName));

            Instance.BackgroundImage.texture = texture;
            Instance.BackgroundAspectRatio.aspectRatio = texture.width / (float)texture.height;
        }

        public static async void SetAudio(FileInfo file)
        {
            Card.AudioFile = file;

            AudioClip clip = await MusicManager.LoadAudio(file.FullName);
            Instance.AudioSource.clip = clip;
        }

        #region Pages
        public Canvas PathMakerUI;
        public Canvas MapInfoUI;

        private Pages _page = Pages.MapInformation;
        public static Pages Page
        {
            get => Instance._page;
            set
            {
                Instance._page = value;

                switch (value)
                {
                    case Pages.PathMaker:
                        Instance.MapInfoUI.gameObject.SetActive(false);
                        Instance.PathMakerUI.gameObject.SetActive(true);
                        break;
                    case Pages.MapInformation:
                        Instance.MapInfoUI.gameObject.SetActive(true);
                        Instance.PathMakerUI.gameObject.SetActive(false);
                        break;
                }
            }
        }

        
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            Instance = this;

            Card ??= new();
            Page = Pages.MapInformation;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private static string CardFile => CardDirectory.FullName + $"/{Card.Name}.rlc";

        public static void Save()
        {
            CardDirectory ??= Directory.CreateDirectory($"Cards/{Card}");

            string data = JsonUtility.ToJson(Card, true);
            Debug.Log(data);
            File.WriteAllText(CardFile, data);
        }
    }
}