using UnityEngine;
using System.Collections;
using System;

public class Menu : MonoBehaviour
{
	private static Menu Instance;
	private long CardEditorEnterTime;

    private void Awake()
    {
        if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
    }

	[SerializeField] private TMPro.TMP_Text NotifyText;

	public static bool IsShow { get; private set; } = false;

	public void SetIsShow(bool value) => IsShow = value;

    // Use this for initialization
    void Start()
	{
		CardEditorEnterTime = DateTime.Now.Ticks;
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

