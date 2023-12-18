using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/Context menu")]
[RequireComponent(typeof(RectTransform))]
public class ContextMenu : Selectable
{
    public class Event : UnityEvent<int> { }

    public class Item : MonoBehaviour, IPointerEnterHandler, ICancelHandler
    {
        public TMP_Text Text;
        public RectTransform RectTransform;

        public void OnCancel(BaseEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ContextMenu contextMenu = GetComponentInParent<ContextMenu>();
            if (contextMenu) contextMenu.Hide();
        }
    }

    public class OptionData
    {
        public string Text;

        public OptionData(string text)
        {
            Text = text;
        }
    }

    public class OptionDataList
    {
        public List<OptionData> Options { get; set; }

        public OptionDataList()
        {
            Options = new List<OptionData>();
        }
    }

    [SerializeField]
    private RectTransform _template;
    public RectTransform Template
    {
        get => _template;
        set
        {
            _template = value;
            RefreshShownValue();
        }
    }

    [SerializeField]
    private TMP_Text _itemText;
    public TMP_Text ItemText
    {
        get => _itemText;
        set
        {
            _itemText = value;
            RefreshShownValue();
        }
    }

    private readonly OptionDataList _options = new();
    public List<OptionData> Options
    {
        get => _options.Options;
        set
        {
            _options.Options = value;
            RefreshShownValue();
        }
    }

    public Event Selected = new();

    public float AlphaFadeSpeed = .15f;

    private GameObject _contextMenu;
    private GameObject _blocker;

    public void RefreshShownValue()
    {

    }

    public void Show()
    {

    }

    public void Hide()
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Canvas canvas = GetComponentInParent<Canvas>();

        Debug.Log(canvas == null);
        if(canvas != null)
            Debug.Log(canvas.gameObject.name);

        Canvas canvas2 = canvas.transform.parent.GetComponentInParent<Canvas>();
        if (canvas2 != null)
            Debug.Log("canvas2: " + canvas2.name);
    }

    //protected virtual GameObject CreateBlocker(Canvas rootCanvas)
    //{
    //    // Create blocker GameObject.
    //    GameObject blocker = new GameObject("Blocker");

    //    // Setup blocker RectTransform to cover entire root canvas area.
    //    RectTransform blockerRect = blocker.AddComponent<RectTransform>();
    //    blockerRect.SetParent(rootCanvas.transform, false);
    //    blockerRect.anchorMin = Vector3.zero;
    //    blockerRect.anchorMax = Vector3.one;
    //    blockerRect.sizeDelta = Vector2.zero;

    //    // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
    //    Canvas blockerCanvas = blocker.AddComponent<Canvas>();
    //    blockerCanvas.overrideSorting = true;
    //    Canvas dropdownCanvas = _contextMenu.GetComponent<Canvas>();
    //    blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
    //    blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

    //    // Find the Canvas that this dropdown is a part of
    //    Canvas parentCanvas = null;
    //    Transform parentTransform = _template.parent;
    //    while (parentTransform != null)
    //    {
    //        parentCanvas = parentTransform.GetComponent<Canvas>();
    //        if (parentCanvas != null)
    //            break;

    //        parentTransform = parentTransform.parent;
    //    }

    //    // If we have a parent canvas, apply the same raycasters as the parent for consistency.
    //    if (parentCanvas != null)
    //    {
    //        Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
    //        for (int i = 0; i < components.Length; i++)
    //        {
    //            Type raycasterType = components[i].GetType();
    //            if (blocker.GetComponent(raycasterType) == null)
    //            {
    //                blocker.AddComponent(raycasterType);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        // Add raycaster since it's needed to block.
    //        GetOrAddComponent<GraphicRaycaster>(blocker);
    //    }


    //    // Add image since it's needed to block, but make it clear.
    //    Image blockerImage = blocker.AddComponent<Image>();
    //    blockerImage.color = Color.clear;

    //    // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
    //    Button blockerButton = blocker.AddComponent<Button>();
    //    blockerButton.onClick.AddListener(Hide);

    //    return blocker;
    //}
}
