using System.Collections;
using System.Collections.Generic;
using RL.CardEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GridSettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class Content
    {
        public Toggle UseGlobalGrid;
        public Toggle UseCircleGrid;

        public Vector2Field CircleGridResolution;
        public Vector2Field SquareGridResolution;
    }

    private Animation Anim;
    private RectTransform RectTransform;

    [Header("Sizes")]

    [SerializeField] private Vector2 OpennedSize = new(128, 128);
    [SerializeField] private Vector2 ClosedSize = new(48, 48);

    [Header("Groups")]

    [SerializeField] private CanvasGroup OpennedGroup;
    [SerializeField] private CanvasGroup ClosedGroup;

    [Space]

    [SerializeField] private Content _content;

    private void Awake()
    {
        Anim = new(this)
        {
            Action = AnimSteps,
            Duration = 0.2f,
            UnscaledTime = false
        };

        RectTransform = GetComponent<RectTransform>();

        _content.UseGlobalGrid.onValueChanged.AddListener((v) => PathMaker.GlobalGrid = v);
        _content.UseCircleGrid.onValueChanged.AddListener((v) => PathMaker.BuildMode = v ? PathMaker.BuildModes.LockedHeight : PathMaker.BuildModes.ByGrid);
        _content.CircleGridResolution.onValueChanged.AddListener((v) => PathMaker.Grids.Radius = v.x);
        _content.SquareGridResolution.onValueChanged.AddListener((v) => PathMaker.Grids.Resolution = v);
    }
    
    public void Show()
    {
        d = false;
        Anim.Stop();
        OpennedGroup.blocksRaycasts = true;
        Anim.Start();
    }

    public void Hide()
    {
        d = true;
        Anim.Stop();
        OpennedGroup.blocksRaycasts = false;
        Anim.StartInverted();
    }

    bool d = false;
    protected virtual void AnimSteps(float t)
    {
        float a = Mathf.Max(0, t);
        float b = Mathf.Max(0, (t - 0.5f) * 2f);
        float c = Mathf.Max(0, (1 - t) * 2 - 1f);

        a = d ? Easing.InExpo(a) : Easing.OutExpo(a);
        b = d ? Easing.InExpo(b) : Easing.OutExpo(b);
        c = d ? Easing.OutExpo(c) : Easing.InExpo(c);


        RectTransform.sizeDelta = Vector2.Lerp(ClosedSize, OpennedSize, a);
        OpennedGroup.alpha = a;
        ClosedGroup.alpha = 1 - a;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        => Hide();

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        => Show();
}
