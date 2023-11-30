using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace RL.UI
{
    public class RLButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public UnityEvent onClick;
        private bool IsEnter;
        private Coroutine Anim;
        public Vector2
            SizeDown = new Vector2(0.9f, 0.9f),
            SizeUp = new Vector2(1f, 1f),
            SizeEnter = new Vector2(1.1f, 1.1f),
            SizeExit = new Vector2(1f, 1f);
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsEnter = true;
            if (Anim != null) StopCoroutine(Anim);
            Anim = StartCoroutine(EnterAnim());
        }
        IEnumerator EnterAnim()
        {
            float oldTime = Time.time;
            Vector2 OldScale = transform.localScale;
            while (Time.time - oldTime < 0.1f)
            {
                transform.localScale = Vector2.Lerp(OldScale, SizeEnter, (Time.time - oldTime) / 0.1f);
                yield return 1;
            }
            transform.localScale = SizeEnter;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsEnter = false;
            if (Anim != null) StopCoroutine(Anim);
            Anim = StartCoroutine(ExitAnim());
        }
        IEnumerator ExitAnim()
        {
            float oldTime = Time.time;
            Vector2 OldScale = transform.localScale;
            while (Time.time - oldTime < 0.1f)
            {
                transform.localScale = Vector2.Lerp(OldScale, SizeExit, (Time.time - oldTime) / 0.1f);
                yield return 1;
            }
            transform.localScale = SizeExit;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (Anim != null) StopCoroutine(Anim);
            Anim = StartCoroutine(DownAnim());
        }
        IEnumerator DownAnim()
        {
            float oldTime = Time.time;
            Vector2 OldScale = transform.localScale;
            while (Time.time - oldTime < 0.1f)
            {
                transform.localScale = Vector2.Lerp(OldScale, SizeDown, (Time.time - oldTime) / 0.1f);
                yield return 1;
            }
            transform.localScale = SizeDown;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (Anim != null) StopCoroutine(Anim);
            Anim = StartCoroutine(UpAnim());
            onClick.Invoke();
        }
        IEnumerator UpAnim()
        {
            float oldTime = Time.time;
            Vector2 OldScale = transform.localScale;
            while (Time.time - oldTime < 0.1f)
            {
                transform.localScale = Vector2.Lerp(OldScale, SizeUp, (Time.time - oldTime) / 0.1f);
                yield return 1;
            }
            transform.localScale = SizeUp;
            if (IsEnter) StartCoroutine(EnterAnim());
        }

    }
}