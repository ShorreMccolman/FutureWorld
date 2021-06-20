using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInfoMessenger : MonoBehaviour, IPopable
{
    public void ShowPopup()
    {
        HUD.Instance.Popups.ShowText("New Sorpigal", "");
    }
}
