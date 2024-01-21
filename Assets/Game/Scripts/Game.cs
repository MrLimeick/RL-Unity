using System;
using System.Collections.Generic;
using System.IO;
using OsuBeatmaps;
using RL;
using RL.Game;
using RL.UI;
using UnityEngine;
using UnityEngine.UI;
using CardPath = RL.Paths.Path;
using CardPathPoint = RL.Paths.PathPoint;

public class Game : MonoBehaviour
{
    private static Game instance;

    private Beatmap beatmap;
    public static string PathToBeatmap = string.Empty;
#if UNITY_EDITOR
    [SerializeField] private string pathToTestBeatmap;
#endif

    [Space]
    [SerializeField] private AudioSource audioSource;

    public static float Time => instance.audioSource.time;
    public static float Length => instance.audioSource.clip.length;

    private AudioClip clip;
    private CardPath path;

    [Space]
    [SerializeField] private PathRenderer pathRendererPrefab;
    [SerializeField] private NotesManager notesManagerPrefab;

    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;
    private NotesManager notesManager;

    [Space]
    [SerializeField] private Player player;
    public static Player Player => instance.player;

    [Space]
    [SerializeField] private PauseMenu PauseMenu;

    private bool canStart = false;
    public static bool IsStarted { get; private set; } = false;

    [SerializeField] private TimeHpOverlay timeHpOverlay;

    async void Start()
    {
        if (instance != null) Destroy(instance.gameObject);
        instance = this;

        if (!GetBeatmap(out string mapDir)) return;
        clip = await MusicManager.LoadAudio(Path.Combine(mapDir, beatmap.General.AudioFilename));
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.time = 0;

        SetBackground(Path.Combine(mapDir, beatmap.BackgroundFilename));

        timeHpOverlay.Init();

        path = new()
        {
            new CardPathPoint(0, new(0, 0)),
            new CardPathPoint(clip.length, new(clip.length * 10f, 0))
        };
        var pathRenderer = Instantiate(pathRendererPrefab);
        pathRenderer.Path = path;

        var notes = HitObjectsToNotes(beatmap);
        scoreManager.NotesManager = SpawnNotesManager(notes);

        canStart = true;
    }

    [Header("Background")]
    [SerializeField] private RawImage background;
    [SerializeField] private AspectRatioFitter backgroundAspectRatioFitter;
    [SerializeField] private RectTransform backgroundRectTransform;
    private Texture2D backgroundTexture;

    private void SetBackground(string path)
    {
        backgroundTexture = new Texture2D(2, 2);
        backgroundTexture.LoadImage(File.ReadAllBytes(path));
        backgroundTexture.Apply();

        background.texture = backgroundTexture;
        backgroundAspectRatioFitter.aspectRatio = backgroundTexture.width / (float)backgroundTexture.height;
        backgroundRectTransform.sizeDelta = new(clip.length * 10f, 0);
    }

    private void Update()
    {
        if (!IsStarted)
        {
            if (canStart && Input.anyKeyDown)
            {
                IsStarted = true;
                StartGame();
            }

            return;
        }

        float t = Time / clip.length;

        backgroundRectTransform.pivot = new(t, .5f);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;

            if (IsPaused) Pause();
            else Play();
        }
    }

    public bool IsPaused { get; private set; } = false;

    public void Pause()
    {
        IsPaused = true;

        audioSource.Pause();
        PauseMenu.Show();
    }

    public void Play()
    {
        IsPaused = false;

        audioSource.Play();
        PauseMenu.Hide();
    }

    public void Exit()
    {
        SceneLoader.LoadScene(0);
    }

    void StartGame()
    {
        
        audioSource.Play();

        player.Move(path, () => Time);
    }

    public bool GetBeatmap(out string mapDir)
    {
        try
        {
#if UNITY_EDITOR
            string mapFile = string.IsNullOrEmpty(PathToBeatmap) ? pathToTestBeatmap : PathToBeatmap;
#else
            string mapFile = PathToBeatmap;
#endif

            beatmap = Beatmap.FromFile(mapFile);

            var mode = beatmap.General.Mode;
            if (mode == Mode.CTB)
            {
                throw new Exception("osu!catch бит-карты не поддерживаются.");
            }
            else if(mode == Mode.Mania && beatmap.Difficulty.CircleSize > 4)
            {
                throw new Exception("Поддерживаются только osu!mania бит-карты с количеством столбцов от 1 до 4");
            }
            else
            {
                mapDir = new FileInfo(mapFile).DirectoryName;
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Load map failed… " + e.Message);

            mapDir = string.Empty;
            return false;
        }
    }

    protected virtual NoteSettings[] HitObjectsToNotes(Beatmap beatmap)
    {
        IReadOnlyList<IHitObject> hitObjects = beatmap.HitObjects;
        List<NoteSettings> notesSettings = new();

        int columnCount = beatmap.General.Mode == Mode.Mania ? Mathf.FloorToInt(beatmap.Difficulty.CircleSize) : 0;

        foreach (var hitObject in hitObjects)
        {
            var tapTime = hitObject.Time / 1000f;

            if (!hitObject.Type.HasFlag(HitObjectType.Circle))
            {
                Debug.LogWarning("Обнаружена хит объект отличный от кружка, он не будет добавлен. Тип объекта:" + hitObject.Type);
                continue;
            }

            if (notesSettings.Count > 0 && notesSettings[^1].time == tapTime)
            {
                Debug.LogWarning("Обнаружены нота которая на одном времени с другой, она не будет добавленна.");
                continue;
            }

            var hitSound = hitObject.HitSound;
            NoteDirection direction = beatmap.General.Mode switch
            {
                Mode.Mania => (NoteDirection)Mathf.FloorToInt(hitObject.Position.x * columnCount / 512),
                _ => (hitSound.HasFlag(HitSound.Whistle) || hitSound.HasFlag(HitSound.Clap)) ?
                (hitSound.HasFlag(HitSound.Finish) ? NoteDirection.Up : NoteDirection.Right) :
                (hitSound.HasFlag(HitSound.Finish) ? NoteDirection.Left : NoteDirection.Down)
            };

            var noteSettings = new NoteSettings()
            {
                direction = direction,
                time = tapTime
            };

            notesSettings.Add(noteSettings);
        }

        return notesSettings.ToArray();
    }

    protected virtual NotesManager SpawnNotesManager(NoteSettings[] notesSettings)
    {
        notesManager = Instantiate(notesManagerPrefab);
        notesManager.notesSettings = notesSettings;
        notesManager.path = path;

        return notesManager;
    }
}
