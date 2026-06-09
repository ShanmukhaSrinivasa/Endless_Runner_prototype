using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ButtonSlide : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.IsSinglePlayer())
        {
            GameManager.instance.singlePlayerPlayer?.slidingButton();
        }
        else
        {
            GameManager.instance.networkPlayer?.slidingButton();
        }
    }
}
