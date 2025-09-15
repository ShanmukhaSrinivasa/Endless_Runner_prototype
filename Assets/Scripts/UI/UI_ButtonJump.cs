using UnityEngine;
using UnityEngine.EventSystems;

public class UI_JumpSlashButtons : MonoBehaviour , IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.instance.player.jumpButton();
    }
}
