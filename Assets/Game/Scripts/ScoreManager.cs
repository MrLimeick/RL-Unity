using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OsuBeatmaps;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    //[SerializeField] private TMP_Text MinAccuracyText;
    //[SerializeField] private TMP_Text MaxAccuracyText;
    //[SerializeField] private TMP_Text CurrentAccuracyText;
    [SerializeField] private TMP_Text AccuracyText;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text ComboText;

    private NotesManager notesManager;
    public NotesManager NotesManager
    {
        get => notesManager;
        set
        {
            if (notesManager != null)
            {
                notesManager.OnHit.RemoveListener(Hit);
                notesManager.OnMiss.RemoveListener(Miss);
            }

            notesManager = value;

            notesManager.OnHit.AddListener(Hit);
            notesManager.OnMiss.AddListener(Miss);

            UpdateText();
        }
    }

    public IReadOnlyList<NoteSettings> Notes => notesManager.notesSettings;

    private int Count => Notes.Count;

    void Hit()
    {
        Hitted++;
        Combo++;

        Score += 100 * Mathf.Pow(Combo, 0.5f);

        UpdateText();
    }

    void Miss()
    {
        Missed++;
        Combo = 0;

        UpdateText();
    }

    public int Hitted { get; private set; } = 0;
    public int Missed { get; private set; } = 0;
    public int Combo { get; private set; } = 0;
    public float Score { get; private set; } = 0;
    public float MinAccuracy => Hitted / (float)Count * 100;
    public float MaxAccuracy => (Count - Missed) / (float)Count * 100;
    public float Accuracy => (Hitted != 0) ? ((float)Hitted / (Hitted + Missed) * 100) : 0;

    void UpdateText()
    {
        //MinAccuracyText.text = (hitted / Count * 100).ToString("00:00%");
        //MaxAccuracyText.text = ((Count - missed) / Count * 100).ToString("00:00%");
        //CurrentAccuracyText.text = (hitted / (hitted + missed) * 100).ToString("00:00%");

        AccuracyText.text =
            $"Accuracy:\n" +
            $"Min: {MinAccuracy:00:00%}\n" +
            $"Cur: {Accuracy:00:00%}\n" +
            $"Max: {MaxAccuracy:00:00%}";
        ScoreText.text = Score.ToString("00000000");
        ComboText.text = Combo.ToString("0x");
    }
}
