using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RL.Game
{
    public class ScoreCounter : MonoBehaviour
    {
        public static ScoreCounter Instance { get; private set; }
        [SerializeField] private TMPro.TMP_Text Text;
        public float Score { get; private set; }
        private float m_ShowScore;
        /// <summary>
        /// Очки показываемые на данный момент игроку
        /// </summary>
        private float ShowScore 
        {
            get => m_ShowScore;
            set
            {
                if (Text != null) Text.text = value.ToString("0");
                m_ShowScore = value;
            }
        }
        void Start()
        {
            if (Instance != null) Destroy(Instance);
            Instance = this;
        }
        /*void Update()
        {

        }*/
        private Coroutine ScoreAnim;
        public void Add(float Score)
        {
            this.Score += Score;
            if (ScoreAnim != null) StopCoroutine(ScoreAnim);
            ScoreAnim = StartCoroutine(ScoreAnimation());
        }

        IEnumerator ScoreAnimation()
        {
            float OldTime = Time.unscaledTime;
            float OldScore = ShowScore;

            float localTime;
            while ((localTime = (Time.unscaledTime - OldTime) / 1) < 1)
            {
                yield return new WaitForEndOfFrame();
                ShowScore = Mathf.Lerp(OldScore, Score, localTime);
            }
            ShowScore = Score;
        }
    }
}