using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

using RL.Paths;

using System.IO;


namespace RL.Game
{
    internal class GameController : MonoBehaviour
    {
        public static Card map;
        public PathBuilder PathBuilder;
        public Player Player;

        public Image Clock, BlackScreen;
        public RectTransform TimePoint;
        public TMPro.TMP_Text Countdown;

        public CancellationTokenSource cts = new();
        public void Awake()
        {
            if(map == null)
            {
                List<Line> lines = new()
                {
                    new Line(
                            new Point(0, new Vector2(0, 0)),
                            new Point(1, new Vector2(1, 0))
                            ),
                        new Line(
                            new Point(1, new Vector2(1, 0)),
                            new Point(2, new Vector2(1, 1))
                            ),
                        new Line(
                            new Point(2, new Vector2(1, 1)),
                            new Point(3, new Vector2(0, 1))
                            ),
                        new Line(
                            new Point(3, new Vector2(0, 1)),
                            new Point(4, new Vector2(0, 0))
                            )
                };

                map = new Card()
                {
                    Name = "Test",
                    Artits = "Artist?",
                    Difficulty = "Easy",
                    Creators = "MrLimeick",
                    PlayerPath = 
                    new(lines.ToArray()),
                };
                DirectoryInfo DI = Directory.CreateDirectory(RLScript.CreatedMapsDirectory.FullName + "/" + map.Name);
                string fileName = DI.FullName + "/" + map.Difficulty + ".RLM";
                File.WriteAllText(fileName, JsonUtility.ToJson(map,true));
            }
            PathBuilder.Load(map.PlayerPath);

            
        }
        public void Start()
        {
            PlayerStart();
        }
        public async void PlayerStart()
        {
            /*await BlackScreen.ColorAnimAsync(new Color(0, 0, 0, 0), 1);*/

            float OldTime = Time.time;
            float time = 3;
            float localTime;
            while((localTime = (Time.time - OldTime) / time) < 1)
            {
                await Task.Yield();
                Clock.fillAmount = localTime;
                Countdown.text = (time - Time.time - OldTime).ToString("0");
                Player.transform.localScale = Vector3.Lerp(new Vector3(0, 0), new Vector3(1,1),localTime);
            }
            Clock.gameObject.SetActive(false);
            Countdown.gameObject.SetActive(false);

            /*var PlayerTask = Player.Move(map.PlayerPath, cts.Token);*/

            time = map.PlayerPath.GetLinesArray()[^1].B.time;
            OldTime = Time.time;
            while ((localTime = (Time.time - OldTime) / time) < 1)
            {
                await Task.Yield();
                TimePoint.anchorMax = Vector2.Lerp(new Vector2(0, 0), new Vector2(1, 0), localTime);
                TimePoint.anchorMin = Vector2.Lerp(new Vector2(0, 0), new Vector2(1, 0), localTime);
            }

            /*await BlackScreen.ColorAnimAsync(new Color(0, 0, 0, 1), 1);*/

            await SceneLoader.LoadSceneAsync("MainMenu");
        }

        public void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
