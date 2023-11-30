using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
public class MapListButtonTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool IsStay;
    private Coroutine StayAnim;
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsStay = true;
        /*if (StayAnim != null) StopCoroutine(StayAnim);
        StayAnim = StartCoroutine(StayAnimation());*/
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsStay = false;
    }

    public Quaternion Rot
    {
        get
        {
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 LocalPositon = MousePos - (Vector2)transform.position;
            LocalPositon = new Vector2(Mathf.Clamp(LocalPositon.x, -1, 1), Mathf.Clamp(LocalPositon.y, -1.5f, 1.5f) / 1.5f);

            return Quaternion.Euler(new Vector3(30 * LocalPositon.y, 30 * -LocalPositon.x, 0));
        }
    }
    /*IEnumerator StayAnimation()
    {
        float OldTime = Time.unscaledTime;
        float localTime;

        Quaternion OldRot = transform.GetChild(0).rotation;
        
        while ((localTime = (Time.unscaledTime - OldTime) / 0.2f) < 1)
        {
            transform.GetChild(0).rotation = Quaternion.Lerp(OldRot, Rot, Easing.Quart.Out(localTime));
        
            yield return new WaitForEndOfFrame();
        }
        while (IsStay)
        {
            transform.GetChild(0).rotation = Rot;
        
            yield return new WaitForEndOfFrame();
        }
        OldTime = Time.unscaledTime;

        OldRot = Rot;
        while ((localTime = (Time.unscaledTime - OldTime) / 0.2f) < 1)
        {
            transform.GetChild(0).rotation = Quaternion.Lerp(OldRot, Quaternion.Euler(new Vector3(0,0,0)),Easing.Quart.Out(localTime));

            yield return new WaitForEndOfFrame();
        }
    }*/
    void Update()
    {
        
    }
}
