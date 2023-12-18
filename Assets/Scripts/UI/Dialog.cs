using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(RectTransform))]
public class Dialog : MonoBehaviour
{
    enum DialogType
    {
        Question,
        Notify
    }

    [Header("Texts")]
    [SerializeField] TMP_Text m_Header;
    [SerializeField] TMP_Text m_Message;

    [Header("Question buttons")]
    [SerializeField] GameObject m_QuestionButtons;
    [SerializeField] Button m_Yes;
    [SerializeField] Button m_No;

    [Header("Notify button")]
    [SerializeField] GameObject m_NotifyButton;
    [SerializeField] Button m_Ok;

    protected static Dialog Instance;

    static bool NeedUpdateLayoutGroup = false;
    [SerializeField] RectTransform m_LayoutGroup;
    static RectTransform LayoutGroup => Instance.m_LayoutGroup;

    protected static Button.ButtonClickedEvent Yes => Instance.m_Yes.onClick;
    protected static Button.ButtonClickedEvent No => Instance.m_No.onClick;
    protected static Button.ButtonClickedEvent Ok => Instance.m_Ok.onClick;

    static void SetDialogType(DialogType type)
    {
        Instance.m_NotifyButton.SetActive(type == DialogType.Notify);
        Instance.m_QuestionButtons.SetActive(type == DialogType.Question);
    }

    protected static void SetHeaderAndMessage(string header, string message)
    {
        Instance.m_Header.text = header;
        Instance.m_Message.text = message;
        NeedUpdateLayoutGroup = true;
    }

    public static bool IsActive
    {
        get => Instance.gameObject.activeSelf;
        private set => Instance.gameObject.SetActive(value);
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init() => InstanceObject.InstantiatePrefab(nameof(Dialog));
    protected void Awake() => this.SetInstance(ref Instance);

    private void OnRenderObject()
    {
        if (NeedUpdateLayoutGroup)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_LayoutGroup);
            NeedUpdateLayoutGroup = false;
        }
    }

    public static async Task ShowNotify(string header, string message)
    {
        IsActive = true;

        SetDialogType(DialogType.Notify);
        SetHeaderAndMessage(header, message);

        bool pressed = false;
        Ok.AddListener(() => pressed = true);

        while (!pressed)
        {
            if (!Application.isPlaying || Input.GetKeyDown(KeyCode.Escape))
                pressed = true;

            await Task.Yield();
        }

        Ok.RemoveAllListeners();

        IsActive = false;
    }

    public static async Task<bool> ShowQuestion(string header, string message)
    {
        SetDialogType(DialogType.Question);
        SetHeaderAndMessage(header, message);

        IsActive = true;

        bool? ans = null;
        Yes.AddListener(() => ans = true);
        No.AddListener(() => ans = false);

        while (ans == null)
        {
            if (!Application.isPlaying) ans = false;

            await Task.Yield();
        }

        Yes.RemoveAllListeners();
        No.RemoveAllListeners();

        IsActive = false;

        return ans.Value;
    }
}
