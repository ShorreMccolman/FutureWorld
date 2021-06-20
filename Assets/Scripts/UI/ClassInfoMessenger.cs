using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfoMessenger : MonoBehaviour, IPopable
{
    public void ShowPopup()
    {
        ProfilePopupInfo info = ProfilePopupInfoDatabase.Instance.GetInfo(Party.Instance.ActiveMember.Profile.Class.ToString());
        string title = info.Title;
        string body = info.Body;
        HUD.Instance.Popups.ShowText(title, body, 16);
    }
}