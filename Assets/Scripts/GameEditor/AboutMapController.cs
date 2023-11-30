using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using UnityEngine;
using UnityEngine.Events;

using UnityEngine.UI;
using InputField = TMPro.TMP_InputField;

namespace RL.GameEditor.AboutMap
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
        public void WriteMusicName(string name) => Music.OnMusicNameWrite.Invoke(name);
        public void WriteArtistName(string name) => Music.OnArtistNameWrite.Invoke(name);
        public void WriteCreatorName(string name) => OnCreatorNameWrite.Invoke(name);
        public void WriteDifficultyName(string name) => OnDifficultyWrite.Invoke(name);
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