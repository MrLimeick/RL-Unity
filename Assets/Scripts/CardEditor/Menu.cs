using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using RL.CardEditor;

public class Menu : MonoBehaviour
{
    private const string USE_TRACKPAD_KEY = "CardEditor_UseTrackpad";

    protected static Menu Instance;
	private long CardEditorEnterTime;

    private void Awake()
    {
        Instance = this;
		//if (!this.SetInstance(ref Instance)) return;
    }

	[SerializeField] private TMPro.TMP_Text NotifyText;

	[SerializeField] private CameraController CameraController;
	[SerializeField] private Toggle UseTrackpadToggle;

    public static bool IsShow { get; private set; } = false;

	public void SetIsShow(bool value) => IsShow = value;

    public static bool UseTrackpad
    {
        get => Instance.CameraController.IsTrackpad;
        set
        {
            if (UseTrackpad == value) return;

            Instance.UseTrackpadToggle.SetIsOnWithoutNotify(value);
            Instance.CameraController.IsTrackpad = value;
            PlayerPrefs.SetInt(USE_TRACKPAD_KEY, value ? 1 : 0);
        }
    }

    public void SetUseTrackpad(bool value) => UseTrackpad = value;

    void Start()
	{
		CardEditorEnterTime = DateTime.Now.Ticks;

		if (PlayerPrefs.HasKey(USE_TRACKPAD_KEY))
            UseTrackpad = PlayerPrefs.GetInt(USE_TRACKPAD_KEY) == 1;
    }

	// Update is called once per frame
	void Update()
	{
		if (!IsShow) return;

        TimeSpan time = new(CardEditorEnterTime - DateTime.Now.Ticks);

		NotifyText.text =
			$"Now <b>{DateTime.Now:HH:mm:ss}</b>\n" +
            $"You've been in the editor for <b>{time:hh\\:mm\\:ss}</b>\n" +
			$"Don't forget to take a break!";
	}
}

