using System;
using System.Collections;
using RL.CardEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RL.CardEditor
{

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

        #region Show/Hide
        private Coroutine _coroutine;
        public static bool IsShow { get; set; } = false;

        public void Show()
        {
            IsShow = true;

            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Anim(_animationSpeed, SetStep, false));
        }

        public void Hide()
        {
            IsShow = false;

            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Anim(_animationSpeed, SetStep, true));
        }

        [Header("Animation")]
        [SerializeField] RectTransform _menu;
        [SerializeField] RectTransform _menuContent;
        [SerializeField] CanvasGroup _canvasGroup;

        [SerializeField] private float _animationSpeed = 0.3f;

        float OutExpo(float x) => x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x);
        float InExpo(float x) => x == 0 ? 0 : MathF.Pow(2, 10 * x - 10);

        void SetStep(float t)
        {
            _menu.pivot = new(t, .5f);
            _menuContent.anchoredPosition = new(-150 * t, 0);
            _canvasGroup.alpha = 1 - t;
        }

        IEnumerator Anim(float duration, Action<float> action, bool invert = false) // Анимации как отдельный класс?
        {
            _canvasGroup.blocksRaycasts = !invert;

            float start = Time.unscaledTime;

            float localTime;
            while ((localTime = (Time.unscaledTime - start) / duration) < 1)
            {
                float t = invert ? OutExpo(localTime) : (1 - OutExpo(localTime));
                action(t);

                yield return 0;
            }

            action(invert ? 1 : 0);
        }

        #endregion

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

        public void SetPage(int index)
        {
            CardEditor.Page = (Pages)index;
            Hide();
        }

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
}