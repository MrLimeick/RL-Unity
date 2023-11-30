using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackground : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    RectTransform RT;
    Image Graphic;

    public Sprite[] DefaultImage;
    void Start()
    {
        RT = GetComponent<RectTransform>();
        Graphic = GetComponent<Image>();
        Graphic.sprite = DefaultImage[Random.Range(0, DefaultImage.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        if (Graphic.sprite != null)
        {
            var ScreenResolution = new Vector2(Screen.width, Screen.height);
            var SpriteResolution = new Vector2(Graphic.sprite.texture.width, Graphic.sprite.texture.height);
            float YonX = SpriteResolution.y / SpriteResolution.x;
            float XonY = SpriteResolution.x / SpriteResolution.y;

            float ScreenAsspect = ScreenResolution.y / ScreenResolution.x;
            if (ScreenAsspect > YonX)
            {
                RT.sizeDelta = new Vector2(ScreenResolution.y * XonY + 20, Screen.height + 20);
            }
            else
            {
                RT.sizeDelta = new Vector2(Screen.width + 20, ScreenResolution.x * YonX + 20);
            }
        }
        RT.sizeDelta /= canvas.scaleFactor;
        Vector2 mousePos = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition) - new Vector2(0.5f, 0.5f);
        RT.anchoredPosition = new Vector2(20 * mousePos.x, 20 * mousePos.y);
    }
}
