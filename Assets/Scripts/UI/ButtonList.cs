using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace RL.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ButtonList : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_Template;
        public RectTransform Template
        {
            get => m_Template;
            set => m_Template = value;
        }

        [SerializeField]
        private UnityEngine.UI.Button m_ButtonUp;
        [SerializeField]
        private UnityEngine.UI.Button m_ButtonDown;

        public UnityEngine.UI.Button ButtonUp
        {
            get => m_ButtonUp;
            set
            {
                if (value == null) return;
                m_ButtonUp = value;
                ButtonUpRT = value.GetComponent<RectTransform>();
            }
        }
        public UnityEngine.UI.Button ButtonDown
        {
            get => m_ButtonDown;
            set
            {
                if (value == null) return;
                m_ButtonDown = value;
                ButtonDownRT = value.GetComponent<RectTransform>();
            }
        }

        [SerializeField]
        private UnityEngine.UI.Button m_ButtonForCopy;

        [SerializeField]
        private TMPro.TMP_Text m_ButtonForCopyText;


        [SerializeField]
        private List<string> m_ButtonTexts = new List<string>();
        public List<string> ButtonTexts
        {
            get => m_ButtonTexts;
            set 
            {
                m_ButtonTexts = value;
                UpdateList();
            }
        }

        [System.NonSerialized]
        public List<UnityEngine.UI.Button> Buttons = new List<UnityEngine.UI.Button>();

        private RectTransform ButtonUpRT;
        private RectTransform ButtonDownRT;

        public RectTransform Content;

        public UnityEvent<int> OnClick = new UnityEvent<int>();


        public void Awake()
        {
            if (ButtonUp != null)
                ButtonUpRT = ButtonUp.GetComponent<RectTransform>();
            if (ButtonDown != null)
                ButtonDownRT = ButtonDown.GetComponent<RectTransform>();

            UpdateList();
            OnClick.AddListener((arg) => print($"Button {arg} is clicked"));
        }
        public void UpdateList()
        {
            foreach (UnityEngine.UI.Button button in Buttons)
            {
                Destroy(button.gameObject);
            }
            if (ButtonTexts.Count == 0) return;
            if (Template != null &&
                ButtonUp != null &&
                ButtonDown != null &&
                Content != null &&
                m_ButtonForCopy != null
                && m_ButtonForCopyText != null)
            {
                Debug.Log("Все условия выполнены!");
                float indent = ButtonDownRT.anchoredPosition.y - ButtonUpRT.anchoredPosition.y;
                for (int i = 0; i < m_ButtonTexts.Count; i++)
                {

                    UnityEngine.UI.Button NewButton = Instantiate(m_ButtonForCopy, Content).GetComponent<UnityEngine.UI.Button>();
                    RectTransform NewButtonRT = NewButton.GetComponent<RectTransform>();
                    TMPro.TMP_Text NewButtonText = NewButton.transform.Find(m_ButtonForCopyText.gameObject.name).GetComponent<TMPro.TMP_Text>();

                    Buttons.Add(NewButton);

                    NewButtonRT.anchoredPosition = new Vector2(0, ButtonUpRT.anchoredPosition.y + indent * i);
                    NewButtonText.text = m_ButtonTexts[i];

                    int num = new int();
                    num = i;
                    NewButton.onClick.AddListener(() => OnClick.Invoke(num));
                }
                Content.sizeDelta = new Vector2(0, 5 + -indent * m_ButtonTexts.Count);
            }
        }
    }
}