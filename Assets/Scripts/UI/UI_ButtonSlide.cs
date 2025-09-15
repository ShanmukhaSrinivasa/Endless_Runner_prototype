using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ButtonSlide : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.instance.player.slidingButton();
    }
}
