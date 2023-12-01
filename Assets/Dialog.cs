using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Dialog : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text m_Header;
    [SerializeField] private TMP_Text m_Message;

    private static string Header { set => Instance.m_Header.text = value; }
    private static string Message { set => Instance.m_Message.text = value; }

    [Header("Buttons")]
    [SerializeField] private Button m_Yes;
    [SerializeField] private Button m_No;

    private static Button.ButtonClickedEvent Yes => Instance.m_Yes.onClick;
    private static Button.ButtonClickedEvent No => Instance.m_No.onClick;

    public static bool IsActive
    {
        get => Instance.gameObject.activeSelf;
        private set => Instance.gameObject.SetActive(value);
    }


    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        //var prefab = Resources.InstanceIDToObject(20944) as GameObject;
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

    public static async Task<bool> ShowDialog(string header, string message)
    {
        IsActive = true;

        Header = header;
        Message = message;

        bool? ans = null;
        Yes.AddListener(() => ans = true);
        No.AddListener(() => ans = false);

        while (ans == null) await Task.Yield();

        Yes.RemoveAllListeners();
        No.RemoveAllListeners();

        IsActive = false;
        return ans.Value;
    }
}
