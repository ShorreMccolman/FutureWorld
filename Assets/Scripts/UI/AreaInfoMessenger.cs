using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInfoMessenger : MonoBehaviour, IPopable
{
    public void ShowPopup()
    {
        Popups.ShowText("New Sorpigal", "");
    }
}
