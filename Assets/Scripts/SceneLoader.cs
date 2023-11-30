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
    public Image Background,Logo;
    public TMPro.TMP_Text Text;
    public RectTransform Graphic;

    public void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public async static Task LoadSceneAsync(string sceneName)
        => await Instance.LoadScene(sceneName);

    public async Task LoadScene(string sceneName)
    {
        Background.GetComponent<GraphicRaycaster>().enabled = true;

        /*Graphic.SizeAnim(new Vector2(350, 0), 0.25f);
        await Background.ColorAnimAsync(new Color(0, 0, 0, 1), 0.25f);*/

        var Scene = SceneManager.LoadSceneAsync(sceneName);
        while (!Scene.isDone) await Task.Yield();
        await Task.Delay(100);

        /*Graphic.SizeAnim(new Vector2(0, 0), 0.25f);
        await Background.ColorAnimAsync(new Color(0, 0, 0, 0), 0.25f);*/

        Background.GetComponent<GraphicRaycaster>().enabled = false;
        Destroy(this.gameObject);
    }
}
