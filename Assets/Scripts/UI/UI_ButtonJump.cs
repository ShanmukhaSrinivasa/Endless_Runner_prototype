using UnityEngine;
using UnityEngine.EventSystems;

public class UI_JumpSlashButtons : MonoBehaviour , IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.IsSinglePlayer())
        {
            GameManager.instance.singlePlayerPlayer?.jumpButton();
        }
        else
        {
            GameManager.instance.networkPlayer?.jumpButton();
        }
    }
}
