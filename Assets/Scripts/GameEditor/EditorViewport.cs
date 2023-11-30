using UnityEngine;
using RL.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace RL.GameEditor
{
    public class EditorViewport : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Events

        /// <summary>
        /// Клик левой кнопкой мыши по в GUI объекту
        /// </summary>
        public UnityEvent<PointerEventData> OnLeftClick = new();
        /// <summary>
        /// Клик правой кнопкой мыши по в GUI объекту
        /// </summary>
        public UnityEvent<PointerEventData> OnRightClick = new();
        /// <summary>
        /// Клик колёсиком мышки по в GUI объекту
        /// </summary>
        public UnityEvent<PointerEventData> OnMiddleClick = new();
        /// <summary>
        /// курсор входит в GUI объекте
        /// </summary>
        public UnityEvent<PointerEventData> OnEnter = new();
        /// <summary>
        /// курсор выходит из GUI объекта
        /// </summary>
        public UnityEvent<PointerEventData> OnExit = new();
        /// <summary>
        /// Вызывается каждый кадр пока курсор находится в GUI объекте
        /// </summary>
        public UnityEvent<PointerEventData> OnStay = new();
        /// <summary>
        /// Вызывается когда курсор двигается в GUI объекте
        /// </summary>
        public UnityEvent<PointerEventData> OnMove = new();

        #endregion

        /// <summary>
        /// Если true то курсор находится в GUI объекте
        /// </summary>
        public static bool isStay;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerPress == gameObject)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                    OnLeftClick.Invoke(eventData);
                if (eventData.button == PointerEventData.InputButton.Right)
                    OnRightClick.Invoke(eventData);
                if (eventData.button == PointerEventData.InputButton.Middle)
                    OnMiddleClick.Invoke(eventData);
            }
        }
        public void OnPointerMove(PointerEventData eventData)
        {
            OnMove.Invoke(eventData);
        }
        private void FixedUpdate()
        {
            PointerEventData pointer = new(EventSystem.current);
            pointer.position = Input.mousePosition;

            if (isStay) OnStay.Invoke(pointer);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            isStay = false;
            OnExit.Invoke(eventData);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            isStay = true;
            OnEnter.Invoke(eventData);
        }
    }
}