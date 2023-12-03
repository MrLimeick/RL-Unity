using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private TMPro.TMP_Text m_Text;
    public static string Text
    {
        get => Instance.m_Text.text;
        set => Instance.m_Text.text = value;
    }

    public static bool IsActive
    {
        get => Instance.gameObject.activeSelf;
        set => Instance.gameObject.SetActive(value);
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        var prefab = Resources.Load<GameObject>("Prefabs/SceneLoader");
        Instantiate(prefab);
    }

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    public static void LoadScene(int buildIndex)
        => LoadSceneInternal(() => SceneManager.LoadSceneAsync(buildIndex));

    public static void LoadScene(string sceneName)
        => LoadSceneInternal(() => SceneManager.LoadSceneAsync(sceneName));

    private static async void LoadSceneInternal(Func<AsyncOperation> loadScene)
    {
        IsActive = true;

        var scene = loadScene();
        while (!scene.isDone)
        {
            Text = $"Loading… {Mathf.Round(scene.progress * 100)}";
            await Task.Yield();
        }

        IsActive = false;
    }
}
