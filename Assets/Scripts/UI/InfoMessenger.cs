using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoMessenger : MonoBehaviour, IInfoMessenger
{
    public string Message;

    public string GetInfoMessage()
    {
        return Message;
    }
}
