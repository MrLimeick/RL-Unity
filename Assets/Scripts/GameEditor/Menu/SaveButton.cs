using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Events;
using System.Threading;

namespace RL.GameEditor.Menu
{
    public class SaveButton : MonoBehaviour
    {
        public TMPro.TMP_Text UpText;
        public TMPro.TMP_Text DownText;

        public UnityEvent onClick = new();

        public Button Button;
        CancellationTokenSource cts = new();
#if UNITY_EDITOR
        public void OnValidate()
        {
            Button = GetComponent<Button>();
        }
#endif
        public void Awake()
        {
            Button.onClick.AddListener(onClick.Invoke);
        }

        public void PlaySaveAnimation() => Animation("Done!");
        public void PlayFailedAnimation() => Animation("Failed...");
        private async void Animation(string text)
        {
            /*cts.Cancel();*/

            RectTransform UpTextRT = UpText.GetComponent<RectTransform>();
            RectTransform DownTextRT = DownText.GetComponent<RectTransform>();

            UpText.text = "Save map";
            DownText.text = text;

            UpTextRT.anchoredPosition = new Vector2(0, 0);
            DownTextRT.anchoredPosition = new Vector2(0, -40);

            /*float time = 0.25f;*/

            cts = new();
            var ct = cts.Token;
            List<Task> tasks = new()
        {
            /*UpTextRT.PositionAnimAsync(new Vector2(0, 40f), time,ct),
            DownTextRT.PositionAnimAsync(new Vector2(0, 0), time,ct)*/
        };
            await Task.WhenAll(tasks.ToArray());

            await Task.Delay(500);

            UpText.text = text;
            DownText.text = "Save map";

            UpTextRT.anchoredPosition = new Vector2(0, 0);
            DownTextRT.anchoredPosition = new Vector2(0, -40);

            /*UpTextRT.PositionAnim(new Vector2(0, 40f), time);
            await DownTextRT.PositionAnimAsync(new Vector2(0, 0), time);*/

            UpText.text = "Save map";
            DownText.text = text;

            UpTextRT.anchoredPosition = new Vector2(0, 0);
            DownTextRT.anchoredPosition = new Vector2(0, -40);
        }
    }
}