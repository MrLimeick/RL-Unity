using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class Message : MonoBehaviour
{
    public static Message Instance;
    public Image Background;
    public RectTransform Graphic;
    public TMPro.TMP_InputField Text;
    public Button OKButton;
    public Button CancelButton;
    public void Awake()
    {
        if(Instance != null) Destroy(Instance.gameObject);
        Instance = this;
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public async Task ShowNotifyMessageAsync(string text)
    {
        CancelButton.gameObject.SetActive(false);
        Text.text = text;

        var AS = GetComponent<AudioSource>();
        AS.Stop();
        AS.Play();

        /*await ShowBackgroundAndGraphic();*/
        Background.GetComponent<GraphicRaycaster>().enabled = true;
        bool OKPressed = false;
        OKButton.onClick.AddListener(() => OKPressed = true);
        while (!OKPressed)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) break;
            await Task.Yield();
        }
        /*await HideBackgroundAndGraphic();*/
        Background.GetComponent<GraphicRaycaster>().enabled = false;
    }
    public async void ShowNotifyMessage(string text)
    {
        CancelButton.gameObject.SetActive(false);
        Text.text = text;

        var AS = GetComponent<AudioSource>();
        AS.Stop();
        AS.Play();

        /*await ShowBackgroundAndGraphic();*/
        Background.GetComponent<GraphicRaycaster>().enabled = true;
        bool OKPressed = false;
        OKButton.onClick.AddListener(() => OKPressed = true);
        while (!OKPressed)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) break;
            await Task.Yield();
        }
        /*await HideBackgroundAndGraphic();*/
        Background.GetComponent<GraphicRaycaster>().enabled = false;
    }
    /*private async Task ShowBackgroundAndGraphic()
    {
        *//*Background.ColorAnim(new Color(0, 0, 0, 0.9f), 0.25f);
        await Graphic.SizeAnimAsync(new Vector2(350, 0), 0.25f);*//*
    }
    private async Task HideBackgroundAndGraphic()
    {
        Background.ColorAnim(new Color(0, 0, 0, 0f),0.25f);
        await Graphic.SizeAnimAsync(new Vector2(0, 0), 0.25f);
    }*/

    public enum PressedButton
    {
        Nothing,OK,Cancel
    }
    public async Task<bool> ShowQuestionMessage(string text)
    {
        Text.text = text;
        CancelButton.gameObject.SetActive(true);

        var AS = GetComponent<AudioSource>();
        AS.Stop();
        AS.Play();

        /*await ShowBackgroundAndGraphic();*/
        Background.GetComponent<GraphicRaycaster>().enabled = true;
        PressedButton PressedButton = PressedButton.Nothing;
        OKButton.onClick.AddListener(() => PressedButton = PressedButton.OK);
        CancelButton.onClick.AddListener(() => PressedButton = PressedButton.Cancel);
        while (PressedButton == PressedButton.Nothing) await Task.Yield();
        /*await HideBackgroundAndGraphic();*/
        CancelButton.gameObject.SetActive(false);
        Background.GetComponent<GraphicRaycaster>().enabled = false;
        if (PressedButton == PressedButton.OK)
            return true;
        else
            return false;
    }

}
