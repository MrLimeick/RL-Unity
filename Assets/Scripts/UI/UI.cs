using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;


namespace RL.UI
{
    [AddComponentMenu("RL/UI/Default UI Tools",0)]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(RectTransform))]
    public class UI : MonoBehaviour
    {
        #region Hide/UnHide animation

        public Vector2 PivotHidePosition = new(0, 0);
        public Vector2 PivotUnHidePosition = new(1, 0);
        public AnimationType Animation = AnimationType.PivotMove;

        public enum AnimationType
        {
            ///ускользание, исчизновение
            PivotMove, Alpha
        }

        public float Lenght = 0.5f;
        public bool IsHide = false;
        public bool DisableRaycastOnHide = true;
        private CancellationTokenSource Cancellation = new();

        public void HideToggle(bool fast = false)
        {
            IsHide = !IsHide;
            if (IsHide) Hide(fast);
            else Show(fast);
        }

        public void ShowAsync()
        {
            IsHide = false;
            GetComponent<GraphicRaycaster>().enabled = true;

            if (Cancellation != null) Cancellation.Cancel();
            Cancellation = new();
            
            switch (Animation)
            {
                case AnimationType.PivotMove:
                    /*await RT.PivotAnimAsync(PivotUnHidePosition, Lenght,Cancellation.Token);*/
                    break;
               /* #region Alpha Animation
                case AnimationType.Alpha:
                    StartCoroutine(ColorAlphaAnim(transform,1, Lenght));
                    break;
                    #endregion*/
            }
            if (Cancellation != null && Cancellation.IsCancellationRequested) return;
            Cancellation = null;
        }
        public void HideAsync()
        {

            IsHide = true;
            GetComponent<GraphicRaycaster>().enabled = !DisableRaycastOnHide;

            Cancellation?.Cancel();
            Cancellation = new();

            switch (Animation)
            {
                case AnimationType.PivotMove:
                      /*await RT.PivotAnimAsync(PivotHidePosition, Lenght, Cancellation.Token);*/
                    break;
                /*case AnimationType.Alpha:
                        StartCoroutine(ColorAlphaAnim(transform, 0, Lenght));
                    break;*/
            }
            if (Cancellation != null && Cancellation.IsCancellationRequested) return;
            Cancellation = null;
        }
        public void Hide(bool fast = false)
        {
            if (fast)
            {
                switch (Animation)
                {
                    case AnimationType.PivotMove:
                        GetComponent<RectTransform>().pivot = PivotHidePosition;
                        break;
                    #region Alpha Animation
                    case AnimationType.Alpha:
                        SetAlphaToSelfAndChilds(transform, 0);
                        break;
                        #endregion
                }
                return;
            }
            HideAsync();
        }
        public void Show(bool fast = false)
        {
            if (fast)
            {
                switch (Animation)
                {
                    case AnimationType.PivotMove:
                        GetComponent<RectTransform>().pivot = PivotUnHidePosition;
                        break;
                    #region Alpha Animation
                    case AnimationType.Alpha:
                        SetAlphaToSelfAndChilds(transform, 1);
                        break;
                        #endregion
                }
                return;
            }
            ShowAsync();
        }
        public void SetAlphaToSelfAndChilds(Transform child,float alpha)
        {
            for (int i = 0; i < child.transform.childCount; i++)
            {
                Transform NextChild = child.transform.GetChild(i);
                SetAlphaToSelfAndChilds(NextChild, alpha);
            }
            if (child.TryGetComponent(out Graphic component))
            {
                component.color = new Color(component.color.r, component.color.g, component.color.b, alpha);
                //child.gameObject.SetActive((alpha == 0) ? false : true);
            }
        }
        private IEnumerator ColorAlphaAnim(Transform child, float To, float time)
        {
            for (int i = 0; i < child.transform.childCount; i++)
            {
                Transform n_child = child.transform.GetChild(i);
                
                StartCoroutine(ColorAlphaAnim(n_child, To, time));
            }

            if(child.TryGetComponent(out Graphic component))
            {
                float StartTime = Time.unscaledTime;
                float From = component.color.a;

                float localTime;
                while ((localTime = (Time.unscaledTime - StartTime) / time) < 1f)
                {
                    yield return new WaitForEndOfFrame();
                    component.color = new Color(component.color.r, component.color.g, component.color.b,
                        Mathf.Lerp(From, To, localTime));
                }
                component.color = new Color(component.color.r, component.color.g, component.color.b, To);
            }
        }
        #endregion
        
        private void Awake()
        {
            if (IsHide) Hide(true);
            else Show(true);
        }
        public void ActiveToogle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void OnEnable()
        {
            if (IsHide) Hide(true);
            else Show(true);
        }
    }
}
