using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
	private RectTransform rectTransform; // TODO: Animation
	private CanvasGroup canvasGroup;

    private void Awake()
    {
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
	{
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
	}

	public void Hide()
	{
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}

