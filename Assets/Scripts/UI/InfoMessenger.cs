using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoMessenger : MonoBehaviour, IPointerEnterHandler
{
    public string Message;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        HUD.Instance.SendInfoMessage(Message);
    }
}
