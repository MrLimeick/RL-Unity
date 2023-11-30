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

            switch (NoteType)
            {
                case NoteType.Up:
                    Text.text = "^";
                    Text.margin = new Vector4(0, 0, 0, 0);
                    break;
                case NoteType.Down:
                    Text.text = "V";
                    Text.margin = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
                    break;
                case NoteType.Left:
                    Text.text = "<";
                    Text.margin = new Vector4(0f, 0f, 0f, 0f);
                    break;
                case NoteType.Right:
                    Text.text = ">";
                    Text.margin = new Vector4(0f, 0f, 0f, 0f);
                    break;
                    /*case NoteType.W:
                        Text.text = "W";
                        Text.margin = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
                        break;
                    case NoteType.A:
                        Text.text = "A";
                        Text.margin = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
                        break;
                    case NoteType.S:
                        Text.text = "S";
                        Text.margin = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
                        break;
                    case NoteType.D:
                        Text.text = "D";
                        Text.margin = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
                        break;*/
            }
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

                var gradient = new TMPro.VertexGradient();

                gradient.bottomRight = color1;
                gradient.bottomLeft = color2;
                gradient.topLeft = color3;
                gradient.topRight = color4;

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
                        switch (Key)
                        {
                            case 'w':
                                print("Up ^");
                                break;
                            case 'a':
                                print("Left <");
                                break;
                            case 's':
                                print("Down v");
                                break;
                            case 'd':
                                print("Right >");
                                break;
                            case 'u':
                                print("Up ^");
                                break;
                            case 'h':
                                print("Left <");
                                break;
                            case 'j':
                                print("Down v");
                                break;
                            case 'k':
                                print("Right >");
                                break;
                            default:
                                print("other key clicked: " + Key);
                                break;
                        }
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

            if (localTime < -1f)
            {
                return;
            }
            else if (localTime >= -1f && localTime < -0.75f)
            {
                ScoreCounter.Instance.Add(0);
            }
            else if(localTime >= -0.75f && localTime < -0.5f)
            {
                ScoreCounter.Instance.Add(50);
            }
            else if (localTime >= -0.5f && localTime < -0.25f)
            {
                ScoreCounter.Instance.Add(100);
            }
            else if (localTime >= -0.25f && localTime < 0.25f)
            {
                ScoreCounter.Instance.Add(300);
            }
            else if (localTime >= 0.25f && localTime < 0.5f)
            {
                ScoreCounter.Instance.Add(100);
            }
            else if (localTime >= 0.5f && localTime < 0.75f)
            {
                ScoreCounter.Instance.Add(50);
            }
            else if (localTime >= 0.75f)
            {
                ScoreCounter.Instance.Add(0);
            }
        }
    }
}