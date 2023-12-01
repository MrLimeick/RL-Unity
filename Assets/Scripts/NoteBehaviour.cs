using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace RL.Game
{
    public enum NoteType
    {
        Up,
        Down,
        Left,
        Right/*,
    W,
    A,
    S,
    D*/
    }
    public class NoteBehaviour : MonoBehaviour
    {
        public float Time;
        public NoteType NoteType;

        public TMPro.TMP_Text Text;
        public TMPro.VertexGradient SPTextGradient;
        public SpriteRenderer Graphic;

        public Sprite Default, SP;

        public bool IsSPNote;

        private float SpawnTime;
        public void Awake()
        {
            SpawnTime = UnityEngine.Time.time;
        }
        public void Start()
        {
            void setType((string symbol, float offset) type)
            {
                Text.text = type.symbol;
                Text.margin = new Vector4(type.offset, type.offset, type.offset, type.offset);
            }

            setType(NoteType switch
            {
                NoteType.Up => ("^", 0),
                NoteType.Down => ("V", 0.2f),
                NoteType.Left => ("<", 0f),
                NoteType.Right => (">", 0f),
                _ => throw new System.NotImplementedException()
            });

            if (IsSPNote)
            {
                Graphic.sprite = SP;
                Text.colorGradient = SPTextGradient;
                Text.enableVertexGradient = true;
                StartCoroutine(SPAnim());
            }
            else
            {
                Graphic.sprite = Default;
                Text.enableVertexGradient = false;
            }
            Wait();
        }
        private float GradientAngle;
        IEnumerator SPAnim()
        {
            while (true)
            {
                GradientAngle += 0.005f;
                Color color1 = Color.HSVToRGB((GradientAngle + 0.07f) % 1, 1, 1);
                Color color2 = Color.HSVToRGB((GradientAngle + 0.28f) % 1, 1, 1);
                Color color3 = Color.HSVToRGB((GradientAngle + 0.56f) % 1, 1, 1);
                Color color4 = Color.HSVToRGB((GradientAngle + 0.84f) % 1, 1, 1);

                var gradient = new TMPro.VertexGradient
                {
                    bottomRight = color1,
                    bottomLeft = color2,
                    topLeft = color3,
                    topRight = color4
                };

                Text.colorGradient = gradient;

                transform.rotation = Quaternion.Euler(0, 0, GradientAngle % 1 * 360);
                Text.transform.rotation = Quaternion.Euler(0, 0, 0);

                yield return null;
            }

        }
        async void Wait()
        {
            float localTime;
            while ((localTime = UnityEngine.Time.time - SpawnTime - 1) < 1)
            {
                if (Input.anyKeyDown)
                {
                    var Keys = Input.inputString;
                    foreach (char Key in Keys.ToCharArray())
                    {
                        if (ScoreCounter.Instance != null) ScoreCounter.Instance.Add(300);
                        print(Key switch
                        {
                            'w' or 'u' => "Up ^",
                            'a' or 'h' => "Left <",
                            's' or 'j' => "Down v",
                            'd' or 'k' => "Right >",
                            _ => "other key clicked: " + Key
                        });
                    }
                }
                await Task.Yield();
            }

            Wait();
            //Destroy(this.gameObject);
        }

        public void Tap()
        {
            float localTime = UnityEngine.Time.time - SpawnTime - 1;

            if (localTime < -1f) return;

            const float MISS = 1f, BAD = 0.75f, GOOD = 0.5f, PERFECT = 0.25f;

            ScoreCounter.Instance.Add(localTime switch
            {
                (>= -MISS    and < -BAD    ) or (>= BAD               ) => 0,
                (>= -BAD     and < -GOOD   ) or (>= GOOD    and < BAD ) => 50,
                (>= -GOOD    and < -PERFECT) or (>= PERFECT and < GOOD) => 100,
                (>= -PERFECT and <  PERFECT)                            => 300,
               _ => throw new System.NotImplementedException()
            });
        }
    }
}