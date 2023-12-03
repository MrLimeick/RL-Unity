using UnityEngine;
using RL.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace RL.CardEditor
{
    public class EditorViewport : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Events

        public UnityEvent<PointerEventData> OnLeftClick = new();
        public UnityEvent<PointerEventData> OnRightClick = new();
        public UnityEvent<PointerEventData> OnMiddleClick = new();
        public UnityEvent<PointerEventData> OnEnter = new();
        public UnityEvent<PointerEventData> OnExit = new();
        public UnityEvent<PointerEventData> OnStay = new();
        public UnityEvent<PointerEventData> OnMove = new();

        #endregion

        public static bool IsStay;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerPress != gameObject) return;
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left: OnLeftClick?.Invoke(eventData); break;
                case PointerEventData.InputButton.Right: OnRightClick?.Invoke(eventData); break;
                case PointerEventData.InputButton.Middle: OnMiddleClick?.Invoke(eventData); break;
            }
        }
        public void OnPointerMove(PointerEventData eventData)
        {
            OnMove.Invoke(eventData);
        }
        private void FixedUpdate()
        {
            if (IsStay)
            {
                PointerEventData pointer = new(EventSystem.current);
                pointer.position = Input.mousePosition;

                OnStay.Invoke(pointer);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            IsStay = false;
            OnExit.Invoke(eventData);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsStay = true;
            OnEnter.Invoke(eventData);
        }
    }
}