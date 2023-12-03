using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Screenshoter : MonoBehaviour
{
    private Screenshoter Instance;

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        var prefab = Resources.Load<GameObject>("Prefabs/Screenshoter");
        Instantiate(prefab);
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
        gameObject.hideFlags = HideFlags.HideAndDontSave;
    }

    public static void Take()
    {
        if (!Directory.Exists("Screenshots")) Directory.CreateDirectory("Screenshots");
        ScreenCapture.CaptureScreenshot($"Screenshots/{DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}.png");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) Take();
    }
}

