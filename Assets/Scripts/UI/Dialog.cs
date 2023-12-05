using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Dialog : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text m_Header;
    [SerializeField] private TMP_Text m_Message;

    private static string Header { set => Instance.m_Header.text = value; }
    private static string Message { set => Instance.m_Message.text = value; }

    [Header("Question buttons")]
    [SerializeField] private GameObject m_QuestionButtons;
    [SerializeField] private Button m_Yes;
    [SerializeField] private Button m_No;

    [Header("Notify button")]
    [SerializeField] private GameObject m_NotifyButton;
    [SerializeField] private Button m_Ok;

    private static Button.ButtonClickedEvent Yes => Instance.m_Yes.onClick;
    private static Button.ButtonClickedEvent No => Instance.m_No.onClick;
    private static Button.ButtonClickedEvent Ok => Instance.m_Ok.onClick;

    private enum DialogType
    {
        Question,
        Notify
    }

    private static void SetDialogType(DialogType type)
    {
        switch(type)
        {
            case DialogType.Question:
                Instance.m_NotifyButton.SetActive(false);
                Instance.m_QuestionButtons.SetActive(true);
                break;
            case DialogType.Notify:
                Instance.m_NotifyButton.SetActive(true);
                Instance.m_QuestionButtons.SetActive(false);
                break;
        }
    }

    public static bool IsActive
    {
        get => Instance.gameObject.activeSelf;
        private set => Instance.gameObject.SetActive(value);
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        var prefab = Resources.Load<GameObject>("Prefabs/Dialog");
        Instantiate(prefab);
    }

    private static Dialog Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gameObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    public static async Task ShowNotify(string header, string message)
    {
        SetDialogType(DialogType.Notify);

        IsActive = true;

        Header = header;
        Message = message;

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

        IsActive = true;

        Header = header;
        Message = message;

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
