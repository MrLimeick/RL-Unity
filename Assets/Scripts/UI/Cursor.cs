using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RL.UI
{
    public class Cursor : MonoBehaviour
    {
        #region Rainbow Cursor
        /// <summary>
        /// Радужный курсор
        /// </summary>
        public bool RainbowCursor = false;
        /// <summary>
        /// Скорость изменения цвета у радужного курсора
        /// </summary>
        public float RainbowSpeed = 1;
        private Coroutine RainbowAnimation;
        /// <summary>
        /// Оттенок курсора на данный момент
        /// </summary>
        public float Hue { get; private set; }
        #endregion
        /// <summary>
        /// Показывать ли оригинальный курсор(Windows) или кастомный
        /// </summary>
        public bool ViewOriginalCursor = false;
        public bool CustomCursor = false;

        public TrailRenderer Trail;
        public SpriteRenderer SR;
        /// <summary>
        /// Цвет курсора
        /// </summary>
        public Color Color;
        [HideInInspector]
        public Gradient TrailColor { get; private set; }
        public float scale = 0.25f;
        void Start()
        {
            if(!ViewOriginalCursor) UnityEngine.Cursor.visible = false;
            if(RainbowCursor) StartCoroutine(RainbowCursorCoroutine());
            if(SR == null) SR = GetComponent<SpriteRenderer>();
            TrailColor = new Gradient();
            SR.color = Color; 
            TrailColor.alphaKeys = new GradientAlphaKey[] { new(   1f, 0f), new(   0f, 1f) };
            TrailColor.colorKeys = new GradientColorKey[]
            {
                new(Color, 0f),
                new(Color, 1f)
            }; 
            Trail.colorGradient = TrailColor;
        }
        void Update()
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localScale = new Vector3((Camera.main.orthographicSize / 5), (Camera.main.orthographicSize / 5),0);
            Trail.widthCurve = new AnimationCurve(new Keyframe[] { new(0f, (Camera.main.orthographicSize / 5) * scale), new(1f, 0f) });
            if(UnityEngine.Cursor.visible == true && !ViewOriginalCursor) UnityEngine.Cursor.visible = false;
            if (RainbowAnimation == null && RainbowCursor) RainbowAnimation = StartCoroutine(RainbowCursorCoroutine());
            if (ViewOriginalCursor) 
            { 
                SR.gameObject.SetActive(false);
                UnityEngine.Cursor.visible = true;
            }
            else SR.gameObject.SetActive(true);
            SR.gameObject.transform.localScale = new Vector3(scale,scale,0);
            if (CustomCursor)
            {
                TrailColor.alphaKeys = new GradientAlphaKey[] 
                { 
                    new(0f, 0.02f),
                    new(1f, 0.05f),
                    new(0f, 1f) };
                TrailColor.colorKeys = new GradientColorKey[]
                {
                    new(Color, 0f),
                    new(Color, 1f)
                };
                Trail.colorGradient = TrailColor;
            }
            else
            {
                TrailColor.alphaKeys = new GradientAlphaKey[] { new(1f, 0f), new(0f, 1f) };
                TrailColor.colorKeys = new GradientColorKey[]
                {
                new(Color, 0f),
                new(Color, 1f)
                };
                Trail.colorGradient = TrailColor;
            }
        }
        private IEnumerator RainbowCursorCoroutine()
        {
            while (RainbowCursor)
            {
                yield return 1;
                if (Hue >= 1) Hue = 0;
                else Hue += 0.0025f * RainbowSpeed;
                Color = Color.HSVToRGB(Hue, 1, 1);
                SR.color = Color;
                TrailColor.colorKeys = new GradientColorKey[]
                {
                    new(Color, 0f),
                    new(Color, 1f)
                };
                Trail.colorGradient = TrailColor;
            }
            RainbowAnimation = null;
        }
    }
}
