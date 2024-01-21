using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class TimeHpOverlay : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private RectTransform hpFill; // TODO: hpFill
    [SerializeField] private TMP_Text timeLeft;
    [SerializeField] private TMP_Text timeCurrent;
    [SerializeField] private TMP_Text timeLength;

    public void Init()
    {
        TimeSpan time = TimeSpan.FromSeconds(Game.Length);
        timeLength.text = time.ToString(@"mm\:ss");
        timeLeft.text = time.ToString(@"\-mm\:ss");
    }

	void Update()
	{
        if (!Game.IsStarted) return;
        
        timeCurrent.text = TimeSpan.FromSeconds(Game.Time).ToString(@"mm\:ss");
        timeLeft.text = TimeSpan.FromSeconds(Game.Length - Game.Time).ToString(@"\-mm\:ss");
    }
}

