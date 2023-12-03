using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    static bool JustLaunched = true;

#if !UNITY_EDITOR
    private async void JustLaunchedInvoke()
    {
        if (!JustLaunched) return;
        else JustLaunched = false;

        await Dialog.ShowNotify("Alpha!", "This is alpha version of Rhythm Paths!");
    }
#else
    private void JustLaunchedInvoke()
    {
        if (!JustLaunched) return;
        else JustLaunched = false;
    }
#endif

    private void Start()
    {
        JustLaunchedInvoke();
    }

    public void OpenCardEditor()
    {
        SceneLoader.LoadScene(1);
    }

    public async void Close()
    {
        bool ans = await Dialog.ShowQuestion("Quit?", "Are you sure to quit?");
        if(ans) Application.Quit();
    }
}