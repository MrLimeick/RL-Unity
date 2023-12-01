using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using UnityEngine;
using UnityEngine.Events;

using UnityEngine.UI;
using InputField = TMPro.TMP_InputField;

namespace RL.CardEditor.AboutMap
{
    [System.Serializable]
    public class Background
    {
        public UnityEvent<Sprite> OnBackgroundSelect = new();

        public Image BackgroundPreview;
        public FileInfo File;
        public Sprite Sprite;
    }
    [System.Serializable]
    public class Music
    {
        public UnityEvent<string> OnMusicNameWrite = new();
        public UnityEvent<string> OnArtistNameWrite = new();
        public UnityEvent<AudioClip> OnMusicSelect = new();

        public Button SelectMusicFile;
        public InputField MusicName;
        public InputField ArtistName;
        public FileInfo File;
        public AudioClip Clip;
    }
    public class AboutMapController : Page
    {
        public UnityEvent<string> OnCreatorNameWrite = new();
        public UnityEvent<string> OnDifficultyWrite = new();
        public InputField CreatorName;
        public InputField DifficultyName;
        public Background Background;
        public Music Music;
        public RectTransform UI;

        public void Awake()
        {
        }
        //public async void LoadBackground() => await LoadBackgroundAsync();
        //public async Task LoadBackgroundAsync()
        //{
        //    var BackGroundFile = FileBrowser.OpenFile("������ ������ ��� ��� ������");
        //    if (BackGroundFile != null)
        //    {
        //        var sprite = await RLIO.GetSpriteFromPathAsync(BackGroundFile.FullName);
        //        Background.BackgroundPreview.sprite = sprite;
        //        Background.Sprite = sprite;
        //        Background.File = BackGroundFile;
        //        Background.OnBackgroundSelect.Invoke(sprite);
        //    }
        //}
        //public async void LoadMusic() => await LoadMusicAsync();
        //public async Task LoadMusicAsync()
        //{
        //    var AudioFile = FileBrowser.OpenFile("������ ���� ��� ������", new ExtensionFilter("mp3 music".ToString(), "mp3"));
        //    if (AudioFile != null)
        //    {
        //        Music.File = AudioFile;
        //        Music.Clip = await RLIO.GetAudioClipFromPathAsync(AudioFile.FullName);
        //        Music.OnMusicSelect.Invoke(Music.Clip);
        //    }
        //}
        public enum Property
        {
            MusicName, ArtistName, CreatorName, DifficultyName
        }
        public void Write(Property property, string value)
        {
            switch(property)
            {
                case Property.MusicName:
                    Music.OnMusicNameWrite.Invoke(value);
                    break;
                case Property.ArtistName:
                    Music.OnArtistNameWrite.Invoke(value);
                    break;
                case Property.CreatorName:
                    OnCreatorNameWrite.Invoke(value);
                    break;
                case Property.DifficultyName:
                    OnDifficultyWrite.Invoke(value);
                    break;
            }
        }
        /*protected override async Task OpenAnimation(CancellationToken ct)
        {
            List<Task> tasks = new()
            {
                UI.AnchorMaxAnimAsync(new Vector2(1, 1), 0.5f,ct),
                UI.AnchorMinAnimAsync(new Vector2(0, 0), 0.5f,ct)
            };
            await Task.WhenAll(tasks.ToArray());
        }
        protected override async Task CloseAnimation(CancellationToken ct)
        {
            List<Task> tasks = new()
            {
                UI.AnchorMaxAnimAsync(new Vector2(0.5f, 1), 0.5f,ct),
                UI.AnchorMinAnimAsync(new Vector2(0.5f, 0), 0.5f,ct)
            };
            await Task.WhenAll(tasks.ToArray());
        }*/
    }

}