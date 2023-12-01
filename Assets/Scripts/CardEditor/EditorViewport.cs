using UnityEngine;
using RL.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace RL.CardEditor
{
    public class EditorViewport : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Events

        /// <summary>
        /// ???? ????? ??????? ???? ?? ? GUI ???????
        /// </summary>
        public UnityEvent<PointerEventData> OnLeftClick = new();
        /// <summary>
        /// ???? ?????? ??????? ???? ?? ? GUI ???????
        /// </summary>
        public UnityEvent<PointerEventData> OnRightClick = new();
        /// <summary>
        /// ???? ????????? ????? ?? ? GUI ???????
        /// </summary>
        public UnityEvent<PointerEventData> OnMiddleClick = new();
        /// <summary>
        /// ?????? ?????? ? GUI ???????
        /// </summary>
        public UnityEvent<PointerEventData> OnEnter = new();
        /// <summary>
        /// ?????? ??????? ?? GUI ???????
        /// </summary>
        public UnityEvent<PointerEventData> OnExit = new();
        /// <summary>
        /// ?????????? ?????? ???? ???? ?????? ????????? ? GUI ???????
        /// </summary>
        public UnityEvent<PointerEventData> OnStay = new();
        /// <summary>
        /// ?????????? ????? ?????? ????????? ? GUI ???????
        /// </summary>
        public UnityEvent<PointerEventData> OnMove = new();

        #endregion

        /// <summary>
        /// ???? true ?? ?????? ????????? ? GUI ???????
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