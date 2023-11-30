using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RL.GameEditor
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class HierarchyPoint : MonoBehaviour, IPointerClickHandler
    {
        public UnityEngine.Events.UnityEvent OnClick = new();
        public CardEditorPoint OriginalPoint;
        public RectTransform RectT;
        //public GameObject PerviousLine;
        public void OnPointerClick(PointerEventData eventData) => OnClick.Invoke();
    }
}
