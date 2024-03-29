using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FileBrowser : MonoBehaviour
{
    private static FileBrowser Instance;

    [SerializeField] private TMP_InputField PathInputField;
    [SerializeField] private Button PreviousButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button UpButton;

    [SerializeField] private GridLayoutGroup Content;
    [SerializeField] private Button FilePrefab;
    [SerializeField] private Button FolderPrefab;

    private DirectoryInfo _nextDirectory;

    List<DirectoryInfo> PreviousDirectories;
    private DirectoryInfo PreviousDirectory;
    public DirectoryInfo NextDirectory
    {
        get => _nextDirectory;
        private set
        {
            _nextDirectory = value;
            NextButton.interactable = value is not null;
        }
    }
    private DirectoryInfo CurrentDirectory;

#if UNITY_STANDALONE_OSX
    const string Root = "/";
#elif UNITY_STANDALONE_Win
    const string Root = "C:/";
#endif

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        InstanceObject.InstantiatePrefab("FileBrowser");
    }

    void Start()
    {
        InstanceObject.SetInstance(this, ref Instance, true);

        PathInputField.onEndEdit.AddListener(SelectDirectory);
        UpButton.onClick.AddListener(Up);
        PreviousButton.onClick.AddListener(Previous);
        NextButton.onClick.AddListener(Next);

        SelectDirectory(Root);
    }

    readonly List<GameObject> _elements = new();

    public void SelectDirectory(string path)
        => SelectDirectory(new DirectoryInfo(path));

    public void SelectDirectory(DirectoryInfo info)
    {
        _elements.ForEach((e) => Destroy(e));
        _elements.Clear();

        PathInputField.SetTextWithoutNotify(info.FullName);

        PreviousDirectory = CurrentDirectory;
        CurrentDirectory = info;
        NextDirectory = null;

        if (!info.Exists) throw new DirectoryNotFoundException();

        SpawnElements(FolderPrefab, info.GetDirectories(), SelectDirectory, (dis) => !dis.Name.StartsWith('.'));
        SpawnElements(FilePrefab, info.GetFiles(), SelectFile, (file) => !file.Name.StartsWith('.'));
    }

    public void Up()
    {
        if (CurrentDirectory.Parent == null) return;

        DirectoryInfo old = CurrentDirectory;
        SelectDirectory(CurrentDirectory.Parent);
        NextDirectory = old;
    }

    public void Previous()
    {
        if (PreviousDirectory == null) return;

        DirectoryInfo old = CurrentDirectory;
        SelectDirectory(PreviousDirectory);
        NextDirectory = old;
        NextButton.interactable = true;
    }

    public void Next()
    {
        SelectDirectory(NextDirectory);
    }

    void SpawnElements<T>(Button elementPrefab, T[] values, Action<T> action, Func<T, bool> predicate)
        where T : FileSystemInfo
    {
        for (int i = 0; i < values.Length; i++)
        {
            T value = values[i];
            if (!predicate(value)) continue;

            var button = Instantiate(elementPrefab, Content.transform);
            button.onClick.AddListener(() => action(value));

            var text = button.GetComponentInChildren<TMP_Text>();
            text.text = value.Name;

            _elements.Add(button.gameObject);
        }
    }

    private static UnityEvent<FileInfo> OnFileSelected = new();

    void SelectFile(FileInfo file)
    {
        OnFileSelected.Invoke(file);

        Debug.Log("File " + file.Name + " selected");
    }

    #region Anim

    static Coroutine _coroutine;
    [SerializeField] RectTransform _browser;
    [SerializeField] CanvasGroup _canvasGroup;

    static float OutExpo(float x) => x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x);

    void SetStep(float t)
    {
        _browser.localScale = new(0.9f + (0.1f * t), 0.9f + (0.1f * t));
        _canvasGroup.alpha = t;
    }

    static IEnumerator Anim(float duration, Action<float> action, bool invert = false)
    {
        Instance._canvasGroup.blocksRaycasts = invert;

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

    public async static Task<FileInfo> Open()
    {
        if (_coroutine != null) Instance.StopCoroutine(_coroutine);
        _coroutine = Instance.StartCoroutine(Anim(0.5f, Instance.SetStep, true));

        FileInfo file = null;
        OnFileSelected.AddListener((selectedFile) => file = selectedFile);

        while (file == null) await Task.Yield();
        Hide();

        OnFileSelected.RemoveAllListeners();

        return file;
    }

    static void Hide()
    {
        if (_coroutine != null) Instance.StopCoroutine(_coroutine);
        _coroutine = Instance.StartCoroutine(Anim(0.5f, Instance.SetStep, false));
    }
}
