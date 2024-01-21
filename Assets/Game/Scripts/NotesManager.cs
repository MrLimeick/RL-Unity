using System.Collections;
using System.Collections.Generic;
using RL.Paths;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.CanvasScaler;

public class NotesManager : MonoBehaviour
{
    public IReadOnlyList<NoteSettings> notesSettings;
    public IReadOnlyPath path;

    public Note notePrefab;
    private readonly List<Note> notes = new();

    private int spawnIndex = 0;
    public int InputIndex { get; private set; } = 0;

    public float noteShowTime = 1;
    public float marginOfMiss = 0.1f;

    private AudioSource SFXSource;
    public AudioClip KickSound;

    private void Start()
    {
        SFXSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        Spawn();
        CheckInput();
    }

    /// <summary>
    /// Может ли нота создаться?
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    bool CanSpawn(out NoteSettings note)
    {
        note = notesSettings[spawnIndex];
        return note.time - noteShowTime <= Game.Time;
    }

    /// <summary>
    /// Создаёт ноты.
    /// </summary>
    void Spawn()
    {
        while (spawnIndex < notesSettings.Count && CanSpawn(out NoteSettings noteSettings))
        {
            var note = Instantiate(notePrefab);
            note.transform.localPosition = path.GetPosition(noteSettings.time) ?? new(0, 0);
            note.direction = noteSettings.direction;
            note.time = noteSettings.time;
            notes.Add(note);
            spawnIndex++;
        }
    }

    public IReadOnlyDictionary<NoteDirection, (KeyCode first, KeyCode second)> KeyMap = new Dictionary<NoteDirection, (KeyCode first, KeyCode second)>
    {
        { NoteDirection.Up,    (KeyCode.W, KeyCode.UpArrow   ) },
        { NoteDirection.Left,  (KeyCode.A, KeyCode.LeftArrow ) },
        { NoteDirection.Down,  (KeyCode.S, KeyCode.DownArrow ) },
        { NoteDirection.Right, (KeyCode.D, KeyCode.RightArrow) },
    };

    /// <summary>
    /// Активен ли авто мод?
    /// </summary>
    public static bool AutoMode { get; set; } = true;

    public bool IsEnd { get; private set; } = false;

    public UnityEvent OnEnd = new();

    public UnityEvent OnHit = new();
    public UnityEvent OnMiss = new();

    /// <summary>
    /// Проверяет упарвление.
    /// </summary>
    void CheckInput()
    {
        if (InputIndex >= notes.Count)
        {
            if(!IsEnd)
            {
                OnEnd.Invoke();
                IsEnd = true;
            }

            return;
        }

        float tapTime = notes[InputIndex].time;
        float time = Game.Time - tapTime;
        NoteDirection direction = notes[InputIndex].direction;

        if (AutoMode)
        {
            if (0 <= time) Hit();
        }
        else
        {
            if (GetKeyDown(direction))
            {
                if (Mathf.Abs(time) < marginOfMiss) Hit();
                else Debug.Log("Oops!");
            }

            if (marginOfMiss <= time) Miss();
        }
    }

    public void Hit()
    {
        Debug.Log("Hit!");

        SFXSource.PlayOneShot(KickSound);
        notes[InputIndex].Hit();
        Game.Player.Hit();

        OnHit.Invoke();

        InputIndex++;
    }

    public void Miss()
    {
        Debug.Log("Miss!");

        notes[InputIndex].Miss();

        OnMiss.Invoke();
        
        InputIndex++;
    }

    // UNDONE: Notes Input

    bool GetKeyDown(NoteDirection direction)
    {
        return Input.anyKeyDown;

        var (first, second) = KeyMap[direction];
        return Input.GetKeyDown(first) || Input.GetKeyDown(second);
    }
}
