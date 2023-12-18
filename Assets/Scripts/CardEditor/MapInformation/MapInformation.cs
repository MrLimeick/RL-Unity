using UnityEngine;
using System.Collections;

namespace RL.CardEditor.MapInformation
{
    public class MapInformation : MonoBehaviour
	{
		public Audio Audio;
		public Background Background;
		public CardData CardData;
		public TemporaryData TemporaryData;

		// Use this for initialization
		void Start()
		{
			Audio.Select.onClick.AddListener(async () =>
			{
                var file = await FileBrowser.Open();
				Debug.Log("Аудио: " + file.FullName);
				CardEditor.SetAudio(file);
			});
			Audio.Title.onEndEdit.AddListener((str) => CardEditor.Card.Title = str);
            Audio.Artist.onEndEdit.AddListener((str) => CardEditor.Card.Artits = str);

			Background.Select.onClick.AddListener(async () =>
			{
				var file = await FileBrowser.Open();
				CardEditor.SetBackground(file);
			});

            CardData.Creator.onEndEdit.AddListener((str) => CardEditor.Card.Creators = str);
            CardData.Name.onEndEdit.AddListener((str) => CardEditor.Card.Name = str);

            TemporaryData.BPM.onEndEdit.AddListener((str) => CardEditor.Card.BPM = float.Parse(str));
        }
	}
}