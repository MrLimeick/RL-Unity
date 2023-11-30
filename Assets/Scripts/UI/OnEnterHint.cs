using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OnEnterHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform hint;

    public void OnValidate()
    {
        hint = transform.Find("Hint").GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hint != null) hint.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hint != null) hint.gameObject.SetActive(false);
    }
}
