using UnityEngine;
using UnityEngine.UI;

namespace RL.UI
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class ImagePropotions : MonoBehaviour
    {
        private Image Graphic;
        private RectTransform RT,ParentRect;
        public bool InFull = true;


        private bool OldInFull = true;
        private Rect OldParentRect;

        public void Start()
        {

            RT = GetComponent<RectTransform>();
            RT.anchorMin = new Vector2(0.5f, 0.5f);
            RT.anchorMax = new Vector2(0.5f, 0.5f);
            RT.pivot = new Vector2(0.5f, 0.5f);

            ParentRect = transform.parent.GetComponent<RectTransform>();

            new DrivenRectTransformTracker().Add(this, RT, DrivenTransformProperties.All);

            Graphic = GetComponent<Image>();
            OldParentRect = transform.parent.GetComponent<RectTransform>().rect;
            UpdatePropotion();
            OldInFull = InFull;
            
        }
        void Update()
        {
            if (ParentRect.rect.width != OldParentRect.width || ParentRect.rect.height != OldParentRect.height)
            {
                UpdatePropotion(); // при изменении размера родительского объекта обновляем пропорции 
                OldParentRect = ParentRect.rect; // Применяем новый Rect родительского объекта
            }
            if(OldInFull != InFull)
            {
                UpdatePropotion();
                OldInFull = InFull;
            }
        }
        void UpdatePropotion()
        {
            if (Graphic.sprite == null)
            {
                RT.sizeDelta = new Vector2(0,0);
                return;
            }
            Rect rect = ParentRect.rect;
            float w = Graphic.sprite.texture.width;
            float h = Graphic.sprite.texture.height;
            float ImagePropotion = w / h;
            float RectPropotion = rect.size.x / rect.size.y;
            if (InFull)
            {
                if (RectPropotion > ImagePropotion)
                {
                    RT.sizeDelta = new Vector2(rect.size.x, rect.size.x / ImagePropotion);
                }
                else
                {
                    RT.sizeDelta = new Vector2(rect.size.y * ImagePropotion, rect.size.y);
                }
            }
            else
            {

                if (RectPropotion < ImagePropotion)
                {
                    RT.sizeDelta = new Vector2(rect.size.x, rect.size.x / ImagePropotion);
                }
                else
                {
                    RT.sizeDelta = new Vector2(rect.size.y * ImagePropotion, rect.size.y);
                }
            }
        }

        public Sprite Sprite 
        { 
            get => Graphic.sprite; 
            set
            {
                Graphic.sprite = value;
                UpdatePropotion();
            } 
        }
    }
}