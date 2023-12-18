using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

namespace RL.UI
{
    [AddComponentMenu("RL/UI/Button")]
    public class Button : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
        }
    }
}